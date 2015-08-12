using System.Collections.Generic;
using System.Linq;
using MV.Common;

public class StreamingAssetInventory : ProductInventory
{
	private static MVWorldObjectClientManager WOCM => MVGameController.WOCM;

	public StreamingAssetInventory(InventoryExpirationChecker expirationChecker)
		: base(expirationChecker)
	{
	}

	protected override void OnAdded(ProductInventoryInfo invInfo)
	{
		base.OnAdded(invInfo);
		AsyncWWWManager.WWWRequest(new StreamingAssetRequest(Urls.StreamingAssets + invInfo.ProductInfo.RequestPath, null));
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
