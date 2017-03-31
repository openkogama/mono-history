using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlots : MonoBehaviour, IEventSystemHandler, IGameObjectDroppedInSlot
{
	private Dictionary<int, InventorySlot> inventorySlots = new Dictionary<int, InventorySlot>();

	[SerializeField]
	private InventorySlot inventorySlotPrefab;

	public void Initialize(int numberOfSlots)
	{
		for (int i = 0; i < numberOfSlots; i++)
		{
			InventorySlot inventorySlot = Object.Instantiate(inventorySlotPrefab);
			inventorySlot.transform.SetParent(transform, worldPositionStays: false);
			inventorySlots.Add(i, inventorySlot);
		}
	}

	public void HighlightSlot(int slotPosition)
	{
		inventorySlots[slotPosition].HighlightSlot();
	}

	public void Clear()
	{
		foreach (InventorySlot value in inventorySlots.Values)
		{
			value.Clear();
		}
	}

	public void UpdateAbsoluteSlotValues(int page, int numberOfSlots)
	{
		foreach (KeyValuePair<int, InventorySlot> inventorySlot in inventorySlots)
		{
			inventorySlot.Value.UpdateAbsoluteSlotValue(inventorySlot.Key + numberOfSlots * (page - 1));
		}
	}

	public void AddItem(GameObject item, int slotIndex)
	{
		inventorySlots[slotIndex].Set(item);
	}

	public void SlotChanged(GameObject draggedItem, int toSlotIndex)
	{
		int fromSlotIndex = draggedItem.GetComponent<InventoryItemMetaData>().SlotIndex;
		InventorySlot slotBasedOnAbsolute = GetSlotBasedOnAbsolute(toSlotIndex);
		GameObject item = slotBasedOnAbsolute.Item;
		InventorySlot slotBasedOnAbsolute2 = GetSlotBasedOnAbsolute(fromSlotIndex);
		InventoryItemMetaData component = draggedItem.GetComponent<InventoryItemMetaData>();
		component.Initialize(toSlotIndex);
		if (item != null)
		{
			InventoryItemMetaData component2 = item.GetComponent<InventoryItemMetaData>();
			component2.Initialize(fromSlotIndex);
		}
		if (slotBasedOnAbsolute2 == null)
		{
			Object.Destroy(item);
		}
		else
		{
			slotBasedOnAbsolute2.Set(item);
		}
		slotBasedOnAbsolute.Set(draggedItem);
		ExecuteEvents.ExecuteHierarchy(gameObject.transform.parent.gameObject, null, (ISlotChanged x, BaseEventData y) =>
		{
			x.SlotChanged(fromSlotIndex, toSlotIndex);
		});
	}

	private InventorySlot GetSlotBasedOnAbsolute(int absoluteSlotIndex)
	{
		foreach (KeyValuePair<int, InventorySlot> inventorySlot in inventorySlots)
		{
			if (inventorySlot.Value.AbsoluteSlot == absoluteSlotIndex)
			{
				return inventorySlot.Value;
			}
		}
		return null;
	}
}
