using System;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DesktopPlayModeController : ModeControllerBase, IPlayModeUI, IActivateUIElement, ILeaveEditPlayModeHandler, ICanvasController, IAccessoryPopupHandler, IEventSystemHandler
{
	private ILockCursorManager lockCursorManager;

	[SerializeField]
	private UIStack uiStack;

	[SerializeField]
	private TeamMenu teamMenu;

	[SerializeField]
	private DesktopInGameGUIController inGameController;

	[SerializeField]
	private RectTransform lobbyState;

	[SerializeField]
	private RectTransform inGameMenu;

	[SerializeField]
	private RectTransform playerListButton;

	[SerializeField]
	private RectTransform notificationsManager;

	[SerializeField]
	private GameObject stackBottom;

	[SerializeField]
	private ChatControllerUGUI chatController;

	[SerializeField]
	private AccessoryShopController accessoryShopController;

	[SerializeField]
	private LevelBadge levelBadge;

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private TouristAdController touristAdController;

	[SerializeField]
	private TouristModeController touristModeController;

	[SerializeField]
	private GameObject fullscreenPlayModeStateTransform;

	[SerializeField]
	private WinningConditionBriefing winningConditionBriefingMenu;

	[SerializeField]
	private LobbyStatePlayModeController lobbyStatePlayModeController;

	[SerializeField]
	private ChatBubbleController chatBubbleController;

	[SerializeField]
	private TimeAttackFlagDebriefing timeAttackFlagDebriefing;

	private RectTransform lobbyStateRect;

	private RectTransform inGameMenuRect;

	public UnityAction OnLeaveEditPlayMode;

	private GameObject playModeState;

	public ILockCursorManager LockCursorManager => lockCursorManager;

	public bool InLobbyState
	{
		get
		{
			return lobbyStatePlayModeController.IsInLobbyState;
		}
		set
		{
			lobbyStatePlayModeController.IsInLobbyState = value;
		}
	}

	private void Awake()
	{
		uiStack.Push(stackBottom, UIPushOption.None, null, UIGroupFlags.StackBottom);
		CreateGUI();
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			lockCursorManager = gameObject.AddComponent<LockCursorManager2DMode>();
		}
		else
		{
			lockCursorManager = gameObject.AddComponent<LockCursorManager3DMode>();
		}
		MVGameControllerDesktop.RegisterPlayModeController(this);
		if (MVGameControllerBase.Game.LocalPlayer.IsReady)
		{
			SetUIReady();
		}
		else
		{
			MVGameControllerBase.OnFirstFrameUpdateActorReady = (Action)Delegate.Combine(MVGameControllerBase.OnFirstFrameUpdateActorReady, new Action(SetUIReady));
		}
	}

	private void Start()
	{
		RegisterHotkeys();
	}

	private void Update()
	{
		HandleFpsShortcut();
		HandleInput();
	}

	protected void OnDestroy()
	{
		MVGameControllerDesktop.UnregisterPlayModeController();
	}

	private void HandleInput()
	{
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.LobbyMenu))
		{
			lockCursorManager.CursorLock = false;
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.ToggleHD) && uiStack.IsStackEmpty())
		{
			ToggleHD();
		}
		if (MVGameControllerBase.EditModeUI != null && MVInputWrapper.GetBooleanControlDown(KogamaControls.ToggleLogicRendering))
		{
			ToggleLogicVisibility();
		}
	}

	private void RegisterHotkeys()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IShortcutKeyRegister x, BaseEventData y) =>
		{
			x.RegisterShortcutKey(KogamaControls.Respawn, KeyState.Down, Respawn);
		});
	}

	private void Respawn()
	{
		if (MVGameControllerBase.WOCM.AvatarLocal != null)
		{
			MVGameControllerBase.WOCM.AvatarLocal.Respawn();
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		if (MVGameControllerBase.EditModeUI == null)
		{
			MVGameControllerBase.CameraController.AvatarLobbyFocus = true;
		}
		chatBubbleController = UnityEngine.Object.Instantiate(chatBubbleController);
		chatBubbleController.transform.SetParent(transform, worldPositionStays: false);
		lobbyStateRect = UnityEngine.Object.Instantiate(lobbyState);
		lobbyStateRect.SetParent(playModeState.transform, worldPositionStays: false);
		inGameMenuRect = UnityEngine.Object.Instantiate(inGameMenu);
		inGameMenuRect.SetParent(playModeState.transform, worldPositionStays: false);
		timeAttackFlagDebriefing = UnityEngine.Object.Instantiate(timeAttackFlagDebriefing);
		timeAttackFlagDebriefing.transform.SetParent(playModeState.transform, worldPositionStays: false);
		FlagDebriefingControl.OnFlagDebriefing = (Action<int>)Delegate.Combine(FlagDebriefingControl.OnFlagDebriefing, new Action<int>(OnShowTimeAttackFlagDebriefing));
		FlagDebriefingControl.OnFlagCountDown = (Action)Delegate.Combine(FlagDebriefingControl.OnFlagCountDown, new Action(OnShowTimeAttackFlagCountDown));
		FlagDebriefingControl.OnFlagCountDownEnd = (Action)Delegate.Combine(FlagDebriefingControl.OnFlagCountDownEnd, new Action(OnHideTimeAttackFlagCountDown));
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnRoundEnd));
		if (MVGameControllerBase.Game.GameType == MVGameType.Classic)
		{
			MVInputWrapper.SetInputMap(new DesktopPlayMode());
		}
		else if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			MVInputWrapper.SetInputMap(new Desktop2DPlayMode());
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
		lobbyStatePlayModeController.Initialize(inGameController, lobbyStateRect, inGameMenuRect, chatController);
		if (MVGameControllerBase.GameMode == MVGameMode.Play)
		{
			InLobbyState = true;
		}
		chatController.Initialize();
		playerListButton.gameObject.SetActive(value: true);
	}

	private void ToggleLogicVisibility()
	{
		MVGameControllerBase.CameraController.IsLogicRendered = !MVGameControllerBase.CameraController.IsLogicRendered;
	}

	private void ToggleHD()
	{
		int currentLevel = ((MVQualitySettings.CurrentLevel == 0) ? 1 : 0);
		MVQualitySettings.CurrentLevel = currentLevel;
	}

	private void CreateGUI()
	{
		chatController = UnityEngine.Object.Instantiate(chatController);
		chatController.transform.SetParent(stackBottom.transform, worldPositionStays: false);
		chatController.SubscribeToMessages();
		playModeState = UnityEngine.Object.Instantiate(fullscreenPlayModeStateTransform);
		uiStack.Push(playModeState, UIPushOption.None, null, UIGroupFlags.MainUI);
		inGameController = UnityEngine.Object.Instantiate(inGameController);
		inGameController.Initialize();
		inGameController.transform.SetParent(playModeState.transform, worldPositionStays: false);
		playerListButton = UnityEngine.Object.Instantiate(playerListButton);
		playerListButton.transform.SetParent(playModeState.transform, worldPositionStays: false);
		levelBadge = UnityEngine.Object.Instantiate(levelBadge);
		levelBadge.transform.SetParent(playModeState.transform, worldPositionStays: false);
		notificationsManager = UnityEngine.Object.Instantiate(notificationsManager);
		notificationsManager.transform.SetParent(stackBottom.transform, worldPositionStays: false);
		if (MVGameControllerBase.IsTouristSession && !MVGameControllerBase.GameSessionData.IsPlayedFromPoki)
		{
			touristAdController = UnityEngine.Object.Instantiate(touristAdController);
			touristAdController.transform.SetParent(playModeState.transform, worldPositionStays: false);
			touristAdController.Initialize(touristModeController);
		}
	}

	private void OnShowTimeAttackFlagDebriefing(int captureTime)
	{
		timeAttackFlagDebriefing.gameObject.SetActive(value: true);
		timeAttackFlagDebriefing.Initialize(captureTime);
		lobbyStateRect.gameObject.SetActive(value: false);
		inGameMenuRect.gameObject.SetActive(value: false);
		playerListButton.gameObject.SetActive(value: false);
	}

	private void OnShowTimeAttackFlagCountDown()
	{
		timeAttackFlagDebriefing.InitializeCountDown();
		timeAttackFlagDebriefing.gameObject.SetActive(value: true);
		lobbyStateRect.gameObject.SetActive(value: false);
		inGameMenuRect.gameObject.SetActive(value: false);
		playerListButton.gameObject.SetActive(value: false);
	}

	private void OnHideTimeAttackFlagCountDown()
	{
		timeAttackFlagDebriefing.gameObject.SetActive(value: false);
		playerListButton.gameObject.SetActive(value: true);
	}

	private void OnRoundEnd(IWinningCondition winningCondition)
	{
		if (timeAttackFlagDebriefing.isActiveAndEnabled)
		{
			timeAttackFlagDebriefing.OnRoundEnd();
			timeAttackFlagDebriefing.gameObject.SetActive(value: false);
			lobbyStateRect.gameObject.SetActive(value: true);
			inGameMenuRect.gameObject.SetActive(value: true);
		}
	}

	private void LeavePlayMode()
	{
		timeAttackFlagDebriefing.OnLeavePlayMode();
	}

	public void ShowEUseIcon(ShowUseOption option, int woID = 0)
	{
		inGameController.ShowEUseIcon(option, woID);
	}

	public void HideEUseIcon()
	{
		inGameController.HideEUseIcon();
	}

	public IGUICrossHair GetCrossHair()
	{
		return inGameController.GetCrossHair();
	}

	public void Activate(ActivateUIElement element)
	{
		switch (element)
		{
		case ActivateUIElement.AvatarAccessoryShop:
			accessoryShopController.Activate(UIPushOption.HideAll);
			break;
		case ActivateUIElement.AvatarAccessoryShopBundles:
			accessoryShopController.Activate(UIPushOption.HideAll, AccessoryCategoryClient.Bundles);
			break;
		}
	}

	public void LeaveEditPlayMode()
	{
		LeavePlayMode();
		if (OnLeaveEditPlayMode != null)
		{
			OnLeaveEditPlayMode();
		}
	}

	public void SetPixelPerfect(bool pixelPerfect)
	{
		canvas.pixelPerfect = pixelPerfect;
	}

	public void SetUIReady()
	{
		uiStack.SetStackReady();
		MVGameControllerBase.OnFirstFrameUpdateActorReady = (Action)Delegate.Remove(MVGameControllerBase.OnFirstFrameUpdateActorReady, new Action(SetUIReady));
	}

	public void OpenInventoryAtItem(UIPushOption pushOption, AccessoryDataClient displayShopItems)
	{
		accessoryShopController.OpenInventoryAtItem(pushOption, displayShopItems);
	}
}
