using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class DesktopLobbyStateController : MonoBehaviour
{
	[SerializeField]
	private GameObject teamButton;

	[SerializeField]
	private GameObject respawnButton;

	[SerializeField]
	private GameObject avatarAccessoriesButton;

	[SerializeField]
	private GameObject touristRegisterButton;

	[SerializeField]
	private TimedPlayReward playReward;

	[SerializeField]
	private AdOfferGold adOfferGold;

	[SerializeField]
	private Image lobbyStateBlockingOverlay;

	private void Start()
	{
		bool isTouristSession = MVGameControllerBase.IsTouristSession;
		bool flag = MVGameControllerBase.IEditModeUI == null && !isTouristSession && MVGameControllerBase.GameMode == MVGameMode.Play;
		playReward.gameObject.SetActive(value: false);
		bool active = MVGameControllerBase.IsTouristSession && MVClientSettings.ShowTouristPromotion && !MVGameControllerBase.GameSessionData.IsPlayedFromPoki;
		touristRegisterButton.SetActive(active);
		avatarAccessoriesButton.SetActive(!isTouristSession);
		if (flag)
		{
			playReward.Initialize();
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
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			teamButton.SetActive(value: true);
		}
		else
		{
			teamButton.SetActive(value: false);
		}
	}
}
