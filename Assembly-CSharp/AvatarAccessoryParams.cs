public struct AvatarAccessoryParams(int inventoryID, string assetReqPath)
{
	public readonly int InventoryID = inventoryID;

	public readonly string AssetReqPath = assetReqPath;

	public readonly InventoryExpirationInfo ExpirationInfo = null;

	public override string ToString()
	{
		return $"{InventoryID}, {AssetReqPath}";
	}
}
