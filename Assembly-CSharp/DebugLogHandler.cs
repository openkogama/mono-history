using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class DebugLogHandler : MonoBehaviour
{
	private class StatHatErrorCount
	{
		private bool reportedError;

		private bool reportedOngoingError;

		public void Increment(bool errorDetected, bool onGoingErrorDetected)
		{
			try
			{
				if (errorDetected == onGoingErrorDetected)
				{
					Debug.LogWarning("An errorDetected and onGoingErrorDetected cannot have the same value: " + errorDetected);
					return;
				}
				if (onGoingErrorDetected)
				{
					IncrementErrorCountOnGoing();
				}
				if (errorDetected)
				{
					IncrementErrorCount();
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		private void IncrementErrorCountOnGoing()
		{
			if (!reportedOngoingError)
			{
				StatHatWrapper.Count("errorcountongoing", 1);
				reportedOngoingError = true;
			}
		}

		private void IncrementErrorCount()
		{
			if (!reportedError)
			{
				StatHatWrapper.Count("errorcount", 1);
				reportedError = true;
			}
		}
	}

	private const int maxErrorBeforeReport = 50;

	private const int sampleErrorFrequency = 100;

	private static List<string> sanitizeLogSubstrings = new List<string> { "Could not allocate memory: System out of memory!", "Failed to update dynamic font", "Screen position out of view frustum" };

	private static HashSet<string> ignoreLogStrings = new HashSet<string> { "Fullscreen mode can only be enabled in the web player after clicking on the content." };

	private int maxLogContextQueueCount = 4;

	private ProxyLogHandler kogamaLogHandler;

	private StatHatErrorCount statHatErrorCount = new StatHatErrorCount();

	[SerializeField]
	private SentrySdk sentrySdk;

	private int timeFrameCount;

	private int errorCount;

	private bool logErrorHasBeenSendOnce;

	private Queue<Dictionary<string, object>> logContextQueue = new Queue<Dictionary<string, object>>();

	private string sanitizedString;

	private bool isSampling;

	private bool isInBrokenState;

	private static bool didConnectToGameServer = false;

	private static string firstError = string.Empty;

	public static bool DidConnectToGameServer
	{
		get
		{
			return didConnectToGameServer;
		}
		set
		{
			didConnectToGameServer = value;
		}
	}

	private bool SendOnGoingError => 50 == errorCount;

	private bool AlwaysSampling => true;

	public void Initialize(DebugLogHandlerConfig debugLogHandlerConfig, SentryConfig sentryConfig)
	{
		maxLogContextQueueCount = debugLogHandlerConfig.maxLogContextQueueCount;
		if (debugLogHandlerConfig.useProxyLogHandler)
		{
			kogamaLogHandler = new ProxyLogHandler();
			kogamaLogHandler.OnLogReceived += KogamaLogHandlerOnOnLogReceived;
			kogamaLogHandler.filterLogTypeConsoleWrite = debugLogHandlerConfig.proxyLogHandlerConfig.filterLogTypeConsoleWrite;
		}
		isSampling = UnityEngine.Random.Range(0, 101) == 100 || AlwaysSampling || !debugLogHandlerConfig.useSamplingOnAndroidAndWebGL;
		Application.logMessageReceived += HandleLog;
		sentrySdk.Initialize(sentryConfig);
	}

	private void KogamaLogHandlerOnOnLogReceived(object sender, ProxyLogHandler.LogFormatData e)
	{
		kogamaLogHandler.OnLogReceived -= KogamaLogHandlerOnOnLogReceived;
		AddLogToLogContext(e.Message, e.LogType);
		kogamaLogHandler.OnLogReceived += KogamaLogHandlerOnOnLogReceived;
	}

	public void Destroy()
	{
		errorCount = 0;
		logErrorHasBeenSendOnce = false;
		logContextQueue.Clear();
		isSampling = false;
		if (isInBrokenState)
		{
			isInBrokenState = false;
			return;
		}
		try
		{
			if (kogamaLogHandler != null)
			{
				kogamaLogHandler.Disable();
				kogamaLogHandler.OnLogReceived -= KogamaLogHandlerOnOnLogReceived;
				kogamaLogHandler = null;
			}
			Application.logMessageReceived -= HandleLog;
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		Application.logMessageReceived -= HandleLog;
		try
		{
			HandleLogExecute(logString, stackTrace, type);
		}
		catch (Exception exception)
		{
			isInBrokenState = true;
			Debug.LogException(exception);
			return;
		}
		Application.logMessageReceived += HandleLog;
	}

	private static string CleanStackTrace(string stackTrace)
	{
		string[] separator = new string[1] { "UnityEngine.Debug:LogError(Object)\n" };
		string[] array = stackTrace.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length != 2)
		{
			return stackTrace;
		}
		return array[1];
	}

	private void HandleLogExecute(string logString, string stackTrace, LogType type)
	{
		if (AddLogToLogContext(logString, type))
		{
			return;
		}
		if (type == LogType.Error)
		{
			stackTrace = CleanStackTrace(stackTrace);
		}
		errorCount++;
		bool errorDetected = false;
		if (!logErrorHasBeenSendOnce)
		{
			firstError = logString;
			errorDetected = true;
		}
		if (!logErrorHasBeenSendOnce || SendOnGoingError)
		{
			logErrorHasBeenSendOnce = true;
			bool onGoingErrorDetected = false;
			if (SendOnGoingError)
			{
				logString = "[Ongoing error] " + logString;
				onGoingErrorDetected = true;
			}
			SendToConsole(logString, stackTrace);
			ReportError(logString, stackTrace, type);
			statHatErrorCount.Increment(errorDetected, onGoingErrorDetected);
		}
	}

	private void ReportError(string logString, string stackTrace, LogType type)
	{
		if (!DidConnectToGameServer)
		{
			sentrySdk.OnLogMessageReceived(logString, stackTrace, type, GetExtraSentryData(), GetTags());
		}
		else if (isSampling)
		{
			logString = SanitizeLogStringForUniqueErrors(logString);
			Dictionary<string, object> extraSentryData = GetExtraSentryData();
			Dictionary<string, string> tags = GetTags();
			MVGameControllerBase.OperationRequests.SendClientLog(logString, stackTrace, type, extraSentryData, tags);
		}
	}

	private string SanitizeLogStringForUniqueErrors(string logString)
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
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private static bool IsIgnored(string logString)
	{
		return ignoreLogStrings.Contains(logString);
	}

	private bool AddLogToLogContext(string logString, LogType type)
	{
		if ((!IsIgnored(logString) || type != LogType.Error) && type != LogType.Warning && type != LogType.Log)
		{
			return false;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("Frame", timeFrameCount);
		dictionary.Add(type.ToString(), logString);
		logContextQueue.Enqueue(dictionary);
		if (logContextQueue.Count > maxLogContextQueueCount)
		{
			logContextQueue.Dequeue();
		}
		return true;
	}

	private void Update()
	{
		timeFrameCount = Time.frameCount;
	}

	private Dictionary<string, object> GetExtraSentryData()
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
		try
		{
			dictionary.Add("Version", MVGameControllerBase.KoGaMaSettings.VersionString);
			dictionary.Add("ReleaseName", MVGameControllerBase.KoGaMaSettings.ReleaseName);
			dictionary.Add("JoinState", MVGameControllerBase.JoinState.ToString());
			dictionary.Add("Source", "standalone");
		}
		catch
		{
			dictionary.Add("TagsN/A", "GetTagsException");
		}
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
		catch
		{
			return "N/A";
		}
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

	private string GetLogContext()
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
