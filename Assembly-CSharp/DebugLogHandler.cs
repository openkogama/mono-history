using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogHandler
{
	private bool logErrorHasBeenSendOnce;

	private bool showErrorPopupClient;

	private bool enableSentry;

	private Queue<Dictionary<string, object>> logContextQueue = new Queue<Dictionary<string, object>>();

	private int maxLogContextQueueCount = 10;

	private HashSet<string> ignoreLogStrings = new HashSet<string> { "Fullscreen mode can only be enabled in the web player after clicking on the content." };

	public DebugLogHandler(bool showErrorPopupClient, bool enableSentry)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected Obj, but got Unknown
		this.showErrorPopupClient = showErrorPopupClient;
		this.enableSentry = enableSentry;
		Application.RegisterLogCallback((LogCallback)HandleLog);
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
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

	private bool IsIgnored(string logString)
	{
		return ignoreLogStrings.Contains(logString);
	}

	private void AddLogToLogContext(string logString, LogType type)
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

	private Dictionary<string, object> GetExtraSentryData()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
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

	private string GetIsTouristSession()
	{
		if ((Object)(object)MVGameController.Instance == (Object)null)
		{
			return "MVGameController is null";
		}
		return MVGameController.Instance.IsTouristSession.ToString();
	}

	private string GetPlanetID()
	{
		if ((Object)(object)MVGameController.Instance == (Object)null)
		{
			return "MVGameController is null";
		}
		return MVGameController.Instance.PlanetID.ToString();
	}

	private string GetProfileID()
	{
		if ((Object)(object)MVGameController.Instance == (Object)null)
		{
			return "MVGameController is null";
		}
		return MVGameController.Instance.ProfileID.ToString();
	}

	private string GetGameMode()
	{
		if ((Object)(object)MVGameController.Instance == (Object)null)
		{
			return "MVGameController is null";
		}
		return MVGameController.Instance.GameMode.ToString();
	}

	private string GetJoinState()
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

	private string GetPlayersCount()
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

	private string GetSystemInfo()
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

	private string GenerateSystemInfoString(Dictionary<string, string> systemInfo)
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
			text += "Log: ";
			foreach (KeyValuePair<string, object> item2 in item)
			{
				text += $"{item2.Key}: {item2.Value} ";
			}
			text += "\n";
		}
		return text;
	}

	private string GetBrowserInfo()
	{
		return $"{BrowserComm.BrowserName}. version: {BrowserComm.BrowserVersion}";
	}
}
