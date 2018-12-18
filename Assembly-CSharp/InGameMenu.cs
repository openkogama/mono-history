using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
	[SerializeField]
	private GameObject respawnButton;

	[SerializeField]
	private GameObject touristRegisterButton;

	[SerializeField]
	private TimedPlayReward playReward;

	[SerializeField]
	private AdOfferGold adOfferGold;

	[SerializeField]
	private Image inGameMenuBlockingOverlay;

	private void Start()
	{
		bool isTouristSession = MVGameControllerBase.IsTouristSession;
		bool flag = MVGameControllerBase.EditModeUI == null && !isTouristSession && MVGameControllerBase.GameMode == MVGameMode.Play;
		playReward.gameObject.SetActive(value: false);
		bool active = MVGameControllerBase.IsTouristSession && MVClientSettings.ShowTouristPromotion && !MVGameControllerBase.GameSessionData.IsPlayedFromPoki;
		touristRegisterButton.SetActive(active);
		if (flag)
		{
			playReward.Initialize();
		}
	}

	private void Update()
	{
		if (MVGameControllerDesktop.LockCursorManager.CursorLock)
		{
			inGameMenuBlockingOverlay.raycastTarget = true;
		}
		else
		{
			inGameMenuBlockingOverlay.raycastTarget = false;
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

	public void OnPressQuit()
	{
		MVGameControllerBase.PlayModeUI.InLobbyState = false;
		MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Hidden);
		MVGameControllerBase.PlayModeUI.InLobbyState = true;
	}
}
