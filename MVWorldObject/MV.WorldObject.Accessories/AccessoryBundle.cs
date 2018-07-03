using System.Collections.Generic;

namespace MV.WorldObject.Accessories;

public class AccessoryBundle
{
	public int accessoryBundleID = -1;

	public List<AccessoryBundleItem> accessoryBundleItems = new List<AccessoryBundleItem>();

	public int discount;

	public int level;

	public string name;

	public bool isAvailable;

	public AccessoryTimelimit timelimit = new AccessoryTimelimit();

	public bool IsEmptyBundle => accessoryBundleItems.Count == 0;

	public bool IsTimeLimited
	{
		get
		{
			if (timelimit.IsTimeLimited)
			{
				return true;
			}
			return false;
		}
	}

	public bool GetShowInShop()
	{
		bool flag = true;
		if (IsTimeLimited)
		{
			flag = timelimit.GetHasTimeLeft();
		}
		if (flag)
		{
			return isAvailable;
		}
		return false;
	}

	public override string ToString()
	{
		return $"{name}. IsAvailable: {isAvailable} Discount: {discount} Level: {level} ";
	}
}
