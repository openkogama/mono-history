using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInventoryDropToGameHandler : MonoBehaviour, IEventSystemHandler, IDropHandler
{
	public void OnDrop(PointerEventData eventData)
	{
		PlayerInventoryPreviewItem component = eventData.selectedObject.GetComponent<PlayerInventoryPreviewItem>();
		component.SlotPressed();
	}
}
