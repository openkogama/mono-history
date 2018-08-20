using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInventoryDropToGameHandler : MonoBehaviour, IDropHandler, IEventSystemHandler
{
	public void OnDrop(PointerEventData eventData)
	{
		PlayerInventoryPreviewItem component = eventData.selectedObject.GetComponent<PlayerInventoryPreviewItem>();
		component.SlotPressed();
	}
}
