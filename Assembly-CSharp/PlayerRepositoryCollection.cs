using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class PlayerRepositoryCollection : RepositoryCollection
{
	public PlayerRepositoryCollection(ARepository repository, int[] allowedItemCategories)
		: base(repository, allowedItemCategories)
	{
	}

	protected override int CountItems()
	{
		PlayerRepository playerRepository = (PlayerRepository)repository;
		return playerRepository.GetItemsByItemCategory(allowedItemCategories).Keys.Count;
	}

	public override int GetSlotIndexFromItemId(int itemId)
	{
		IUXCollectionItem[] array = cache;
		for (int i = 0; i < array.Length; i++)
		{
			DefaultCollectionItem defaultCollectionItem = (DefaultCollectionItem)array[i];
			if ((defaultCollectionItem.Object as MVItem).itemID == itemId)
			{
				return defaultCollectionItem.Index;
			}
		}
		return -1;
	}

	protected override void HandleRepositoryChange(ARepository repository)
	{
		if (!(repository is PlayerRepository))
		{
			Debug.LogError((object)string.Concat("Trying to use Repository of type '", repository.GetType(), "' in PlayerRepositoryCollection"));
			return;
		}
		PlayerRepository playerRepository = (PlayerRepository)repository;
		Dictionary<int, MVItem> itemsByItemCategory = playerRepository.GetItemsByItemCategory(allowedItemCategories);
		cache = new IUXCollectionItem[itemsByItemCategory.Values.Count];
		int num = 0;
		foreach (KeyValuePair<int, MVItem> item in itemsByItemCategory)
		{
			cache[num++] = new DefaultCollectionItem((int)playerRepository.itemIDToInventorySlotIndex[item.Key], item.Value);
		}
		NotifyOnCollectionChange();
	}
}
