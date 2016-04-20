using System;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class RewardGenerator : RewardButtonBase
{
	[SerializeField]
	private Text rewardTimer;

	[SerializeField]
	private AccessoryAdCreator accessoryAdCreator;

	[SerializeField]
	private RewardMinigame rewardMinigamePrefab;

	[SerializeField]
	private PurchaseSpins purchaseSpinsPrefab;

	[SerializeField]
	private Image SpinnyBackground;

	private float maxTime;

	private bool rewardAvailable;

	private void Start()
	{
		RewardManager.NumberOfPendingRewardsChanged = (UnityAction)Delegate.Combine(RewardManager.NumberOfPendingRewardsChanged, new UnityAction(RewardCountChanged));
		SpinnyBackground.gameObject.SetActive(value: false);
	}

	public void OnRewardPressed()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.Popup);
		});
		if (rewardAvailable)
		{
			if (RewardManager.CountDownTimeInMS > 0)
			{
				OnRewardCollected();
				return;
			}
			OffersManager.OnActorOffer = (UnityAction)Delegate.Combine(OffersManager.OnActorOffer, new UnityAction(OfferReady));
			OffersManager.RequestOffer();
		}
		else
		{
			PurchaseSpins purchaseSpinsPopup = UnityEngine.Object.Instantiate(purchaseSpinsPrefab);
			purchaseSpinsPopup.Initialize(OnPurchaseSpinsPop);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(purchaseSpinsPopup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
			});
		}
	}

	private void OnPurchaseSpinsPop()
	{
		OffersManager.OnActorOffer = (UnityAction)Delegate.Combine(OffersManager.OnActorOffer, new UnityAction(OfferReady));
		OffersManager.RequestOffer();
	}

	private void OfferReady()
	{
		IActorOfferClient currentOffer = OffersManager.CurrentOffer;
		switch (currentOffer.ActorOfferType)
		{
		case ActorOfferType.Accessory:
			accessoryAdCreator.Initialize(OnRewardCollected, currentOffer as ActorOfferAccessory);
			break;
		case ActorOfferType.Unavailable:
			OnRewardCollected();
			break;
		}
	}

	private void RewardCountChanged()
	{
		rewardAvailable = RewardManager.NumberOfPendingRewards > 0;
	}

	private void OnRewardCollected()
	{
		RewardMinigame rewardMinigame = UnityEngine.Object.Instantiate(rewardMinigamePrefab);
		rewardMinigame.Initialize();
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(rewardMinigame.gameObject, UIPushOption.Blocking, null, UIGroupFlags.GameObjectUI);
		});
		gameObject.SetActive(RewardManager.CountDownTimeInMS > 0);
	}

	private void Update()
	{
		float num = RewardManager.CountDownTimeInMS;
		if (RewardManager.NumberOfPendingRewards > 0)
		{
			rewardTimer.text = TM._("Claim!");
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
			string arg2 = (num2 % 60f).ToString("00");
			rewardTimer.text = $"{arg:00}:{arg2:00}";
			UpdateOutline((maxTime - num2) / maxTime);
			DisableEffects();
		}
	}
}
