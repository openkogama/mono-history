public struct AvatarAccessoryParams(int inventoryID, string assetReqPath)
{
	public readonly int InventoryID = inventoryID;

	public readonly string AssetReqPath = assetReqPath;

	public override string ToString()
	{
		return $"{InventoryID}, {AssetReqPath}";
	}
}
