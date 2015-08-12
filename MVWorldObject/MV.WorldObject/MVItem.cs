using System.Collections.Generic;

namespace MV.WorldObject;

public class MVItem
{
	public int itemID;

	public int itemCategoryID;

	public int itemTypeID;

	public int originalItemID;

	public int marketPlaceItemID;

	public string name;

	public string description;

	public byte[] data;

	public bool resellable;

	public int authorProfileID;

	public int shopInventoryID;

	public int priceSilver;

	public int priceGold;

	public bool isDeleted;

	public MVItem()
	{
	}

	public MVItem(int itemID, Dictionary<object, object> itemData)
	{
		this.itemID = itemID;
		itemCategoryID = (int)itemData[(byte)116];
		itemTypeID = (int)itemData[(byte)15];
		name = (string)itemData[(byte)10];
		description = (string)itemData[(byte)107];
		resellable = (bool)itemData[(byte)104];
		priceSilver = (int)itemData[(byte)77];
		priceGold = (int)itemData[(byte)76];
		shopInventoryID = (int)itemData[(byte)108];
		authorProfileID = (int)itemData[(byte)106];
		originalItemID = (int)itemData[(byte)110];
		isDeleted = (bool)itemData[(byte)109];
		if (!isDeleted)
		{
			data = (byte[])itemData[(byte)11];
		}
	}

	public override string ToString()
	{
		return $"Name: {name} ItemID: {itemID} ItemTypeID: {itemTypeID}\nPriceSilver: {priceSilver} PriceGold: {priceGold} Resellable: {resellable} \n";
	}
}
