using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class DesktopLobbyStateController : MonoBehaviour
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
	private GamePassesUI gamePassesUIPrefab;

	[SerializeField]
	private Image lobbyStateBlockingOverlay;

	private GamePassesUI gamePassesUI;

	private void Start()
	{
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
		if (!MVGameControllerBase.WOCM.AvatarLocal.IsInMode(AvatarModeTypes.Playing) && respawnButton.gameObject.activeSelf)
		{
			respawnButton.gameObject.SetActive(value: false);
		}
		else if (MVGameControllerBase.WOCM.AvatarLocal.IsInMode(AvatarModeTypes.Playing) && !respawnButton.gameObject.activeSelf)
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
