using System;
using MV.Common;

public class ProductRenewedEventArgs : EventArgs
{
	public readonly MVProductType ProductType;

	public readonly int InvetoryID;

	public readonly ProductExpirationState State;

	public readonly DateTime PurchaseTime;

	public readonly int RentExpireSeconds;

	public ProductRenewedEventArgs(MVProductType productType, int invetoryID, ProductExpirationState state, DateTime purchaseTime, int rentExpireSeconds)
	{
		ProductType = productType;
		InvetoryID = invetoryID;
		State = state;
		PurchaseTime = purchaseTime;
		RentExpireSeconds = rentExpireSeconds;
	}
}
