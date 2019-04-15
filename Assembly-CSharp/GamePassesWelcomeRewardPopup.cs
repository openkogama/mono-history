using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePassesWelcomeRewardPopup : MonoBehaviour
{
	[SerializeField]
	private Text amountGamePointEarned;

	private int gamePointsEarned;

	public void Initialize()
	{
		gamePointsEarned = GamePassesManager.playerTierStateCalculator.welcomeReward;
		amountGamePointEarned.text = gamePointsEarned.ToString();
	}

	public void OnClaimPressed()
	{
		MVGameControllerBase.OperationRequests.ClaimGamePointWelcomeReward();
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}
}
