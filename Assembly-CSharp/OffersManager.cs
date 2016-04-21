using System;
using MV.Common;
using Newtonsoft.Json;
using UnityEngine.Events;

public static class OffersManager
{
	public static UnityAction OnActorOffer;

	public static IActorOfferClient CurrentOffer { get; private set; }

	public static void RequestOffer(UnityAction OnOffer)
	{
		OnActorOffer = OnOffer;
		MVGameControllerBase.OperationRequests.GetOffer();
	}

	public static void ClaimOffer()
	{
		MVGameControllerBase.OperationRequests.ClaimOffer();
	}

	public static void UpdateCurrentOffer(ActorOfferType actorOfferType, string jsonData)
	{
		CurrentOffer = Create(actorOfferType, jsonData);
		if (OnActorOffer != null)
		{
			OnActorOffer();
			OnActorOffer = null;
		}
	}

	private static IActorOfferClient Create(ActorOfferType actorOfferType, string jsonData)
	{
		return actorOfferType switch
		{
			ActorOfferType.Accessory => (IActorOfferClient)JsonConvert.DeserializeObject<ActorOfferAccessory>(jsonData), 
			ActorOfferType.Unavailable => JsonConvert.DeserializeObject<ActorOfferUnavailable>(jsonData), 
			_ => throw new Exception("Unknown offer type"), 
		};
	}
}
