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
	private Image lobbyStateBlockingOverlay;

	private void Start()
	{
		bool isTouristSession = MVGameControllerBase.IsTouristSession;
		bool active = MVGameControllerBase.IsTouristSession && MVClientSettings.ShowTouristPromotion && !MVGameControllerBase.GameSessionData.IsPlayedFromPoki;
		touristRegisterButton.SetActive(active);
		avatarAccessoriesButton.SetActive(value: true);
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
}
