using MV.Common;

public class ActorOfferAvatar : IActorOfferClient
{
	public int avatarID = -1;

	public int priceGold = -1;

	public string name;

	public byte[] avatarData;

	public int extraSpins;

	public string creatorName;

	public ActorOfferType ActorOfferType { get; set; }

	public override string ToString()
	{
		return $"avatarID {avatarID}. priceGold {priceGold}. name {name}. avatarData.length {avatarData.Length}. extraSpins {extraSpins}. creatorName {creatorName}.";
	}
}
