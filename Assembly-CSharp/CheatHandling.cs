using CodeStage.AntiCheat.Detectors;
using MV.Common;

public static class CheatHandling
{
	public static void Init()
	{
		SpeedHackDetector.StartDetection(SpeedHackDetected, 1f, 3);
		ObscuredCheatingDetector.StartDetection(ObscuredCheatingDetected);
	}

	public static void TextureHackDetected()
	{
		ExecuteBan(CheatType.TextureTampering);
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
