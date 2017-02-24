using System.Collections.Generic;
using ExitGames.Client.Photon;

public class InventoryItem
{
	public readonly int itemID;

	public readonly int itemCategoryID;

	public readonly int itemTypeID;

	public readonly byte[] data;

	public readonly bool resellable;

	public readonly int priceGold;

	public readonly bool purchased;

	public readonly int authorProfileID;

	public readonly int originalItemID;

	public readonly bool isDeleted;

	public readonly bool isDefaultInvItem;

	public int shopInventoryID;

	public string name;

	public string description;

	public int slotPosition;

	public InventoryItem()
	{
	}

	public InventoryItem(EventData data)
	{
		itemID = (int)data[38];
		itemCategoryID = (int)data[150];
		itemTypeID = (int)data[39];
		name = (string)data[40];
		this.data = (byte[])data[41];
		slotPosition = (int)data[43];
		resellable = (bool)data[138];
		authorProfileID = (int)data[137];
		originalItemID = (int)data[139];
		priceGold = (int)data[67];
		isDefaultInvItem = false;
	}

	public InventoryItem(int itemID, Dictionary<object, object> itemData)
	{
		this.itemID = itemID;
		itemCategoryID = (int)itemData[(byte)116];
		itemTypeID = (int)itemData[(byte)15];
		name = (string)itemData[(byte)10];
		description = (string)itemData[(byte)107];
		resellable = (bool)itemData[(byte)104];
		priceGold = (int)itemData[(byte)76];
		shopInventoryID = (int)itemData[(byte)108];
		authorProfileID = (int)itemData[(byte)106];
		originalItemID = (int)itemData[(byte)110];
		isDeleted = (bool)itemData[(byte)109];
		if (!isDeleted)
		{
			data = (byte[])itemData[(byte)11];
		}
		isDefaultInvItem = (bool)itemData[(byte)141];
	}

	public InventoryItem(ShopItem itemToCopy)
	{
		itemCategoryID = itemToCopy.itemCategoryID;
		itemTypeID = itemToCopy.itemTypeID;
		name = itemToCopy.name;
		description = itemToCopy.description;
		resellable = itemToCopy.resellable;
		priceGold = itemToCopy.priceGold;
		data = itemToCopy.data;
		itemID = itemToCopy.itemID;
		isDeleted = false;
		isDefaultInvItem = false;
	}
}
