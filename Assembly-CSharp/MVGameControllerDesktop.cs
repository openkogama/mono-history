using Assets.Scripts.AdIntegration;
using Assets.Scripts.AdIntegration.Dummy;
using MV.Common;
using UnityEngine;
using UnityEngine.Events;

public class MVGameControllerDesktop : MVGameControllerBase
{
	[SerializeField]
	private GameObject eventSystem;

	private ModeControllerBase modeController;

	private IEditModeObjectPicker editModeObjectPicker;

	private bool applicationHasFocus = true;

	private ILockCursorManager lockCursorManager;

	private IAdManager adManager = new DummyAdManager();

	private static MVGameControllerDesktop Instance => (MVGameControllerDesktop)MVGameControllerBase.instance;

	public static UnityAction OnApplicationLostFocus { get; set; }

	public static UnityAction OnApplicationRegainedFocus { get; set; }

	public static ILockCursorManager LockCursorManager => Instance.lockCursorManager;

	protected override IAdManager GetAdManager => adManager;

	protected override bool IsPlayingInternal
	{
		get
		{
			if (MVGameControllerBase.JoinState != MVJoinState.Playing)
			{
				return false;
			}
			return MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Play || (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit && MVGameControllerBase.EditModeUI.IsInPlayInEditMode);
		}
	}

	protected void Start()
	{
		bool developmentMode = Application.isEditor || koGaMaSettings.ShowDebugLogin;
		InitStandAlone(developmentMode);
		FullScreenController.FullScreen = false;
		Screen.SetResolution(940, 482, fullscreen: false);
		FullScreenController.Init(940, 482);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		OnApplicationLostFocus = null;
		OnApplicationRegainedFocus = null;
	}

	public static void RegisterPlayModeController(DesktopPlayModeController playModeController)
	{
		Instance.lockCursorManager = playModeController.LockCursorManager;
		MVGameControllerBase.PlayModeUI = playModeController;
		if (MVGameControllerBase.GameMode == MVGameMode.Play)
		{
			Instance.modeController = playModeController;
		}
		else
		{
			((DesktopEditModeController)Instance.modeController).RegisterPlayModeController(playModeController);
		}
	}

	public static void UnregisterPlayModeController()
	{
		MVGameControllerBase.PlayModeUI = null;
	}

	public static void RegisterAvaterEditModeController(DesktopAvatarEditModeController avatarEditModeController)
	{
		Instance.modeController = avatarEditModeController;
	}

	public static void RegisterEditModeController(DesktopEditModeController editModeController)
	{
		Instance.modeController = editModeController;
		MVGameControllerBase.EditModeUI = editModeController;
	}

	public static void UnregisterEditModeController()
	{
		if (MVGameControllerBase.IsAlive)
		{
			Instance.modeController = null;
			MVGameControllerBase.EditModeUI = null;
		}
	}

	protected override void UpdateInternal()
	{
		if (!MVGameControllerBase.IsInitialized)
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

	protected override void HandleApplicationQuit(QuitBaseCallback quitBaseCallback)
	{
		quitBaseCallback?.OnQuit();
		Application.Quit();
	}

	protected override void CleanUp()
	{
		MVGameControllerBase.DeleteScreenPlayerPrefs();
		base.CleanUp();
	}
}
