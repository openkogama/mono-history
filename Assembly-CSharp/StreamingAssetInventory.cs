using System.Collections.Generic;
using System.Linq;
using MV.Common;

public class StreamingAssetInventory : ProductInventory<StreamingAssetInfo>
{
	private static MVWorldObjectClientManager WOCM => MVGameController.Instance.WOCM;

	public StreamingAssetInventory(InventoryExpirationChecker expirationChecker)
		: base(expirationChecker)
	{
	}

	protected override void OnAdded(ProductInventoryInfo<StreamingAssetInfo> invInfo)
	{
		base.OnAdded(invInfo);
		MVGameController.Instance.Game.AssetBundleMgr.RequestAssetBundle(invInfo.ProductInfo.RequestPath, null, autoRetry: true, highPriority: false);
	}

	public IEnumerable<ProductInventoryInfo<StreamingAssetInfo>> Get(StreamingAssetType type)
	{
		return Inventory.Values.Where((ProductInventoryInfo<StreamingAssetInfo> invInfo) =>
		{
			StreamingAssetInfo productInfo = invInfo.ProductInfo;
			return productInfo != null && productInfo.StreamedAssetType == type;
		});
	}
}
