using System.Diagnostics;
using CodeStage.AntiCheat.Detectors;
using MV.Common;
using UnityEngine;

public static class CheatHandling
{
	public static void Init()
	{
		SpeedHackDetector.StartDetection(SpeedHackDetected, 1f, 3);
		ObscuredCheatingDetector.StartDetection(ObscuredCheatingDetected);
	}

	[Conditional("UNITY_STANDALONE_WIN")]
	public static void MachineBanDetected()
	{
		MVGameControllerBase.ApplicationQuit(null);
	}

	public static void CheatSoftwareRunningDetected()
	{
		ExecuteBan(CheatType.CheatSoftwareRunning);
	}

	public static void TextureHackDetected()
	{
		ExecuteBan(CheatType.TextureTampering);
	}

	public static void SuspectedHackDetected(string msg)
	{
		DebugLogHandler.ReportError(msg, string.Empty, LogType.Error);
	}

	private static void SpeedHackDetected()
	{
		ExecuteBan(CheatType.SpeedHack);
	}

	private static void ObscuredCheatingDetected()
	{
		ExecuteBan(CheatType.MemTampering);
	}

	private static void ExecuteBan(CheatType cheatType)
	{
		MVGameControllerBase.OperationRequests.Ban(cheatType);
		MVGameControllerBase.ApplicationQuit(null);
		BrowserComm.ToJavaScript.ExternalCall("gotoSignout", null, null);
	}
}
