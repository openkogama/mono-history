using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePassesWelcomeRewardPopup : MonoBehaviour
{
	[SerializeField]
	private Text amountGamePointEarned;

	[SerializeField]
	[Tooltip("Optional")]
	private Text amountGamePointEarnedDouble;

	public void Initialize()
	{
		amountGamePointEarned.text = GamePassesManager.playerTierStateCalculator.welcomeReward.ToString();
		if ((bool)amountGamePointEarnedDouble)
		{
			amountGamePointEarnedDouble.text = (GamePassesManager.playerTierStateCalculator.welcomeReward * 2).ToString();
		}
	}

	public void OnClaimPressed()
	{
		ClaimReward();
	}

	public void OnDoublePressed()
	{
		throw new NotImplementedException("Function not available for this platform.");
	}

	private void ClaimReward(bool doubleReward = false)
	{
		MVGameControllerBase.OperationRequests.ClaimGamePointWelcomeReward(doubleReward);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}
}
