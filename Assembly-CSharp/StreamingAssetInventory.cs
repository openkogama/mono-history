using System.Collections.Generic;
using System.Linq;
using MV.Common;

public class StreamingAssetInventory : ProductInventory
{
	public IEnumerable<ProductInventoryInfo> Get(StreamingAssetType type)
	{
		return Inventory.Values.Where((ProductInventoryInfo invInfo) =>
		{
			StreamingAssetInfo productInfo = invInfo.ProductInfo;
			return productInfo != null && productInfo.StreamedAssetType == type;
		});
	}
}
