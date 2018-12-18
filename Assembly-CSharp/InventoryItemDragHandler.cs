using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemDragHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IEventSystemHandler
{
	[SerializeField]
	private CanvasGroup canvasGroup;

	private static bool dragging;

	public static bool dragRejected;

	public static bool Dragging => dragging;

	public void OnBeginDrag(PointerEventData eventData)
	{
		dragRejected = false;
		transform.SetParent(transform.root);
		canvasGroup.blocksRaycasts = false;
		EventSystem.current.SetSelectedGameObject(gameObject);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IPlayerInventory x, BaseEventData y) =>
		{
			x.SetCurrentDragTarget(gameObject);
		});
		dragging = true;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		canvasGroup.blocksRaycasts = true;
		dragging = false;
		if (transform.parent.GetComponent<InventorySlot>() == null || dragRejected)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IPlayerInventory x, BaseEventData y) =>
			{
				x.DragFailed();
			});
			Object.Destroy(gameObject);
			Object.Destroy(this);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		transform.position = eventData.position;
	}

	private void Update()
	{
		if (dragging)
		{
			MVInputWrapper.SuppressAllInput();
		}
	}
}
