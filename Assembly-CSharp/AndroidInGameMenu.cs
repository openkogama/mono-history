using MV.Common;
using UnityEngine;

public class AndroidInGameMenu : MonoBehaviour
{
	[SerializeField]
	private GameObject touristRegisterButton;

	[SerializeField]
	private TimedPlayReward playReward;

	[SerializeField]
	private GameObject winningConditionDebriefing;

	[SerializeField]
	private GamePassesUI gamePassesUIPrefab;

	private GamePassesUI gamePassesUI;

	public void Initialize()
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
		gamePassesUI = Object.Instantiate(gamePassesUIPrefab);
		gamePassesUI.transform.SetParent(transform, worldPositionStays: false);
		gamePassesUI.Initialize();
		if (!GamePassProgressionController.IsProgressionEnabled || !GamePassesManager.GamePassesActive)
		{
			gamePassesUI.gameObject.SetActive(value: false);
		}
		winningConditionDebriefing.transform.SetAsLastSibling();
	}

	private void OnEnable()
	{
		if (gamePassesUI != null)
		{
			gamePassesUI.gameObject.SetActive(GamePassProgressionController.IsProgressionEnabled && GamePassesManager.GamePassesActive);
		}
	}

	public void OnPressQuit()
	{
		MVGameControllerBase.PlayModeUI.InLobbyState = false;
		MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Hidden);
		MVGameControllerBase.PlayModeUI.InLobbyState = true;
	}
}
