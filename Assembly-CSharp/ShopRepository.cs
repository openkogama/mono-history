using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public class ShopRepository : ARepository
{
	private Dictionary<int, MVItem> shopInventory;

	public List<int> ItemCategoriesInShop;

	public IDictionary<int, MVItem> ShopInventory => shopInventory;

	public ShopRepository()
	{
		shopInventory = new Dictionary<int, MVItem>();
		ItemCategoriesInShop = new List<int>();
	}

	public void ReorganizeItemsByItemType(bool notifyOfChange = false)
	{
		foreach (int item in ItemCategoriesInShop)
		{
			List<int> list = GetItemsByCategory(item).Keys.ToList();
			list.Sort((int a, int b) => (int)itemIDToInventorySlotIndex[a] - (int)itemIDToInventorySlotIndex[b]);
			for (int num = 0; num < list.Count; num++)
			{
				itemIDToInventorySlotIndex[list[num]] = num;
			}
		}
		if (notifyOfChange)
		{
			NotifyRepositoryChange();
		}
	}

	public override void RemoveItem(int itemId)
	{
		shopInventory.Remove(itemId);
		base.RemoveItem(itemId);
	}

	public Dictionary<int, MVItem> GetItemsByCategory(int itemCategory)
	{
		return GetItemsByItemCategories(new int[1] { itemCategory });
	}

	public Dictionary<int, MVItem> GetItemsByItemCategories(int[] itemCategories)
	{
		return ShopInventory.Where((KeyValuePair<int, MVItem> p) => itemCategories.Length == 0 || itemCategories.Contains(p.Value.itemCategoryID)).ToDictionary((KeyValuePair<int, MVItem> pair) => pair.Key, (KeyValuePair<int, MVItem> pair) => pair.Value);
	}

	public void CreateWorldObjectHierarchies()
	{
		Vector3 zero = Vector3.zero;
		Vector3 vector = Vector3.up * 10f;
		List<KoGaMaPackageClient> list = new List<KoGaMaPackageClient>();
		foreach (KeyValuePair<int, MVItem> item in ShopInventory)
		{
			KoGaMaPackageClient koGaMaPackageFromItem = ARepository.GetKoGaMaPackageFromItem(item.Value);
			koGaMaPackageFromItem.worldObjects[koGaMaPackageFromItem.worldObjectRoot].Visible = true;
			koGaMaPackageFromItem.worldObjects[koGaMaPackageFromItem.worldObjectRoot].Position = zero;
			zero += vector;
			list.Add(koGaMaPackageFromItem);
		}
	}
}
