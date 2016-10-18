using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DesktopPlayModeController : ModeControllerBase, IPlayModeUI, ICanvasController, IEventSystemHandler, ILeaveEditPlayModeHandler, IActivateUIElement
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
	private Sprite mysteryBoxIcon;

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private TouristAdController touristAdController;

	[SerializeField]
	private TouristModeController touristModeController;

	public UnityAction OnLeaveEditPlayMode;

	private bool rewardReady;

	public ILockCursorManager LockCursorManager => lockCursorManager;

	public bool InLobbyState
	{
		get
		{
			return !lockCursorManager.LockCursor;
		}
		set
		{
			lockCursorManager.LockCursor = !value;
		}
	}

	private void Awake()
	{
		uiStack.Push(stackBottom);
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
	}

	private void Start()
	{
		RegisterHotkeys();
	}

	private void Update()
	{
		HandleFpsShortcut();
		HandleInput();
		HandleMysteryBoxNotification();
	}

	private void HandleMysteryBoxNotification()
	{
		if (RewardManager.NumberOfPendingRewards > 0)
		{
			if (!rewardReady)
			{
				rewardReady = true;
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add((byte)2, 8);
				NotificationController.OnNotificationReceived(NotificationType.SpinReady, dictionary);
			}
		}
		else
		{
			rewardReady = false;
		}
	}

	private void HandleInput()
	{
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.LobbyMenu))
		{
			lockCursorManager.LockCursor = false;
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.ToggleHD) && uiStack.IsStackEmpty())
		{
			ToggleHD();
		}
		if (MVGameControllerBase.IEditModeUI != null && MVInputWrapper.GetBooleanControlDown(KogamaControls.ToggleLogicRendering))
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
		lobbyState = UnityEngine.Object.Instantiate(lobbyState);
		lobbyState.SetParent(stackBottom.transform, worldPositionStays: false);
		if (MVGameControllerBase.Game.GameType == MVGameType.Classic)
		{
			MVInputWrapper.SetInputMap(new DesktopPlayMode());
		}
		else if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			MVInputWrapper.SetInputMap(new Desktop2DPlayMode());
		}
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1 && MVGameControllerBase.GameMode == MVGameMode.Play)
		{
			TeamMenu newTeamMenu = UnityEngine.Object.Instantiate(teamMenu);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
			{
				handler.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
			});
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(newTeamMenu.gameObject, UIPushOption.Blocking, null, UIGroupFlags.InventoryUI);
			});
		}
		accessoryShopController.Initialize();
		chatController.Initialize();
		playerListButton.gameObject.SetActive(value: true);
		MVGameControllerBase.WOCM.AvatarLocal.Body.AccessoryMoveOverride = true;
		ILockCursorManager lockCursorManager = this.lockCursorManager;
		lockCursorManager.OnCursorLockChanged = (Action<bool>)Delegate.Combine(lockCursorManager.OnCursorLockChanged, new Action<bool>(LobbyStateChange));
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
		inGameController = UnityEngine.Object.Instantiate(inGameController);
		inGameController.Initialize();
		inGameController.transform.SetParent(stackBottom.transform, worldPositionStays: false);
		playerListButton = UnityEngine.Object.Instantiate(playerListButton);
		playerListButton.transform.SetParent(stackBottom.transform, worldPositionStays: false);
		levelBadge = UnityEngine.Object.Instantiate(levelBadge);
		levelBadge.transform.SetParent(stackBottom.transform, worldPositionStays: false);
		notificationsManager = UnityEngine.Object.Instantiate(notificationsManager);
		notificationsManager.transform.SetParent(stackBottom.transform, worldPositionStays: false);
		touristAdController = UnityEngine.Object.Instantiate(touristAdController);
		touristAdController.transform.SetParent(stackBottom.transform, worldPositionStays: false);
		touristAdController.Initialize(touristModeController);
	}

	private void LobbyStateChange(bool cursorLocked)
	{
		lobbyState.gameObject.SetActive(!cursorLocked);
		inGameController.gameObject.SetActive(cursorLocked);
		chatController.OnLobbyStateChange(cursorLocked);
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
		if (element == ActivateUIElement.AvatarAccessoryShop)
		{
			accessoryShopController.Activate(UIPushOption.HideAll);
		}
	}

	public void LeaveEditPlayMode()
	{
		if (OnLeaveEditPlayMode != null)
		{
			OnLeaveEditPlayMode();
		}
	}

	public void SetPixelPerfect(bool pixelPerfect)
	{
		canvas.pixelPerfect = pixelPerfect;
	}
}
