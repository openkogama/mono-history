using MV.Common;
using UnityEngine;
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
	private AdOfferGold adOfferGold;

	[SerializeField]
	private LobbyStateButton lobbyStatePlayButton;

	[SerializeField]
	private GamePassesUI gamePassesUIPrefab;

	[SerializeField]
	private GameObject boostButton;

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
		gamePassesUI = Object.Instantiate(gamePassesUIPrefab);
		gamePassesUI.transform.SetParent(transform, worldPositionStays: false);
		gamePassesUI.Initialize();
		gamePassesUI.TryShowWelcomeReward();
		if (!GamePassProgressionController.IsProgressionEnabled || MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit || !GamePassesManager.GamePassesActive)
		{
			gamePassesUI.gameObject.SetActive(value: false);
		}
	}

	public void SetShouldPopOnExit(bool shouldPop)
	{
		lobbyStatePlayButton.ShouldPop = shouldPop;
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

	private void OnEnable()
	{
		if (gamePassesUI != null)
		{
			gamePassesUI.gameObject.SetActive(GamePassProgressionController.IsProgressionEnabled && GamePassesManager.GamePassesActive);
		}
	}
}
