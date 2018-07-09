using UnityEngine;

public class LobbyStateController : MonoBehaviour
{
	[SerializeField]
	private GameObject touristRegisterButton;

	[SerializeField]
	private GameObject accessoryShop;

	[SerializeField]
	private TimedPlayReward playReward;

	private void Start()
	{
		bool isTouristSession = MVGameControllerBase.IsTouristSession;
		touristRegisterButton.SetActive(isTouristSession && !MVGameControllerBase.GameSessionData.IsPlayedFromPoki);
		if (!isTouristSession)
		{
			accessoryShop.SetActive(value: true);
			playReward.Initialize();
		}
		playReward.gameObject.SetActive(value: false);
	}
}
