using System;
using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public class PlayerInventoryRepository
{
	private readonly Dictionary<int, List<InventoryItem>> repository = new Dictionary<int, List<InventoryItem>>();

	public readonly Dictionary<InventoryCategoryType, string> categories = new Dictionary<InventoryCategoryType, string>
	{
		{
			InventoryCategoryType.CubeModels,
			"Cube\nModels"
		},
		{
			InventoryCategoryType.PremiumModels,
			"Premium\nModels"
		},
		{
			InventoryCategoryType.Pickups,
			"Pickups"
		},
		{
			InventoryCategoryType.Blueprints,
			"Blueprints"
		},
		{
			InventoryCategoryType.Logic,
			"Logic"
		},
		{
			InventoryCategoryType.AdvancedLogic,
			"Advanced\nLogic"
		}
	};

	public Action OnInventoryChanged;

	public Action<int, int> OnInventoryItemAdded;

	public Action OnFailedToAddItem;

	public PlayerInventoryRepository()
	{
		foreach (InventoryCategoryType key in categories.Keys)
		{
			List<InventoryItem> value = new List<InventoryItem>();
			repository.Add((int)key, value);
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
		if (OnInventoryItemAdded != null)
		{
			OnInventoryItemAdded(item.itemCategoryID, item.slotPosition);
		}
	}

	public void FailedToAddItem()
	{
		if (OnFailedToAddItem != null)
		{
			OnFailedToAddItem();
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

	public List<InventoryItem> GetItemsInCategory(InventoryCategoryType category)
	{
		List<InventoryItem> list = new List<InventoryItem>(repository[(int)category]);
		list.Reverse();
		return list;
	}

	public List<InventoryItem> GetItemsInCategorySlow(string category)
	{
		int key = 0;
		foreach (KeyValuePair<InventoryCategoryType, string> category2 in categories)
		{
			if (category2.Value == category)
			{
				key = (int)category2.Key;
				break;
			}
		}
		List<InventoryItem> list = new List<InventoryItem>(repository[key]);
		list.Reverse();
		return list;
	}

	public bool GetItemByWorldObjectTypeInCategory(InventoryCategoryType inventoryCategory, WorldObjectType wo, out InventoryItem item)
	{
		List<InventoryItem> list = new List<InventoryItem>(repository[(int)inventoryCategory]);
		for (int i = 0; i < list.Count; i++)
		{
			BytePacker koGaMaData = new BytePacker(list[i].data);
			KoGaMaPackageClient koGaMaPackageClient = new KoGaMaPackageClient(koGaMaData, readRuntimeValues: false);
			koGaMaPackageClient.InventoryInitialize();
			koGaMaPackageClient.Destroy();
			Dictionary<int, MVWorldObjectClient> worldObjects = koGaMaPackageClient.worldObjects;
			foreach (MVWorldObjectClient value in worldObjects.Values)
			{
				if (value.WorldObjectType == wo)
				{
					item = list[i];
					return true;
				}
			}
		}
		item = null;
		return false;
	}

	public int CategoryItemCount(int category)
	{
		return repository[category].Count;
	}

	public int HighestSlotIndex(InventoryCategoryType category)
	{
		int num = 1;
		for (int i = 0; i < repository[(int)category].Count; i++)
		{
			int slotPosition = repository[(int)category][i].slotPosition;
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
		if (OnInventoryChanged != null)
		{
			OnInventoryChanged();
		}
		if (OnInventoryItemAdded != null)
		{
			OnInventoryItemAdded(inventoryItem.itemCategoryID, inventoryItem.slotPosition);
		}
	}
}
