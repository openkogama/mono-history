using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class InventoryController : MonoBehaviour, IEventSystemHandler, ISlotChanged, IPagedTurned, ITabSelected
{
	private int numberOfSlots;

	[SerializeField]
	private TabMenu tabMenu;

	[SerializeField]
	private InventorySlots inventorySlots;

	public UnityAction<int> OnPageTurned;

	public UnityAction<int> OnTabSelected;

	public UnityAction<int, int> OnSlotChanged;

	public void Initialize(int numberOfSlots)
	{
		this.numberOfSlots = numberOfSlots;
		inventorySlots.Initialize(numberOfSlots);
	}

	public void Clear()
	{
		inventorySlots.Clear();
	}

	public void HighlightSlot(int slotPosition)
	{
		inventorySlots.HighlightSlot(slotPosition);
	}

	public void AddTab(int categoryId, string tabname)
	{
		tabMenu.AddTabMenuButton(categoryId, tabname);
	}

	public void SelectTab(int tabId, int currentPage, int maxPages)
	{
		tabMenu.SelectTab(tabId, currentPage, maxPages);
		inventorySlots.UpdateAbsoluteSlotValues(currentPage, numberOfSlots);
	}

	public void AddObject(GameObject item, int slotIndex)
	{
		inventorySlots.AddItem(item, slotIndex);
	}

	public void PageTurned(int dir)
	{
		if (OnPageTurned != null)
		{
			OnPageTurned(dir);
		}
	}

	public void TabSelected(int tabId)
	{
		if (OnTabSelected != null)
		{
			OnTabSelected(tabId);
		}
	}

	public void SlotChanged(int fromSlotIndex, int toSlotIndex)
	{
		if (OnSlotChanged != null)
		{
			OnSlotChanged(fromSlotIndex, toSlotIndex);
		}
	}

	public List<T> GetComponentsOfSlotsWithType<T>() where T : MonoBehaviour
	{
		List<T> list = new List<T>();
		foreach (KeyValuePair<int, InventorySlot> slot in inventorySlots.GetSlots())
		{
			if (!(slot.Value.Item == null))
			{
				T component = slot.Value.Item.GetComponent<T>();
				if (component != null)
				{
					list.Add(component);
				}
			}
		}
		return list;
	}
}
