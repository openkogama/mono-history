using MV.Common;

namespace MV.WorldObject.Accessories;

public class AccessoryData
{
	public int accessoryMetaDataID;

	public int streamingAssetID;

	public bool isAvailable;

	public bool isNew;

	public bool isFeatured;

	public int priceGold;

	public int discount;

	public int level;

	public string name;

	public AccessoryCategory category;

	public int position;

	public string url;

	public bool owns;

	public AccessorySlotType accessorySlotType;

	public AccessoryTimelimit timelimit = new AccessoryTimelimit();

	public int DiscountedPrice => priceGold - priceGold * discount / 100;

	public override string ToString()
	{
		return $"{name}:\r\n  isAvailable {isAvailable}\r\n  isNew {isNew}\r\n  isFeatured {isFeatured}\r\n  priceGold {priceGold}\r\n  discount {discount}\r\n  level {level}\r\n  category {category}\r\n  position {position}\r\n  url {url}\r\n  accessorySlotType {accessorySlotType}";
	}

	public bool GetShowInShop()
	{
		bool flag = true;
		if (timelimit.IsTimeLimited)
		{
			flag = timelimit.GetHasTimeLeft();
		}
		if (flag)
		{
			return isAvailable;
		}
		return false;
	}
}
