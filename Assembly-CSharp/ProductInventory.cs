using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductInventory
{
	public delegate void OnProductInventoryChangeDelegate(ProductInventory productInventory);

	public OnProductInventoryChangeDelegate OnProductInventoryChange;

	protected Dictionary<int, ProductInventoryInfo> Inventory = new Dictionary<int, ProductInventoryInfo>();

	private static MVWorldObjectClientManager WOCM => MVGameControllerBase.WOCM;

	public int Count => Inventory.Count;

	public bool Contains(int inventoryID)
	{
		return Inventory.ContainsKey(inventoryID);
	}

	public void Add(ProductInventoryInfo invInfo)
	{
		if (invInfo.ProductInfo == null)
		{
			Debug.LogError("Trying to add inventory info without product info to inventory.");
			return;
		}
		Inventory.Add(invInfo.InventoryID, invInfo);
		OnAdded(invInfo);
		NotifyProductInventoryChange();
	}

	protected virtual void OnAdded(ProductInventoryInfo invInfo)
	{
	}

	public ProductInventoryInfo Get(int inventoryID)
	{
		ProductInventoryInfo value = null;
		Inventory.TryGetValue(inventoryID, out value);
		return value;
	}

	public HashSet<int> GetIDs()
	{
		return new HashSet<int>(Inventory.Keys);
	}

	public void Remove(ProductInventoryInfo inventoryInfo)
	{
		Remove(inventoryInfo.InventoryID);
		NotifyProductInventoryChange();
	}

	public void Remove(int inventoryID)
	{
		Inventory.Remove(inventoryID);
		NotifyProductInventoryChange();
	}

	public IEnumerable<ProductInventoryInfo> Get(Func<ProductInventoryInfo, bool> predicate)
	{
		return Inventory.Values.Where(predicate);
	}

	public IEnumerable<ProductInventoryInfo> GetRented()
	{
		return Inventory.Values.Where((ProductInventoryInfo invInfo) => invInfo.IsRented);
	}

	public string BuildLogString(string prependMessage)
	{
		return Inventory.BuildString(prependMessage);
	}

	public void NotifyProductInventoryChange()
	{
		if (OnProductInventoryChange != null)
		{
			OnProductInventoryChange(this);
		}
	}
}
