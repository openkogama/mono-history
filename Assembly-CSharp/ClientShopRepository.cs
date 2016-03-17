using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClientShopRepository
{
	private readonly Dictionary<int, List<ShopItem>> repository = new Dictionary<int, List<ShopItem>>();

	public readonly Dictionary<int, string> categories = new Dictionary<int, string>
	{
		{ 5, "Premium\nModels" },
		{ 7, "Pickups" },
		{ 8, "Blueprints" },
		{ 6, "Logic" },
		{ 10, "Advanced\nLogic" }
	};

	public ClientShopRepository()
	{
		foreach (int key in categories.Keys)
		{
			List<ShopItem> value = new List<ShopItem>();
			repository.Add(key, value);
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

	public List<ShopItem> GetItemsInCategory(string category)
	{
		int key = categories.FirstOrDefault((KeyValuePair<int, string> x) => x.Value == category).Key;
		return new List<ShopItem>(repository[key]);
	}

	public string GetCategoryStringFromId(int category)
	{
		return categories[category];
	}

	public int CategoryItemCount(int category)
	{
		return repository[category].Count;
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
