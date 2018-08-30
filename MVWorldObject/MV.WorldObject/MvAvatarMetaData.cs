namespace MV.WorldObject;

public class MvAvatarMetaData
{
	public readonly int avatarID = -1;

	public readonly string name = "";

	public readonly int priceGold = -1;

	public readonly bool isOnMarketPlace;

	public readonly bool canBeSoldOnMarketPlace;

	public MvAvatarMetaData(int avatarID, string name, int priceGold, bool isOnMarketPlace, bool canBeSoldOnMarketPlace)
	{
		this.avatarID = avatarID;
		this.name = name;
		this.priceGold = priceGold;
		this.isOnMarketPlace = isOnMarketPlace;
		this.canBeSoldOnMarketPlace = canBeSoldOnMarketPlace;
	}

	public MvAvatarMetaData(BytePacker bp)
	{
		avatarID = bp.ReadInt32();
		name = bp.ReadString();
		priceGold = bp.ReadInt32();
		isOnMarketPlace = bp.ReadBoolean();
		canBeSoldOnMarketPlace = bp.ReadBoolean();
	}

	public byte[] ToByteArray()
	{
		BytePacker bytePacker = new BytePacker();
		bytePacker.Write(avatarID);
		bytePacker.Write(name);
		bytePacker.Write(priceGold);
		bytePacker.Write(isOnMarketPlace);
		bytePacker.Write(canBeSoldOnMarketPlace);
		return bytePacker.ToArray();
	}

	public override string ToString()
	{
		return $"avatarID: {avatarID}. name: {name}. priceGold: {priceGold}. isOnMarketPlace: {isOnMarketPlace}. canBeSoldOnMarketPlace: {canBeSoldOnMarketPlace}.";
	}
}
