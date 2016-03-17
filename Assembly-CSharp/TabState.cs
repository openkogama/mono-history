using UnityEngine;

public class TabState
{
	private int slotsPrPage;

	public readonly string name;

	public int highestSlotIndex;

	public int currentPage = 1;

	public int MaxPages => Mathf.CeilToInt((float)highestSlotIndex / (float)slotsPrPage);

	public int[] SlotRange => new int[2]
	{
		(currentPage - 1) * slotsPrPage,
		currentPage * slotsPrPage
	};

	public TabState(string name, int slotsPrPage)
	{
		this.slotsPrPage = slotsPrPage;
		this.name = name;
	}

	public bool UpdatePage(int pageDir)
	{
		if (pageDir == -1 && currentPage == 1)
		{
			return false;
		}
		if (pageDir == 1 && currentPage == MaxPages)
		{
			return false;
		}
		currentPage += pageDir;
		return true;
	}

	public bool SlotIndexIsInRange(int slotIndex)
	{
		int[] slotRange = SlotRange;
		if (slotIndex >= slotRange[0] && slotIndex < slotRange[1])
		{
			return true;
		}
		return false;
	}
}
