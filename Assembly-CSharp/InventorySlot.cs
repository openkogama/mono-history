using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IEventSystemHandler, IDropHandler
{
	private int absoluteSlotValue;

	[SerializeField]
	private NotificationFade fade;

	private GameObject previewItem;

	public int AbsoluteSlot => absoluteSlotValue;

	public GameObject Item => previewItem;

	public void Clear()
	{
		Object.Destroy(Item);
	}

	public void UpdateAbsoluteSlotValue(int slotValue)
	{
		absoluteSlotValue = slotValue;
	}

	public void HighlightSlot()
	{
		fade.Activate();
	}

	public void Set(GameObject item)
	{
		if (!(item == null))
		{
			previewItem = item;
			item.transform.SetParent(transform, worldPositionStays: false);
			item.transform.SetAsFirstSibling();
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
