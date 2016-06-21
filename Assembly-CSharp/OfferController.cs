using System;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class OfferController : MonoBehaviour, IEventSystemHandler, IOfferController
{
	[SerializeField]
	private AccessoryAdCreator accessoryAdCreator;

	[SerializeField]
	private OfferAvatarPopup avatarAdPrefab;

	private bool offerReady = true;

	private UnityAction OnOfferClosed;

	private void Start()
	{
		RewardManager.TimerUpdated = (UnityAction)Delegate.Combine(RewardManager.TimerUpdated, new UnityAction(OfferPrepared));
		if (RewardManager.IsInitialized)
		{
			OfferPrepared();
		}
	}

	private void OnDestroy()
	{
		RewardManager.TimerUpdated = (UnityAction)Delegate.Remove(RewardManager.TimerUpdated, new UnityAction(OfferPrepared));
		OnOfferClosed = null;
	}

	public void RequestShowOffer(UnityAction OnOfferClosed)
	{
		this.OnOfferClosed = OnOfferClosed;
		if (!offerReady)
		{
			OnOfferClosed();
			return;
		}
		OffersManager.RequestOffer(CreateOffer);
		offerReady = false;
	}

	private void CreateOffer()
	{
		if (OnOfferClosed != null)
		{
			IActorOfferClient currentOffer = OffersManager.CurrentOffer;
			switch (currentOffer.ActorOfferType)
			{
			case ActorOfferType.Accessory:
				accessoryAdCreator.CreateOffer(OnOfferClosed, currentOffer as ActorOfferAccessory);
				break;
			case ActorOfferType.Avatar:
				CreateActorOffer(currentOffer);
				break;
			case ActorOfferType.Unavailable:
				OnOfferClosed();
				break;
			}
		}
	}

	private void CreateActorOffer(IActorOfferClient offer)
	{
		MVWorldObjectClient worldObjectFromItemData = GetWorldObjectFromItemData(((ActorOfferAvatar)offer).avatarData);
		Debug.Log(offer);
		OfferAvatarPopup avatarOffer = UnityEngine.Object.Instantiate(avatarAdPrefab);
		avatarOffer.CreateOffer((ActorOfferAvatar)offer, worldObjectFromItemData);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(avatarOffer.gameObject, UIPushOption.Blocking, OnOfferClosed, UIGroupFlags.Popup);
		});
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
}
