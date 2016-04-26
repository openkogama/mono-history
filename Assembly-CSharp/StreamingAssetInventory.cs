using System.Collections.Generic;
using System.Linq;
using MV.Common;

public class StreamingAssetInventory : ProductInventory
{
	private static MVWorldObjectClientManager WOCM => MVGameControllerBase.WOCM;

	public StreamingAssetInventory(InventoryExpirationChecker expirationChecker)
		: base(expirationChecker)
	{
	}

	public IEnumerable<ProductInventoryInfo> Get(StreamingAssetType type)
	{
		return Inventory.Values.Where((ProductInventoryInfo invInfo) =>
		{
			StreamingAssetInfo productInfo = invInfo.ProductInfo;
			return productInfo != null && productInfo.StreamedAssetType == type;
		});
	}
}
