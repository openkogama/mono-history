using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public class ClientShopRepository
{
	private readonly Dictionary<int, List<ShopItem>> repository = new Dictionary<int, List<ShopItem>>();

	public readonly Dictionary<InventoryCategoryType, string> categories = new Dictionary<InventoryCategoryType, string>
	{
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

	public ClientShopRepository()
	{
		foreach (InventoryCategoryType key in categories.Keys)
		{
			List<ShopItem> value = new List<ShopItem>();
			repository.Add((int)key, value);
		}
	}

	public void AddItem(ShopItem item)
	{
		if (!repository.ContainsKey(item.itemCategoryID))
		{
			Debug.LogError("Item in inventory with category which isn't valid.");
		}
		else
		{
			repository[item.itemCategoryID].Add(item);
		}
	}

	public void RemoveItem(ShopItem item)
	{
		repository[item.itemCategoryID].Remove(item);
	}

	public List<ShopItem> GetItemsInCategory(InventoryCategoryType category)
	{
		return new List<ShopItem>(repository[(int)category]);
	}

	public List<ShopItem> GetItemsInCategorySlow(string category)
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
		List<ShopItem> list = new List<ShopItem>(repository[key]);
		list.Reverse();
		return list;
	}

	public string GetCategoryStringFromId(InventoryCategoryType category)
	{
		return categories[category];
	}

	public int CategoryItemCount(InventoryCategoryType category)
	{
		return repository[(int)category].Count;
	}

	public bool GetItemByWorldObjectTypeInCategory(InventoryCategoryType inventoryCategory, WorldObjectType wo, out ShopItem item)
	{
		List<ShopItem> list = new List<ShopItem>(repository[(int)inventoryCategory]);
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

	public bool IsItemShopInventory(string itemName, InventoryCategoryType inventoryCategory)
	{
		List<ShopItem> list = new List<ShopItem>(repository[(int)inventoryCategory]);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].name == itemName)
			{
				return true;
			}
		}
		return false;
	}

	public void ReorganizeBySlotPositions()
	{
		foreach (List<ShopItem> value in repository.Values)
		{
			List<ShopItem> list = value.OrderBy((ShopItem o) => o.slotPosition).ToList();
			for (int num = 0; num < list.Count; num++)
			{
				list[num].slotPosition = num;
			}
		}
	}
}
