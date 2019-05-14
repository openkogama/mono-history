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
