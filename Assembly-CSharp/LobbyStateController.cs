using UnityEngine;
using UnityEngine.EventSystems;

public class LobbyStateController : LobbyFlowMenu
{
	[SerializeField]
	private GameObject touristRegisterButton;

	[SerializeField]
	private GameObject accessoryShop;

	[SerializeField]
	private LobbyStateButton lobbyStatePlayButton;

	[SerializeField]
	private LobbyStateButton playButton;

	[SerializeField]
	private GamePassesUI gamePassesUIPrefab;

	[SerializeField]
	private BoostMenuController boosterMenu;

	private GamePassesUI gamePassesUI;

	protected override LobbyFlowMenuType MenuType => LobbyFlowMenuType.LobbyState;

	public override void Start()
	{
		base.Start();
		bool isTouristSession = MVGameControllerBase.IsTouristSession;
		touristRegisterButton.SetActive(isTouristSession && !MVGameControllerBase.GameSessionData.IsPlayedFromPoki);
		accessoryShop.SetActive(value: true);
		gamePassesUI = Object.Instantiate(gamePassesUIPrefab);
		gamePassesUI.transform.SetParent(transform, worldPositionStays: false);
		gamePassesUI.Initialize();
		gamePassesUI.TryShowWelcomeReward();
		if (!GamePassProgressionController.IsProgressionEnabled || !GamePassesManager.GamePassesActive)
		{
			gamePassesUI.gameObject.SetActive(value: false);
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

	private void OnEnable()
	{
		if (gamePassesUI != null)
		{
			gamePassesUI.gameObject.SetActive(GamePassProgressionController.IsProgressionEnabled && GamePassesManager.GamePassesActive);
		}
		MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.AvatarLobbyFocus;
	}
}
