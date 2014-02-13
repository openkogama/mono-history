using System;
using MV.Common;

[Serializable]
public class ProductInfo
{
	public MVProductType ProductType;

	public int ProductID;

	public string Name;

	public string Desc;

	public ProductShopInfo ShopInfo;

	public virtual bool IsEquippable => false;

	public override string ToString()
	{
		return "productID: " + ProductID + ", name: " + Name + ", " + ((ShopInfo != null) ? ShopInfo.ToString() : string.Empty);
	}
}
