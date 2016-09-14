using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class RewardGenerator : RewardButtonBase
{
	[SerializeField]
	private Text rewardTimer;

	[SerializeField]
	private RewardMinigame rewardMinigamePrefab;

	[SerializeField]
	private PurchaseSpins purchaseSpinsPrefab;

	[SerializeField]
	private Image SpinnyBackground;

	private float maxTime;

	private bool rewardAvailable;

	private bool currentlySpinning;

	private bool changedState = true;

	private void Start()
	{
		RewardManager.NumberOfPendingRewardsChanged = (UnityAction)Delegate.Combine(RewardManager.NumberOfPendingRewardsChanged, new UnityAction(RewardCountChanged));
		SpinnyBackground.gameObject.SetActive(value: false);
		gameObject.SetActive(value: true);
	}

	public void OnRewardPressed()
	{
		if (currentlySpinning)
		{
			return;
		}
		if (rewardAvailable)
		{
			currentlySpinning = true;
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IOfferController x, BaseEventData y) =>
			{
				x.RequestShowOffer(OnRewardCollected);
			});
			return;
		}
		currentlySpinning = true;
		PurchaseSpins purchaseSpinsPopup = UnityEngine.Object.Instantiate(purchaseSpinsPrefab);
		purchaseSpinsPopup.Initialize(OnRewardCollected);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.Popup);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(purchaseSpinsPopup.gameObject, UIPushOption.Blocking, RewardCountChanged, UIGroupFlags.Popup);
		});
	}

	private void RewardCountChanged()
	{
		rewardAvailable = RewardManager.NumberOfPendingRewards > 0;
		changedState = true;
		currentlySpinning = false;
	}

	private void OnRewardCollected()
	{
		RewardMinigame rewardMinigame = UnityEngine.Object.Instantiate(rewardMinigamePrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(rewardMinigame.gameObject, UIPushOption.Blocking, RewardCountChanged, UIGroupFlags.GameObjectUI);
		});
	}

	private void Update()
	{
		float num = RewardManager.CountDownTimeInMS;
		if (rewardAvailable)
		{
			if (changedState)
			{
				rewardTimer.text = TM._("Claim!");
				changedState = false;
			}
			SpinnyBackground.gameObject.SetActive(value: true);
			EnableEffects();
		}
		else if (num > 0f)
		{
			float num2 = num / 1000f;
			if (num2 > maxTime)
			{
				maxTime = num2;
			}
			string arg = Mathf.Max(Mathf.Floor(num2 / 60f), 0f).ToString("00");
			string arg2 = (Mathf.Floor(num2) % 60f).ToString("00");
			rewardTimer.text = $"{arg:00}:{arg2:00}";
			UpdateOutline((maxTime - num2) / maxTime);
			DisableEffects();
			changedState = true;
		}
		else if (changedState)
		{
			rewardTimer.text = TM._("Loading");
			changedState = false;
			UpdateOutline(0f);
			DisableEffects();
		}
	}
}
