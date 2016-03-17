using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventoryRepository
{
	private readonly Dictionary<int, List<InventoryItem>> repository = new Dictionary<int, List<InventoryItem>>();

	public readonly Dictionary<int, string> categories = new Dictionary<int, string>
	{
		{ 1, "Cube\nModels" },
		{ 5, "Premium\nModels" },
		{ 7, "Pickups" },
		{ 8, "Blueprints" },
		{ 6, "Logic" },
		{ 10, "Advanced\nLogic" }
	};

	public Action OnInventoryChanged;

	public PlayerInventoryRepository()
	{
		foreach (int key in categories.Keys)
		{
			List<InventoryItem> value = new List<InventoryItem>();
			repository.Add(key, value);
		}
	}

	public void AddItem(InventoryItem item)
	{
		if (!repository.ContainsKey(item.itemCategoryID))
		{
			Debug.LogError("Item in inventory with category which isn't valid.");
			return;
		}
		repository[item.itemCategoryID].Add(item);
		if (OnInventoryChanged != null)
		{
			OnInventoryChanged();
		}
	}

	public void RemoveItem(InventoryItem item)
	{
		repository[item.itemCategoryID].Remove(item);
		if (OnInventoryChanged != null)
		{
			OnInventoryChanged();
		}
	}

	public int CountItemsWithOriginalID(InventoryItem item)
	{
		if (item.originalItemID == 0)
		{
			return 1;
		}
		return repository[item.itemCategoryID].Count((InventoryItem p) => p.originalItemID == item.originalItemID);
	}

	public void RemoveItem(int itemID)
	{
		List<InventoryItem> list = repository[1];
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].itemID == itemID)
			{
				repository[1].RemoveAt(i);
			}
		}
		if (OnInventoryChanged != null)
		{
			OnInventoryChanged();
		}
	}

	public void SwapItemSlotPositions(InventoryItem from, InventoryItem to)
	{
		Debug.Log("SwapItemSlotPositions");
		int slotPosition = from.slotPosition;
		from.slotPosition = to.slotPosition;
		to.slotPosition = slotPosition;
	}

	public void UpdateShopInventoryID(int itemID, int shopInventoryID)
	{
		List<InventoryItem> list = repository[1];
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].itemID == itemID)
			{
				list[i].shopInventoryID = shopInventoryID;
			}
		}
	}

	public List<InventoryItem> GetItemsInCategory(string category)
	{
		int key = categories.FirstOrDefault((KeyValuePair<int, string> x) => x.Value == category).Key;
		List<InventoryItem> list = new List<InventoryItem>(repository[key]);
		list.Reverse();
		return list;
	}

	public int CategoryItemCount(int category)
	{
		return repository[category].Count;
	}

	public int HighestSlotIndex(int category)
	{
		int num = 1;
		for (int i = 0; i < repository[category].Count; i++)
		{
			int slotPosition = repository[category][i].slotPosition;
			if (num < slotPosition)
			{
				num = slotPosition;
			}
		}
		return num;
	}

	public void AddPurchasedItem(ShopItem purchasedItem)
	{
		InventoryItem inventoryItem = new InventoryItem(purchasedItem);
		repository[purchasedItem.itemCategoryID].Add(inventoryItem);
		inventoryItem.slotPosition = repository[purchasedItem.itemCategoryID].Count - 1;
	}
}
