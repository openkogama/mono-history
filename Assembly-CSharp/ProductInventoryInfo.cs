using System;
using UnityEngine;

public class ProductInventoryInfo
{
	private MVWorldObjectClient equippedOn;

	public int InventoryID { get; private set; }

	public StreamingAssetInfo ProductInfo { get; private set; }

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
			if (ProductInfo.IsEquippable)
			{
				equippedOn = value;
			}
			else
			{
				Debug.LogError("Trying to equip unequippable inventory item: " + ToString());
			}
		}
	}

	public ProductInventoryInfo(int inventoryID, StreamingAssetInfo productInfo, DateTime purchaseTime, bool isRented = false)
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
		return "inventoryID: " + InventoryID + ", " + ProductInfo.ToString();
	}
}
