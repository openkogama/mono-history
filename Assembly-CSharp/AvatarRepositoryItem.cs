using System.Collections.Generic;

public class AvatarRepositoryItem
{
	public byte[] data;

	public int priceGold;

	public int slotPosition;

	public string name;

	public int itemID;

	public AvatarRepositoryItem(Dictionary<object, object> outData, int key)
	{
		data = (byte[])((Dictionary<object, object>)outData[key])[(byte)92];
		itemID = key;
		name = "Avatar " + key;
		priceGold = (int)((Dictionary<object, object>)outData[key])[(byte)76];
		slotPosition = (int)((Dictionary<object, object>)outData[key])[(byte)101];
	}
}
