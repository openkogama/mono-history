using System;
using System.Collections.Generic;
using UnityEngine;

public static class DebugLogHandler
{
	private static bool logErrorHasBeenSendOnce = false;

	private static bool showErrorPopupClient = false;

	private static bool enableSentry = false;

	private static Queue<Dictionary<string, object>> logContextQueue = new Queue<Dictionary<string, object>>();

	private static int maxLogContextQueueCount = 10;

	private static List<Action<string, string, LogType>> logHandlers = new List<Action<string, string, LogType>>();

	private static HashSet<string> ignoreLogStrings = new HashSet<string> { "Fullscreen mode can only be enabled in the web player after clicking on the content." };

	public static void AddLogHandler(Action<string, string, LogType> logHandler)
	{
		logHandlers.Add(logHandler);
	}

	public static void Init()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected Obj, but got Unknown
		Application.RegisterLogCallback((LogCallback)HandleLog);
	}

	public static void Setup(bool showErrorPopupClient, bool enableSentry)
	{
		DebugLogHandler.showErrorPopupClient = showErrorPopupClient;
		DebugLogHandler.enableSentry = enableSentry;
	}

	private static void HandleLog(string logString, string stackTrace, LogType type)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I4
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Invalid comparison between Unknown and I4
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		foreach (Action<string, string, LogType> logHandler in logHandlers)
		{
			logHandler(logString, stackTrace, type);
		}
		if (logErrorHasBeenSendOnce)
		{
			return;
		}
		if ((int)type == 2 || (int)type == 3 || IsIgnored(logString))
		{
			AddLogToLogContext(logString, type);
			return;
		}
		if (showErrorPopupClient)
		{
			UXDialogFactory uXDialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
			uXDialogFactory.BuildDialog(stackTrace, ((Enum)type).ToString(), UXDialogType.Simple, noButtons: false, stackDialog: true).Show();
		}
		if (enableSentry)
		{
			MVGameController.Instance.Game.SendClientLog(logString, stackTrace, type, GetExtraSentryData());
		}
		logErrorHasBeenSendOnce = true;
	}

	private static bool IsIgnored(string logString)
	{
		return ignoreLogStrings.Contains(logString);
	}

	private static void AddLogToLogContext(string logString, LogType type)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("Time.frameCount", Time.frameCount);
		dictionary.Add("LogType", ((Enum)type).ToString());
		dictionary.Add("Log", logString);
		logContextQueue.Enqueue(dictionary);
		if (logContextQueue.Count > maxLogContextQueueCount)
		{
			logContextQueue.Dequeue();
		}
	}

	private static Dictionary<string, object> GetExtraSentryData()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("Time.frameCount", Time.frameCount);
		dictionary.Add("BrowserInfo", GetBrowserInfo());
		dictionary.Add("GameMode", GetGameMode());
		dictionary.Add("JoinState", GetJoinState());
		dictionary.Add("PlayersCount", GetPlayersCount());
		dictionary.Add("Is tourist session", GetIsTouristSession());
		dictionary.Add("ProfileID", GetProfileID());
		dictionary.Add("PlanetID", GetPlanetID());
		dictionary.Add("RuntimePlatform", ((Enum)Application.platform).ToString());
		dictionary.Add("SystemInfo", GetSystemInfo());
		dictionary.Add("Log Context", GetLogContext());
		return dictionary;
	}

	private static string GetIsTouristSession()
	{
		if ((Object)(object)MVGameController.Instance == (Object)null)
		{
			return "MVGameController is null";
		}
		return MVGameController.Instance.IsTouristSession.ToString();
	}

	private static string GetPlanetID()
	{
		if ((Object)(object)MVGameController.Instance == (Object)null)
		{
			return "MVGameController is null";
		}
		return MVGameController.Instance.PlanetID.ToString();
	}

	private static string GetProfileID()
	{
		if ((Object)(object)MVGameController.Instance == (Object)null)
		{
			return "MVGameController is null";
		}
		return MVGameController.Instance.ProfileID.ToString();
	}

	private static string GetGameMode()
	{
		if ((Object)(object)MVGameController.Instance == (Object)null)
		{
			return "MVGameController is null";
		}
		return MVGameController.Instance.GameMode.ToString();
	}

	private static string GetJoinState()
	{
		if ((Object)(object)MVGameController.Instance == (Object)null)
		{
			return "MVGameController is null";
		}
		if (MVGameController.Instance.Game == null)
		{
			return "MVGameController.Instance.Game is null";
		}
		return MVGameController.Instance.Game.JoinState.ToString();
	}

	private static string GetPlayersCount()
	{
		if ((Object)(object)MVGameController.Instance == (Object)null)
		{
			return "MVGameController is null";
		}
		if (MVGameController.Instance.WOCM == null)
		{
			return "MVGameController.Instance.WOCM is null";
		}
		return MVGameController.Instance.Game.Players.Count.ToString();
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
		dictionary.Add("graphicsPixelFillrate", SystemInfo.graphicsPixelFillrate.ToString());
		dictionary.Add("supportsShadows", SystemInfo.supportsShadows.ToString());
		dictionary.Add("supportsRenderTextures", SystemInfo.supportsRenderTextures.ToString());
		dictionary.Add("supportsImageEffects", SystemInfo.supportsImageEffects.ToString());
		dictionary.Add("supportedRenderTargetCount", SystemInfo.supportedRenderTargetCount.ToString());
		dictionary.Add("supportsVertexPrograms", SystemInfo.supportsVertexPrograms.ToString());
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
