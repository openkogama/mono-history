using UnityEngine;

public class GamePassesWelcomeReward : MonoBehaviour
{
	[SerializeField]
	private Sprite crystalIcon;

	public void Initialize()
	{
		NotificationController.PushNotification(TM._("DAILY CRYSTALS RECEIVED: ") + GamePassesManager.playerTierStateCalculator.welcomeReward, crystalIcon);
		ClaimReward();
	}

	private void ClaimReward()
	{
		if (!GamePassesManager.PlayerPlanetData.playerPlanetMetaData.DailyWelcomeRewardClaimedToday())
		{
			MVGameControllerBase.OperationRequests.ClaimGamePointWelcomeReward();
		}
	}
}
