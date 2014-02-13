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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.zero;
		Vector3 val2 = Vector3.up * 10f;
		List<KoGaMaPackageClient> list = new List<KoGaMaPackageClient>();
		foreach (KeyValuePair<int, MVItem> item in PlayerInventory)
		{
			KoGaMaPackageClient koGaMaPackageFromItem = ARepository.GetKoGaMaPackageFromItem(item.Value);
			koGaMaPackageFromItem.worldObjects[koGaMaPackageFromItem.worldObjectRoot].Visible = true;
			koGaMaPackageFromItem.worldObjects[koGaMaPackageFromItem.worldObjectRoot].Position = val;
			val += val2;
			list.Add(koGaMaPackageFromItem);
		}
	}
}
