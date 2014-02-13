using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVItemBusinessLogic
{
	private Dictionary<int, MVItem> items = new Dictionary<int, MVItem>();

	public void AddItem(MVItem item)
	{
		if (items.ContainsKey(item.itemID))
		{
			Debug.LogWarning((object)"Item already added to business logic. Will be overwritten!");
			return;
		}
		MVItem mVItem = new MVItem();
		mVItem.itemID = item.itemID;
		mVItem.resellable = item.resellable;
		mVItem.itemCategoryID = item.itemCategoryID;
		mVItem.itemTypeID = item.itemTypeID;
		mVItem.name = item.name;
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
