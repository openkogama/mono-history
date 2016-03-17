using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IEventSystemHandler, IDropHandler
{
	private int absoluteSlotValue;

	public int AbsoluteSlot => absoluteSlotValue;

	public GameObject Item
	{
		get
		{
			if (transform.childCount > 0)
			{
				return transform.GetChild(0).gameObject;
			}
			return null;
		}
	}

	public void Clear()
	{
		Object.Destroy(Item);
	}

	public void UpdateAbsoluteSlotValue(int slotValue)
	{
		absoluteSlotValue = slotValue;
	}

	public void Set(GameObject item)
	{
		if (!(item == null))
		{
			item.transform.SetParent(transform, worldPositionStays: false);
			item.GetComponent<RectTransform>().localPosition = Vector3.zero;
		}
	}

	public void OnDrop(PointerEventData eventData)
	{
		Debug.Log("From: " + eventData.selectedObject.GetComponent<InventoryItemMetaData>().SlotIndex + " To: " + absoluteSlotValue);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IGameObjectDroppedInSlot x, BaseEventData y) =>
		{
			x.SlotChanged(eventData.selectedObject, absoluteSlotValue);
		});
	}
}
