using System;
using UnityEngine;

public class ProductInventoryInfo<T> where T : ProductInfo
{
	private MVWorldObjectClient equippedOn;

	public int InventoryID { get; private set; }

	public T ProductInfo { get; private set; }

	public bool IsRented { get; private set; }

	public DateTime PurchaseTime { get; private set; }

	public bool IsEquipped => equippedOn != null;

	public MVWorldObjectClient EquippedOn
	{
		get
		{
			return equippedOn;
		}
		set
		{
			T productInfo = ProductInfo;
			if (productInfo.IsEquippable)
			{
				equippedOn = value;
			}
			else
			{
				Debug.LogError((object)("Trying to equip unequippable inventory item: " + ToString()));
			}
		}
	}

	public ProductInventoryInfo(int inventoryID, T productInfo, DateTime purchaseTime, bool isRented = false)
	{
		InventoryID = inventoryID;
		ProductInfo = productInfo;
		PurchaseTime = purchaseTime;
		IsRented = isRented;
	}

	public void Renew(DateTime purchaseTime, int rentExpireSeconds)
	{
		PurchaseTime = purchaseTime;
		ProductInfo.ShopInfo.RentExpireSeconds = rentExpireSeconds;
	}

	public override string ToString()
	{
		object[] array = new object[4] { "inventoryID: ", InventoryID, ", ", null };
		T productInfo = ProductInfo;
		array[3] = productInfo.ToString();
		return string.Concat(array);
	}
}
