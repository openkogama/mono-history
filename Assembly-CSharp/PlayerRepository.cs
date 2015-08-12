using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public class PlayerRepository : ARepository
{
	private Dictionary<int, MVItem> playerInventory;

	public IDictionary<int, MVItem> PlayerInventory => playerInventory;

	public PlayerRepository()
	{
		playerInventory = new Dictionary<int, MVItem>();
	}

	public override void RemoveItem(int itemId)
	{
		playerInventory.Remove(itemId);
		base.RemoveItem(itemId);
	}

	public Dictionary<int, MVItem> GetItemsByItemCategory(int[] itemCategories)
	{
		return PlayerInventory.Where((KeyValuePair<int, MVItem> p) => itemCategories.Contains(p.Value.itemCategoryID)).ToDictionary((KeyValuePair<int, MVItem> pair) => pair.Key, (KeyValuePair<int, MVItem> pair) => pair.Value);
	}

	public int CountItemsWithOriginalID(int originalId)
	{
		if (originalId == 0)
		{
			return 1;
		}
		return PlayerInventory.Where((KeyValuePair<int, MVItem> p) => p.Value.originalItemID == originalId).Count();
	}

	public void CreateWorldObjectHierarchies()
	{
		Vector3 zero = Vector3.zero;
		Vector3 vector = Vector3.up * 10f;
		List<KoGaMaPackageClient> list = new List<KoGaMaPackageClient>();
		foreach (KeyValuePair<int, MVItem> item in PlayerInventory)
		{
			KoGaMaPackageClient koGaMaPackageFromItem = ARepository.GetKoGaMaPackageFromItem(item.Value);
			koGaMaPackageFromItem.worldObjects[koGaMaPackageFromItem.worldObjectRoot].Visible = true;
			koGaMaPackageFromItem.worldObjects[koGaMaPackageFromItem.worldObjectRoot].Position = zero;
			zero += vector;
			list.Add(koGaMaPackageFromItem);
		}
	}
}
