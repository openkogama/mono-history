using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public static class DebugLogHandler
{
	private const int maxErrorBeforeReport = 50;

	private static int errorCount = 0;

	private static bool logErrorHasBeenSendOnce = false;

	private static Queue<Dictionary<string, object>> logContextQueue = new Queue<Dictionary<string, object>>();

	private static string sanitizedString;

	private const int maxLogContextQueueCount = 4;

	private const int sampleErrorFrequency = 100;

	private static bool isSampling = false;

	private static List<string> sanitizeLogSubstrings = new List<string> { "Could not allocate memory: System out of memory!", "Failed to update dynamic font", "Screen position out of view frustum" };

	private static string firstError = string.Empty;

	private static HashSet<string> ignoreLogStrings = new HashSet<string> { "Fullscreen mode can only be enabled in the web player after clicking on the content." };

	public static bool DidConnectToGameServer { get; set; }

	public static bool ErrorDetected { get; private set; }

	public static bool OngoingErrorDetected { get; private set; }

	private static bool SendOnGoingError => 50 == errorCount;

	public static bool IsSampling => true;

	public static void SetupSentryClient(string sentryUrl)
	{
	}

	public static void Init()
	{
		isSampling = UnityEngine.Random.Range(0, 101) == 100;
		Application.logMessageReceived += HandleLog;
	}

	public static void Reset()
	{
		errorCount = 0;
		logErrorHasBeenSendOnce = false;
		logContextQueue.Clear();
		isSampling = false;
		ErrorDetected = false;
		OngoingErrorDetected = false;
		try
		{
			Application.logMessageReceived -= HandleLog;
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
	}

	public static void ForceExtraErrorReport()
	{
		logErrorHasBeenSendOnce = false;
	}

	private static void HandleLog(string logString, string stackTrace, LogType type)
	{
		try
		{
			try
			{
				if (type == LogType.Warning || type == LogType.Log || IsIgnored(logString))
				{
					AddLogToLogContext(logString, type);
					return;
				}
			}
			catch (Exception)
			{
			}
			errorCount++;
			if (!logErrorHasBeenSendOnce)
			{
				firstError = logString;
				ErrorDetected = true;
			}
			if (!logErrorHasBeenSendOnce || SendOnGoingError)
			{
				logErrorHasBeenSendOnce = true;
				if (SendOnGoingError)
				{
					logString = "[Ongoing error] " + logString;
					OngoingErrorDetected = true;
				}
				SendToConsole(logString, stackTrace);
				ReportError(logString, stackTrace, type);
			}
		}
		catch (Exception ex2)
		{
			Debug.LogWarningFormat("Exception in DebugLogHandler: {0}.", ex2.Message);
		}
	}

	private static void ReportError(string logString, string stackTrace, LogType type)
	{
		if (!DidConnectToGameServer)
		{
			SentrySdk.OnLogMessageReceived(logString, stackTrace, type, GetExtraSentryData(), GetTags());
		}
		else if (MVClientSettings.EnableSentry || isSampling)
		{
			logString = SanitizeLogStringForUniqueErrors(logString);
			Dictionary<string, object> extraSentryData = GetExtraSentryData();
			Dictionary<string, string> tags = GetTags();
			MVGameControllerBase.OperationRequests.SendClientLog(logString, stackTrace, type, extraSentryData, tags);
		}
	}

	private static string SanitizeLogStringForUniqueErrors(string logString)
	{
		for (int i = 0; i < sanitizeLogSubstrings.Count; i++)
		{
			string text = sanitizeLogSubstrings[i];
			if (logString.Contains(text))
			{
				sanitizedString = logString.Remove(logString.IndexOf(text), text.Length);
				logString = text;
				break;
			}
		}
		return logString;
	}

	private static void SendToConsole(string logString, string stackTrace)
	{
		try
		{
			string text = logString + ": " + stackTrace;
			if (text.Length > 1024)
			{
				text = text.Substring(0, 1024);
			}
			MVGameControllerBase.PostGameMsg(MVGameMsgType.Warning, text);
		}
		catch
		{
		}
	}

	private static bool IsIgnored(string logString)
	{
		return ignoreLogStrings.Contains(logString);
	}

	private static void AddLogToLogContext(string logString, LogType type)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("Frame", Time.frameCount);
		dictionary.Add(type.ToString(), logString);
		logContextQueue.Enqueue(dictionary);
		if (logContextQueue.Count > 4)
		{
			logContextQueue.Dequeue();
		}
	}

	private static Dictionary<string, object> GetExtraSentryData()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("Time.frameCount", Time.frameCount);
		dictionary.Add("BrowserInfo", TryGetExtraString(GetBrowserInfo));
		dictionary.Add("GameMode", TryGetExtraString(GetGameMode));
		dictionary.Add("JoinState", TryGetExtraString(GetJoinState));
		dictionary.Add("PlayersCount", TryGetExtraString(GetPlayersCount));
		dictionary.Add("PendingPlayersCount", TryGetExtraString(GetPendingPlayersCount));
		dictionary.Add("Is tourist session", TryGetExtraString(GetIsTouristSession));
		dictionary.Add("ProfileID", TryGetExtraString(GetProfileID));
		dictionary.Add("PlanetID", TryGetExtraString(GetPlanetID));
		dictionary.Add("RuntimePlatform", Application.platform.ToString());
		dictionary.Add("SystemInfo", TryGetExtraString(GetSystemInfo));
		dictionary.Add("Log Context", TryGetExtraString(GetLogContext));
		if (!string.IsNullOrEmpty(sanitizedString))
		{
			sanitizedString.Trim();
			dictionary.Add("Sanitized Error Data", sanitizedString);
		}
		if (SendOnGoingError)
		{
			dictionary.Add("First Error", firstError);
		}
		return dictionary;
	}

	private static Dictionary<string, string> GetTags()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Version", MVGameControllerBase.KoGaMaSettings.VersionString);
		dictionary.Add("ReleaseName", MVGameControllerBase.KoGaMaSettings.ReleaseName);
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
		return MVGameControllerBase.Game.MVPlayerContainer.Count.ToString();
	}

	private static string GetPendingPlayersCount()
	{
		if (MVGameControllerBase.WOCM == null)
		{
			return "MVGameController.WOCM is null";
		}
		return MVGameControllerBase.Game.MVPlayerContainer.PendingPlayersCount.ToString();
	}

	private static string TryGetExtraString(Func<string> getFunc)
	{
		try
		{
			return getFunc();
		}
		catch (Exception)
		{
		}
		return "N/A";
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
		dictionary.Add("supportsImageEffects", SystemInfo.supportsImageEffects.ToString());
		dictionary.Add("supportedRenderTargetCount", SystemInfo.supportedRenderTargetCount.ToString());
		if (SystemInfo.graphicsDeviceVendor == "Vivante Corporation")
		{
			string text = string.Empty;
			foreach (RenderTextureFormat value in Enum.GetValues(typeof(RenderTextureFormat)))
			{
				if (SystemInfo.SupportsRenderTextureFormat(value))
				{
					text = ((!string.IsNullOrEmpty(text)) ? (text + " " + value) : (text + value));
				}
			}
			dictionary.Add("supportedRenderTextureFormats", text);
		}
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
