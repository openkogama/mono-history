using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DesktopLobbyStateController : LobbyFlowMenu
{
	[SerializeField]
	private GameObject respawnButton;

	[SerializeField]
	private GameObject avatarAccessoriesButton;

	[SerializeField]
	private GameObject touristRegisterButton;

	[SerializeField]
	private LobbyStateButton playButton;

	[SerializeField]
	private GameObject goldIconOnPlayButton;

	[SerializeField]
	private AdOfferGold adOfferGold;

	[SerializeField]
	private LobbyStateButton lobbyStatePlayButton;

	[SerializeField]
	private GamePassesUI gamePassesUIPrefab;

	[SerializeField]
	private GameObject startGoldRewardPopupPrefab;

	[SerializeField]
	private BoostMenuController boosterMenu;

	[SerializeField]
	private Image lobbyStateBlockingOverlay;

	private GamePassesUI gamePassesUI;

	protected override LobbyFlowMenuType MenuType => LobbyFlowMenuType.LobbyState;

	public override void Start()
	{
		base.Start();
		bool isTouristSession = MVGameControllerBase.IsTouristSession;
		bool active = MVGameControllerBase.IsTouristSession && MVClientSettings.ShowTouristPromotion && !MVGameControllerBase.GameSessionData.IsPlayedFromPoki;
		touristRegisterButton.SetActive(active);
		avatarAccessoriesButton.SetActive(value: true);
		SetCamMaskMode();
		gamePassesUI = Object.Instantiate(gamePassesUIPrefab);
		gamePassesUI.transform.SetParent(transform, worldPositionStays: false);
		gamePassesUI.Initialize();
		gamePassesUI.TryShowWelcomeReward();
		if (!GamePassProgressionController.IsProgressionEnabled || MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit || !GamePassesManager.GamePassesActive)
		{
			gamePassesUI.gameObject.SetActive(value: false);
		}
		if (MVGameControllerBase.GoldRewardManager.CanGetGoldReward() && !MVGameControllerBase.GoldRewardManager.IsCountingDownGoldReward)
		{
			EnableGoldReward();
		}
	}

	public void SetShouldPopOnExit(bool shouldPop)
	{
		lobbyStatePlayButton.ShouldPop = shouldPop;
	}

	public void ShowBoostMenu()
	{
		BoostMenuController boostMenu = Object.Instantiate(boosterMenu);
		boostMenu.Initialize();
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(boostMenu.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		playButton.CancelEnterPlay();
	}

	public void CreateStartGoldRewardPopup()
	{
		GameObject startGoldRewardPopup = Object.Instantiate(startGoldRewardPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(startGoldRewardPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	private void Update()
	{
		if (MVGameControllerDesktop.LockCursorManager.CursorLock)
		{
			lobbyStateBlockingOverlay.raycastTarget = true;
		}
		else
		{
			lobbyStateBlockingOverlay.raycastTarget = false;
		}
		if (!MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Playing) && respawnButton.gameObject.activeSelf)
		{
			respawnButton.gameObject.SetActive(value: false);
		}
		else if (MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Playing) && !respawnButton.gameObject.activeSelf)
		{
			respawnButton.gameObject.SetActive(value: true);
		}
		MVInputWrapper.SuppressInGameInput();
	}

	private void SetCamMaskMode()
	{
		if (MVGameControllerBase.GameMode != MVGameMode.Edit)
		{
			MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.AvatarLobbyFocus;
		}
		else if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.Default;
		}
	}

	private void EnableGoldReward()
	{
		MVGameControllerBase.GoldRewardManager.StartGoldRewardCountdownWhenReady();
		goldIconOnPlayButton.SetActive(value: true);
	}

	private void OnEnable()
	{
		if (gamePassesUI != null)
		{
			gamePassesUI.gameObject.SetActive(GamePassProgressionController.IsProgressionEnabled && GamePassesManager.GamePassesActive);
		}
		SetCamMaskMode();
	}

	private void OnDisable()
	{
		if (MVGameControllerBase.MainCameraManager.CamMaskMode == MaskMode.AvatarLobbyFocus)
		{
			MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.Default;
		}
	}
}
