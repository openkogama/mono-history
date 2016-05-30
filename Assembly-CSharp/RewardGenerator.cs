using System;
using MV.Common;
using MV.WorldObject;
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
	private OfferAvatarPopup avatarAdPrefab;

	[SerializeField]
	private RewardMinigame rewardMinigamePrefab;

	[SerializeField]
	private PurchaseSpins purchaseSpinsPrefab;

	[SerializeField]
	private Image SpinnyBackground;

	private float maxTime;

	private bool rewardAvailable;

	private bool offerReady = true;

	private bool currentlySpinning;

	private void Start()
	{
		RewardManager.NumberOfPendingRewardsChanged = (UnityAction)Delegate.Combine(RewardManager.NumberOfPendingRewardsChanged, new UnityAction(RewardCountChanged));
		RewardManager.TimerUpdated = (UnityAction)Delegate.Combine(RewardManager.TimerUpdated, new UnityAction(OfferPrepared));
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
			if (!offerReady)
			{
				OnRewardCollected();
				return;
			}
			OffersManager.RequestOffer(OfferReady);
			offerReady = false;
		}
		else
		{
			currentlySpinning = true;
			PurchaseSpins purchaseSpinsPopup = UnityEngine.Object.Instantiate(purchaseSpinsPrefab);
			purchaseSpinsPopup.Initialize(OnRewardCollected);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(purchaseSpinsPopup.gameObject, UIPushOption.Blocking, RewardCountChanged, UIGroupFlags.Popup);
			});
		}
	}

	private void OfferReady()
	{
		IActorOfferClient currentOffer = OffersManager.CurrentOffer;
		switch (currentOffer.ActorOfferType)
		{
		case ActorOfferType.Accessory:
			accessoryAdCreator.CreateOffer(OnRewardCollected, currentOffer as ActorOfferAccessory);
			break;
		case ActorOfferType.Avatar:
			CreateActorOffer(currentOffer);
			break;
		case ActorOfferType.Unavailable:
			OnRewardCollected();
			break;
		}
	}

	private void CreateActorOffer(IActorOfferClient offer)
	{
		MVWorldObjectClient worldObjectFromItemData = GetWorldObjectFromItemData(((ActorOfferAvatar)offer).avatarData);
		Debug.Log(offer);
		OfferAvatarPopup avatarOffer = UnityEngine.Object.Instantiate(avatarAdPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(avatarOffer.gameObject, UIPushOption.Blocking, OnRewardCollected, UIGroupFlags.Popup);
		});
		avatarOffer.CreateOffer((ActorOfferAvatar)offer, worldObjectFromItemData);
	}

	private static MVWorldObjectClient GetWorldObjectFromItemData(byte[] data)
	{
		BytePacker koGaMaData = new BytePacker(data);
		KoGaMaPackageClient koGaMaPackageClient = new KoGaMaPackageClient(koGaMaData, readRuntimeValues: false);
		MVWorldObjectClient mVWorldObjectClient = koGaMaPackageClient.worldObjects[koGaMaPackageClient.worldObjectRoot];
		mVWorldObjectClient.InitializeInventory();
		return mVWorldObjectClient;
	}

	private void OfferPrepared()
	{
		offerReady = true;
	}

	private void RewardCountChanged()
	{
		rewardAvailable = RewardManager.NumberOfPendingRewards > 0;
		currentlySpinning = false;
	}

	private void OnRewardCollected()
	{
		RewardMinigame rewardMinigame = UnityEngine.Object.Instantiate(rewardMinigamePrefab);
		rewardMinigame.Initialize();
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
			string arg2 = (Mathf.Floor(num2) % 60f).ToString("00");
			rewardTimer.text = $"{arg:00}:{arg2:00}";
			UpdateOutline((maxTime - num2) / maxTime);
			DisableEffects();
		}
		else
		{
			rewardTimer.text = TM._("Loading");
			UpdateOutline(0f);
			DisableEffects();
		}
	}
}
