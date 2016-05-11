using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public static class DebugLogHandler
{
	private static bool logErrorHasBeenSendOnce = false;

	private static Queue<Dictionary<string, object>> logContextQueue = new Queue<Dictionary<string, object>>();

	private static int maxLogContextQueueCount = 15;

	private static int sampleErrorFrequency = 100;

	private static HashSet<string> ignoreLogStrings = new HashSet<string> { "Fullscreen mode can only be enabled in the web player after clicking on the content." };

	public static void Init()
	{
		Application.logMessageReceived += HandleLog;
	}

	private static void HandleLog(string logString, string stackTrace, LogType type)
	{
		if (logErrorHasBeenSendOnce)
		{
			return;
		}
		if (type == LogType.Warning || type == LogType.Log || IsIgnored(logString))
		{
			AddLogToLogContext(logString, type);
			return;
		}
		logErrorHasBeenSendOnce = true;
		if (MVClientSettings.IsDebugMode)
		{
			try
			{
				MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, logString + ": " + stackTrace);
			}
			catch
			{
			}
		}
		bool flag = Random.Range(0, sampleErrorFrequency + 1) == sampleErrorFrequency;
		if (MVClientSettings.EnableSentry || flag)
		{
			MVGameControllerBase.OperationRequests.SendClientLog(logString, stackTrace, type, GetExtraSentryData(), GetTags());
		}
	}

	private static bool IsIgnored(string logString)
	{
		return ignoreLogStrings.Contains(logString);
	}

	private static void AddLogToLogContext(string logString, LogType type)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("Time.frameCount", Time.frameCount);
		dictionary.Add("LogType", type.ToString());
		dictionary.Add("Log", logString);
		logContextQueue.Enqueue(dictionary);
		if (logContextQueue.Count > maxLogContextQueueCount)
		{
			logContextQueue.Dequeue();
		}
	}

	private static Dictionary<string, object> GetExtraSentryData()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("Time.frameCount", Time.frameCount);
		dictionary.Add("BrowserInfo", GetBrowserInfo());
		dictionary.Add("GameMode", GetGameMode());
		dictionary.Add("JoinState", GetJoinState());
		dictionary.Add("PlayersCount", GetPlayersCount());
		dictionary.Add("Is tourist session", GetIsTouristSession());
		dictionary.Add("ProfileID", GetProfileID());
		dictionary.Add("PlanetID", GetPlanetID());
		dictionary.Add("RuntimePlatform", Application.platform.ToString());
		dictionary.Add("SystemInfo", GetSystemInfo());
		dictionary.Add("Log Context", GetLogContext());
		return dictionary;
	}

	private static Dictionary<string, string> GetTags()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Version", MVGameControllerBase.VersionNumber.VersionString);
		dictionary.Add("JoinState", MVGameControllerBase.JoinState.ToString());
		dictionary.Add("Source", "standalone");
		return dictionary;
	}

	private static string GetIsTouristSession()
	{
		return MVGameControllerBase.IsTouristSession.ToString();
	}

	private static string GetPlanetID()
	{
		return MVGameControllerBase.GameSessionData.planetID.ToString();
	}

	private static string GetProfileID()
	{
		return MVGameControllerBase.GameSessionData.profileID.ToString();
	}

	private static string GetGameMode()
	{
		return MVGameControllerBase.GameMode.ToString();
	}

	private static string GetJoinState()
	{
		if (MVGameControllerBase.Game == null)
		{
			return "MVGameController.Game is null";
		}
		return MVGameControllerBase.JoinState.ToString();
	}

	private static string GetPlayersCount()
	{
		if (MVGameControllerBase.WOCM == null)
		{
			return "MVGameController.WOCM is null";
		}
		return MVGameControllerBase.Game.Players.Count.ToString();
	}

	private static string GetSystemInfo()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("operatingSystem", SystemInfo.operatingSystem);
		dictionary.Add("processorType", SystemInfo.processorType);
		dictionary.Add("processorCount", SystemInfo.processorCount.ToString());
		dictionary.Add("systemMemorySize", SystemInfo.systemMemorySize.ToString());
		dictionary.Add("graphicsMemorySize", SystemInfo.graphicsMemorySize.ToString());
		dictionary.Add("graphicsDeviceName", SystemInfo.graphicsDeviceName);
		dictionary.Add("graphicsDeviceVendor", SystemInfo.graphicsDeviceVendor);
		dictionary.Add("graphicsDeviceID", SystemInfo.graphicsDeviceID.ToString());
		dictionary.Add("graphicsDeviceVendorID", SystemInfo.graphicsDeviceVendorID.ToString());
		dictionary.Add("graphicsDeviceVersion", SystemInfo.graphicsDeviceVersion);
		dictionary.Add("graphicsShaderLevel", SystemInfo.graphicsShaderLevel.ToString());
		dictionary.Add("supportsShadows", SystemInfo.supportsShadows.ToString());
		dictionary.Add("supportsRenderTextures", SystemInfo.supportsRenderTextures.ToString());
		dictionary.Add("supportsImageEffects", SystemInfo.supportsImageEffects.ToString());
		dictionary.Add("supportedRenderTargetCount", SystemInfo.supportedRenderTargetCount.ToString());
		return GenerateSystemInfoString(dictionary);
	}

	private static string GenerateSystemInfoString(Dictionary<string, string> systemInfo)
	{
		string text = string.Empty;
		foreach (KeyValuePair<string, string> item in systemInfo)
		{
			text += $"{item.Key}: {item.Value}\n";
		}
		return text;
	}

	private static string GetLogContext()
	{
		string text = string.Empty;
		foreach (Dictionary<string, object> item in logContextQueue)
		{
			text += "Log: ";
			foreach (KeyValuePair<string, object> item2 in item)
			{
				text += $"{item2.Key}: {item2.Value} ";
			}
			text += "\n";
		}
		return text;
	}

	private static string GetBrowserInfo()
	{
		return $"{BrowserComm.BrowserName}. version: {BrowserComm.BrowserVersion}";
	}
}
