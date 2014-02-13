using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ProductsExpiringEventArgs : EventArgs
{
	public readonly ReadOnlyCollection<InventoryExpirationInfo> ExpiringProducts;

	public ProductsExpiringEventArgs(List<InventoryExpirationInfo> expiringProducts)
	{
		ExpiringProducts = expiringProducts.AsReadOnly();
	}
}
