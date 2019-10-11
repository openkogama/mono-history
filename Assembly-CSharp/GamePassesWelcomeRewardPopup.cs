using Assets.Scripts.AdIntegration;
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
		MVGameControllerBase.AdManager.RequestRewardedAd(RewardedAdCallback, AdContext.ShowDailyCrystals);
	}

	private void RewardedAdCallback(RewardedAdResult result)
	{
		switch (result)
		{
		case RewardedAdResult.RewardUnlocked:
			ClaimReward(doubleReward: true);
			break;
		case RewardedAdResult.RewardNotUnlocked:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("The video was canceled. Your reward has not been doubled."), TM._("Video canceled"));
			});
			break;
		case RewardedAdResult.ErrorClient:
		case RewardedAdResult.ErrorInternal:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("The reward cannot be doubled at this moment."), TM._("An error occurred"));
			});
			break;
		case RewardedAdResult.ErrorTimeout:
			break;
		}
	}

	private void ClaimReward(bool doubleReward = false)
	{
		if (!GamePassesManager.PlayerPlanetData.playerPlanetMetaData.DailyWelcomeRewardClaimedToday())
		{
			MVGameControllerBase.OperationRequests.ClaimGamePointWelcomeReward(doubleReward);
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}
}
