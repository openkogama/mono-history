using System;
using System.Collections.Generic;
using UnityEngine;

public static class DebugLogHandler
{
	private static bool logErrorHasBeenSendOnce = false;

	private static Queue<Dictionary<string, object>> logContextQueue = new Queue<Dictionary<string, object>>();

	private static int maxLogContextQueueCount = 15;

	private static List<Action<string, string, LogType>> logHandlers = new List<Action<string, string, LogType>>();

	private static int sampleErrorFrequency = 1000;

	private static HashSet<string> ignoreLogStrings = new HashSet<string> { "Fullscreen mode can only be enabled in the web player after clicking on the content." };

	public static void AddLogHandler(Action<string, string, LogType> logHandler)
	{
		logHandlers.Add(logHandler);
	}

	public static void Init()
	{
		Application.logMessageReceived += HandleLog;
	}

	private static void HandleLog(string logString, string stackTrace, LogType type)
	{
		foreach (Action<string, string, LogType> logHandler in logHandlers)
		{
			logHandler(logString, stackTrace, type);
		}
		if (logErrorHasBeenSendOnce)
		{
			return;
		}
		if (type == LogType.Warning || type == LogType.Log || IsIgnored(logString))
		{
			AddLogToLogContext(logString, type);
			return;
		}
		if (MVClientSettings.IsDebugMode)
		{
			try
			{
				UXDialogFactory uXDialogFactory = UXUtils.UXDialogFactory;
				uXDialogFactory.BuildDialog(stackTrace, type.ToString(), UXDialogType.Simple, noButtons: false, stackDialog: true).Show();
			}
			catch
			{
			}
		}
		bool flag = UnityEngine.Random.Range(0, sampleErrorFrequency + 1) == sampleErrorFrequency;
		if (MVClientSettings.EnableSentry || flag)
		{
			MVGameController.Game.SendClientLog(logString, stackTrace, type, GetExtraSentryData(), GetTags());
		}
		logErrorHasBeenSendOnce = true;
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
		dictionary.Add("Version", MVGameController.VersionNumber.VersionString);
		dictionary.Add("Source", "standalone");
		return dictionary;
	}

	private static string GetIsTouristSession()
	{
		return MVGameController.Game.IsTouristSession.ToString();
	}

	private static string GetPlanetID()
	{
		return MVGameController.GameSessionData.planetID.ToString();
	}

	private static string GetProfileID()
	{
		return MVGameController.GameSessionData.profileID.ToString();
	}

	private static string GetGameMode()
	{
		return MVGameController.GameMode.ToString();
	}

	private static string GetJoinState()
	{
		if (MVGameController.Game == null)
		{
			return "MVGameController.Game is null";
		}
		return MVGameController.Game.JoinState.ToString();
	}

	private static string GetPlayersCount()
	{
		if (MVGameController.WOCM == null)
		{
			return "MVGameController.WOCM is null";
		}
		return MVGameController.Game.Players.Count.ToString();
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
