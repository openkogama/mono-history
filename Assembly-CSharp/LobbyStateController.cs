using UnityEngine;

public class LobbyStateController : MonoBehaviour
{
	[SerializeField]
	private GameObject touristRegisterButton;

	[SerializeField]
	private GameObject accessoryShop;

	[SerializeField]
	private GamePassesUI gamePassesUIPrefab;

	[SerializeField]
	private GameObject boostButton;

	private GamePassesUI gamePassesUI;

	private void Start()
	{
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

	private void OnEnable()
	{
		if (gamePassesUI != null)
		{
			gamePassesUI.gameObject.SetActive(GamePassProgressionController.IsProgressionEnabled && GamePassesManager.GamePassesActive);
		}
	}
}
