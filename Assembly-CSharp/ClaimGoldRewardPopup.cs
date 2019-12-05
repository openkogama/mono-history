using Assets.Scripts.AdIntegration;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClaimGoldRewardPopup : MonoBehaviour
{
	[SerializeField]
	private Text goldRewardText;

	[SerializeField]
	private GameObject goldRewardUnlockedPopupPrefab;

	private void Start()
	{
		goldRewardText.text = 2.ToString();
	}

	private void OnDestroy()
	{
		MVGameControllerBase.GoldRewardManager.OnClaimGoldReward();
	}

	private void Update()
	{
		bool isBlocked = false;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			isBlocked = x.IsUIElementBlocked(gameObject);
		});
		if (MVGameControllerBase.GoldRewardManager.IsGoldRewardDone && !isBlocked)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
		}
	}

	public void TryClaimGold()
	{
		if (MVGameControllerBase.AdManager.ReadyForRewardedAdRequest)
		{
			MVGameControllerBase.GoldRewardManager.OnClaimGoldReward();
			MVGameControllerBase.AdManager.RequestRewardedAd(RewardedAdCallback, AdContext.GoldReward);
		}
		else
		{
			CreateErrorMessage();
		}
	}

	private void RewardedAdCallback(RewardedAdResult obj)
	{
		OnAdFinished(obj == RewardedAdResult.RewardUnlocked);
	}

	private void OnAdFinished(bool adWasSuccessful)
	{
		if (adWasSuccessful)
		{
			ClaimGold();
		}
		else
		{
			CreateErrorMessage();
		}
	}

	private void CreateErrorMessage()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(MVGameControllerBase.AdManager.RewardedAdNotAvailableText, TM._("No Ad Available"));
		});
	}

	private void ClaimGold()
	{
		MVGameControllerBase.OperationRequests.ClaimPlayingNewGameRewardedGold();
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		GameObject goldRewardUnlockedPopup = Object.Instantiate(goldRewardUnlockedPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(goldRewardUnlockedPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}
}
