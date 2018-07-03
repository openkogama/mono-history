using UnityEngine;

public class LobbyStateController : MonoBehaviour
{
	[SerializeField]
	private GameObject touristRegisterButton;

	[SerializeField]
	private GameObject accessoryShop;

	[SerializeField]
	private TimedPlayReward playReward;

	[SerializeField]
	private GameObject gameCoinBoosterButton;

	private void Start()
	{
		bool isTouristSession = MVGameControllerBase.IsTouristSession;
		touristRegisterButton.SetActive(isTouristSession && !MVGameControllerBase.GameSessionData.IsPlayedFromPoki);
		if (!isTouristSession)
		{
			accessoryShop.SetActive(value: true);
			playReward.Initialize();
			gameCoinBoosterButton.SetActive(value: true);
		}
		playReward.gameObject.SetActive(value: false);
	}
}
