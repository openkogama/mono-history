using MV.Common;
using UnityEngine;

public class MVGameControllerDesktop : MVGameControllerBase
{
	private static ModeControllerBase modeController;

	private static ILockCursorManager lockCursorManager;

	private static IEditModeObjectPicker editModeObjectPicker;

	[SerializeField]
	private GameObject eventSystem;

	public static ILockCursorManager LockCursorManager => lockCursorManager;

	protected override bool IsPlayingInternal
	{
		get
		{
			if (MVGameControllerBase.JoinState != MVJoinState.Playing)
			{
				return false;
			}
			return MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Play || (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit && MVGameControllerBase.IEditModeUI.IsInPlayInEditMode);
		}
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(eventSystem);
		MVGameControllerBase.customBuildSettings = Resources.Load("Prefabs/CustomBuildSettings", typeof(CustomBuildSettings)) as CustomBuildSettings;
		bool developmentMode = Application.isEditor || MVGameControllerBase.customBuildSettings.ShowLogin;
		InitStandAlone(developmentMode);
		FullScreenController.FullScreen = false;
		Screen.SetResolution(940, 482, fullscreen: false);
		FullScreenController.Init(940, 482);
	}

	public static void RegisterPlayModeController(DesktopPlayModeController playModeController)
	{
		lockCursorManager = playModeController.LockCursorManager;
		MVGameControllerBase.playModeUI = playModeController;
		if (MVGameControllerBase.GameMode == MVGameMode.Play)
		{
			modeController = playModeController;
		}
		else
		{
			((DesktopEditModeController)modeController).RegisterPlayModeController(playModeController);
		}
	}

	public static void RegisterAvaterEditModeController(DesktopAvatarEditModeController avatarEditModeController)
	{
		modeController = avatarEditModeController;
	}

	public static void RegisterEditModeController(DesktopEditModeController editModeController)
	{
		modeController = editModeController;
		MVGameControllerBase.editModeUI = editModeController;
	}

	protected override void UpdateInternal()
	{
		if (!MVGameControllerBase.isInitialized)
		{
			Initialize();
			modeController.Initialize();
			LevelingManager.Initialize(MVGameControllerBase.Game.LocalPlayer.ProfileID);
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		FullScreenController.LateUpdate();
	}
}
