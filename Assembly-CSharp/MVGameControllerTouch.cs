using MV.Common;

public class MVGameControllerTouch : MVGameControllerBase
{
	private static ModeControllerBase modeController;

	protected override bool IsPlayingInternal
	{
		get
		{
			if (MVGameControllerBase.JoinState != MVJoinState.Playing)
			{
				return false;
			}
			return MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Play;
		}
	}

	protected override void CreateInGameController()
	{
	}

	public static void RegisterPlayModeController(PlayModeController playModeController)
	{
		modeController = playModeController;
		MVGameControllerBase.playModeUI = playModeController;
	}

	protected override void UpdateInternal()
	{
		if (!MVGameControllerBase.isInitialized)
		{
			Initialize();
			modeController.Initialize();
		}
	}
}
