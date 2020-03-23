using MV.Common;

namespace MV.WorldObject.Accessories;

public class AccessoryData
{
	public int aMDID;

	public int sAID;

	public bool iAvlb;

	public bool iNew;

	public bool iFtr;

	public int cost;

	public int dsc;

	public int lvl;

	public string name;

	public AccessoryCategory cat;

	public int pos;

	public string url;

	public bool owns;

	public AccessorySlotType slot;

	public AccessoryTimelimit time = new AccessoryTimelimit();

	public int DiscountedPrice => cost - cost * dsc / 100;

	public override string ToString()
	{
		return $"{name}:\r\n  isAvailable {iAvlb}\r\n  isLimited {iNew}\r\n  isFeatured {iFtr}\r\n  priceGold {cost}\r\n  discount {dsc}\r\n  level {lvl}\r\n  category {cat}\r\n  position {pos}\r\n  url {url}\r\n  accessorySlotType {slot}";
	}

	public bool GetShowInShop()
	{
		bool flag = true;
		if (time.IsTimeLimited)
		{
			flag = time.GetHasTimeLeft();
		}
		if (flag)
		{
			return iAvlb;
		}
		return false;
	}
}
