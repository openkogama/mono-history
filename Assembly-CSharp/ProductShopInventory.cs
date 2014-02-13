using System.Collections.Generic;
using UnityEngine;

public class ProductShopInventory<TProduct> where TProduct : ProductInfo
{
	public delegate void OnProductShopInventoryChangeDelegate(ProductShopInventory<TProduct> productShopInventory);

	public OnProductShopInventoryChangeDelegate OnProductShopInventoryChange;

	protected Dictionary<int, TProduct> Inventory = new Dictionary<int, TProduct>();

	public int Count => Inventory.Count;

	public bool Contains(int productID)
	{
		return Inventory.ContainsKey(productID);
	}

	public void Add(TProduct node)
	{
		if (node.ShopInfo == null)
		{
			Debug.LogError((object)"Trying to add product without shop info to the shop invventory");
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
