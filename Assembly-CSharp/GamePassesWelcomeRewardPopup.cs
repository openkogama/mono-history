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
		if (MVGameControllerBase.EditModeUI != null)
		{
			result = RewardedAdResult.RewardUnlocked;
		}
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
				x.Create(MVGameControllerBase.AdManager.RewardedAdNotAvailableText, TM._("No Ad Available"));
			});
			break;
		case RewardedAdResult.ErrorTimeout:
			break;
		}
	}

	private void ClaimReward(bool doubleReward = false)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		if (!GamePassesManager.PlayerPlanetData.playerPlanetMetaData.DailyWelcomeRewardClaimedToday())
		{
			MVGameControllerBase.OperationRequests.ClaimGamePointWelcomeReward(doubleReward);
		}
	}
}
