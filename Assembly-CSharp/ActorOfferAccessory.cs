using MV.Common;

public class ActorOfferAccessory : IActorOfferClient
{
	public int priceGold;

	public int streamingAssetsId;

	public string name;

	public string description;

	public string url;

	public int extraSpins;

	public ActorOfferType ActorOfferType { get; set; }
}
