using MV.Common;
using UnityEngine;

public class AndroidInGameMenu : MonoBehaviour
{
	[SerializeField]
	private GameObject touristRegisterButton;

	[SerializeField]
	private TimedPlayReward playReward;

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

	public void OnPressQuit()
	{
		MVGameControllerBase.PlayModeUI.InLobbyState = false;
		MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Hidden);
		MVGameControllerBase.PlayModeUI.InLobbyState = true;
	}
}
