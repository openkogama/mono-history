using System.Collections.Generic;

public class ShopItem
{
	public int itemID;

	public int itemCategoryID;

	public int itemTypeID;

	public string name;

	public string description;

	public byte[] data;

	public bool resellable;

	public int priceGold;

	public int slotPosition;

	public bool purchased;

	public ShopItem(int key, Dictionary<object, object> outData)
	{
		itemID = key;
		itemCategoryID = (int)((Dictionary<object, object>)outData[key])[(byte)116];
		itemTypeID = (int)((Dictionary<object, object>)outData[key])[(byte)15];
		name = (string)((Dictionary<object, object>)outData[key])[(byte)10];
		description = (string)((Dictionary<object, object>)outData[key])[(byte)107];
		data = (byte[])((Dictionary<object, object>)outData[key])[(byte)11];
		resellable = (bool)((Dictionary<object, object>)outData[key])[(byte)104];
		priceGold = (int)((Dictionary<object, object>)outData[key])[(byte)76];
		slotPosition = (int)((Dictionary<object, object>)outData[key])[(byte)101];
	}

	public void ApplyLocalDescriptionOverride(MVWorldObjectDocumentationType t)
	{
		if (InventoryItem.localItemDescriptionOverride.ContainsKey(t))
		{
			InventoryItem.ItemDescription itemDescription = InventoryItem.localItemDescriptionOverride[t];
			name = itemDescription.Name;
			description = itemDescription.Description;
		}
	}
}
