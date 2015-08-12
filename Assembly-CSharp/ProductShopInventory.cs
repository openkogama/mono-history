using System.Collections.Generic;
using UnityEngine;

public class ProductShopInventory
{
	public delegate void OnProductShopInventoryChangeDelegate(ProductShopInventory productShopInventory);

	public OnProductShopInventoryChangeDelegate OnProductShopInventoryChange;

	protected Dictionary<int, StreamingAssetInfo> Inventory = new Dictionary<int, StreamingAssetInfo>();

	public int Count => Inventory.Count;

	public bool Contains(int productID)
	{
		return Inventory.ContainsKey(productID);
	}

	public void Add(StreamingAssetInfo node)
	{
		if (node.ShopInfo == null)
		{
			Debug.LogError("Trying to add product without shop info to the shop invventory");
		}
		else
		{
			Inventory.Add(node.ProductID, node);
		}
	}

	public void LogInventory(string prependMessage)
	{
		Inventory.LogRecursive(prependMessage);
	}

	public void NotifyProductShopInventoryChange()
	{
		if (OnProductShopInventoryChange != null)
		{
			OnProductShopInventoryChange(this);
		}
	}
}
