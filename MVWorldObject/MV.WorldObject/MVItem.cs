namespace MV.WorldObject;

public class MVItem
{
	public int itemID;

	public int itemCategoryID;

	public int itemTypeID;

	public int originalItemID;

	public int marketPlaceItemID;

	public string name;

	public string description;

	public byte[] data;

	public bool resellable;

	public int authorProfileID;

	public int shopInventoryID;

	public int priceSilver;

	public int priceGold;

	public override string ToString()
	{
		return string.Format("Name: {0} ItemID: {1} ItemTypeID: {2}\nPriceSilver: {3} PriceGold: {4} Resellable: {5} \n", new object[6] { name, itemID, itemTypeID, priceSilver, priceGold, resellable });
	}
}
