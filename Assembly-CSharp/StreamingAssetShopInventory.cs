using System.Collections.Generic;
using System.Linq;
using MV.Common;

public class StreamingAssetShopInventory : ProductShopInventory<StreamingAssetInfo>
{
	public int GetCategoryIDFromAssetID(int assetID)
	{
		if (Inventory.ContainsKey(assetID))
		{
			return Inventory[assetID].CategoryID;
		}
		return -1;
	}

	public IEnumerable<StreamingAssetInfo> Get(StreamingAssetType type)
	{
		return Inventory.Values.Where((StreamingAssetInfo ai) => ai.StreamedAssetType == type);
	}

	public StreamingAssetInfo Get(int assetID)
	{
		return Inventory.Values.FirstOrDefault((StreamingAssetInfo ai) => ai.ProductID == assetID);
	}
}
