using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVItemBusinessLogic
{
	private Dictionary<int, MVItem> items = new Dictionary<int, MVItem>();

	public void AddItem(MVItem item)
	{
		AddItemWithNoData(item.itemID, item.resellable, item.itemCategoryID, item.itemTypeID, item.name);
	}

	public void AddItemWithNoData(int itemID, bool resellable, int itemCategoryID, int itemTypeID, string name)
	{
		if (items.ContainsKey(itemID))
		{
			Debug.LogWarning("Item already added to business logic. Will be overwritten!");
			return;
		}
		MVItem mVItem = new MVItem();
		mVItem.itemID = itemID;
		mVItem.resellable = resellable;
		mVItem.itemCategoryID = itemCategoryID;
		mVItem.itemTypeID = itemTypeID;
		mVItem.name = name;
		items[mVItem.itemID] = mVItem;
	}

	public MVItem GetItem(int itemID)
	{
		if (!items.ContainsKey(itemID))
		{
			return null;
		}
		return items[itemID];
	}

	public bool CanAddItemToInventory(int itemID)
	{
		if (!items.ContainsKey(itemID))
		{
			if (!items.ContainsKey(itemID))
			{
				return false;
			}
			return items[itemID].resellable;
		}
		return items[itemID].resellable;
	}
}
