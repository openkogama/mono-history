using System;
using MV.Common;
using UnityEngine;

public class InventoryExpirationInfo
{
	private DateTime purchaseTime;

	private int rentExpireSeconds;

	private DateTime rentExpireTime;

	private ProductExpirationState expirationState;

	private static MVNetworkGame Game => MVGameController.Game;

	public MVProductType ProductType { get; private set; }

	public int InventoryID { get; private set; }

	public DateTime PurchaseTime => purchaseTime;

	public int RentExpireSeconds => rentExpireSeconds;

	public DateTime RentExpireTime => rentExpireTime;

	public ProductExpirationState ExpirationState
	{
		get
		{
			return expirationState;
		}
		private set
		{
			if (expirationState != value)
			{
				if (expirationState == ProductExpirationState.ExpiredNotRenewed)
				{
					Debug.LogError(string.Concat("Changing expiration state of ", InventoryID, " to ", value, " when already already expired permanently"));
				}
				ProductExpirationState oldState = expirationState;
				expirationState = value;
				if (ExpirationStateChanged != null)
				{
					ExpirationStateChangeEventArgs e = new ExpirationStateChangeEventArgs(oldState, expirationState);
					ExpirationStateChanged(this, e);
				}
			}
		}
	}

	public TimeSpan TimeBeforeExpire
	{
		get
		{
			if (expirationState == ProductExpirationState.Expired || expirationState == ProductExpirationState.ExpiredNotRenewed)
			{
				return new TimeSpan(0L);
			}
			long num = rentExpireTime.Ticks - Game.DBTime.Ticks;
			if (num < 0)
			{
				num = 0L;
			}
			return TimeSpan.FromTicks(num);
		}
	}

	public bool IsExpiring => expirationState == ProductExpirationState.Expiring;

	public bool IsExpired => expirationState == ProductExpirationState.Expired || expirationState == ProductExpirationState.ExpiredNotRenewed;

	public bool IsExpiredNotRenewed => expirationState == ProductExpirationState.ExpiredNotRenewed;

	public event EventHandler<ProductRenewedEventArgs> Renewed;

	public event EventHandler<ExpirationStateChangeEventArgs> ExpirationStateChanged;

	public InventoryExpirationInfo(MVProductType productType, int inventoryID, ProductExpirationState expirationState, DateTime purchaseTime, int rentExpireSeconds)
	{
		ProductType = productType;
		InventoryID = inventoryID;
		this.expirationState = expirationState;
		this.purchaseTime = purchaseTime;
		this.rentExpireSeconds = rentExpireSeconds;
		rentExpireTime = purchaseTime.AddSeconds(rentExpireSeconds);
	}

	public override string ToString()
	{
		return $"Product: {ProductType}\nInventoryID: {InventoryID}\nPurchaseTime: {PurchaseTime}\nExpireSec: {RentExpireSeconds}\nExpirationState: {ExpirationState}";
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (!(obj is InventoryExpirationInfo inventoryExpirationInfo))
		{
			return false;
		}
		return ProductType == inventoryExpirationInfo.ProductType && InventoryID == inventoryExpirationInfo.InventoryID && PurchaseTime == inventoryExpirationInfo.PurchaseTime && rentExpireSeconds == inventoryExpirationInfo.rentExpireSeconds;
	}

	public override int GetHashCode()
	{
		return 31 * InventoryID * 13 * (int)ProductType + rentExpireSeconds;
	}

	public void CheckExpiration()
	{
		if (expirationState != ProductExpirationState.Expired && expirationState != ProductExpirationState.ExpiredNotRenewed && TimeBeforeExpire.Ticks <= 0)
		{
			Debug.LogWarning("ExpInfo " + InventoryID + " expired");
			ExpirationState = ProductExpirationState.Expired;
		}
	}

	public void Renew(DateTime purchaseTime, int rentExpireSeconds)
	{
		this.purchaseTime = purchaseTime;
		this.rentExpireSeconds = rentExpireSeconds;
		ExpirationState = ProductExpirationState.Expiring;
	}

	public void ExpirePermanently()
	{
		ExpirationState = ProductExpirationState.ExpiredNotRenewed;
	}
}
