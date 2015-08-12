public class ProductShopInfo
{
	public bool IsBuyable;

	public int PriceSilver;

	public int PriceGold;

	public bool IsRentable;

	public int RentPriceSilver;

	public int RentPriceGold;

	public int RentExpireSeconds;

	public override string ToString()
	{
		return "priceSilver: " + PriceSilver + ", rentPriceSilver: " + RentPriceSilver;
	}
}
