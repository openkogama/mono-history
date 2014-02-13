using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class ShopRepositoryCollection : RepositoryCollection
{
	public ShopRepositoryCollection(ARepository repository, int[] allowedItemCategories)
		: base(repository, allowedItemCategories)
	{
	}

	protected override int CountItems()
	{
		ShopRepository shopRepository = (ShopRepository)repository;
		return shopRepository.GetItemsByItemCategories(allowedItemCategories).Keys.Count;
	}

	public override int GetSlotIndexFromItemId(int itemId)
	{
		IUXCollectionItem[] array = cache;
		for (int i = 0; i < array.Length; i++)
		{
			DefaultCollectionItem defaultCollectionItem = (DefaultCollectionItem)array[i];
			MVItem mVItem = (MVItem)defaultCollectionItem.Object;
			Debug.Log((object)"GetSlotIndexFromItemId");
			if (mVItem.itemID == itemId)
			{
				return defaultCollectionItem.Index;
			}
		}
		return -1;
	}

	protected override void HandleRepositoryChange(ARepository repository)
	{
		if (!(repository is ShopRepository))
		{
			Debug.LogError((object)string.Concat("Trying to use Repository of type '", repository.GetType(), "' in ShopRepositoryCollection"));
			return;
		}
		ShopRepository shopRepository = (ShopRepository)repository;
		Dictionary<int, MVItem> itemsByItemCategories = shopRepository.GetItemsByItemCategories(allowedItemCategories);
		cache = new IUXCollectionItem[itemsByItemCategories.Values.Count];
		int num = 0;
		foreach (KeyValuePair<int, MVItem> item in itemsByItemCategories)
		{
			cache[num++] = new DefaultCollectionItem((int)shopRepository.itemIDToInventorySlotIndex[item.Key], item.Value);
		}
		NotifyOnCollectionChange();
	}
}
