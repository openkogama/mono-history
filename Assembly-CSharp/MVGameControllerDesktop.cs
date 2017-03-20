using MV.Common;
using UnityEngine;
using UnityEngine.Events;

public class MVGameControllerDesktop : MVGameControllerBase
{
	[SerializeField]
	private GameObject eventSystem;

	private static ModeControllerBase modeController;

	private static ILockCursorManager lockCursorManager;

	private static IEditModeObjectPicker editModeObjectPicker;

	private static bool applicationHasFocus = true;

	public static ILockCursorManager LockCursorManager => lockCursorManager;

	public static UnityAction OnApplicationLostFocus { get; set; }

	public static UnityAction OnApplicationRegainedFocus { get; set; }

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
		bool developmentMode = Application.isEditor || koGaMaSettings.ShowDebugLogin;
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
		BackButtonManager.Update();
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		FullScreenController.LateUpdate();
	}

	public override void HandleApplicationQuit(QuitBaseCallback quitBaseCallback)
	{
		quitBaseCallback?.OnQuit();
		Application.Quit();
	}

	protected void OnApplicationFocus(bool focus)
	{
		if (applicationHasFocus != focus)
		{
			applicationHasFocus = focus;
			if (focus && OnApplicationRegainedFocus != null)
			{
				OnApplicationRegainedFocus();
			}
			else if (OnApplicationLostFocus != null)
			{
				OnApplicationLostFocus();
			}
		}
	}
}
