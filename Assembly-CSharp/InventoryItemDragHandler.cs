using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemDragHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IEventSystemHandler
{
	private static bool dragging;

	[SerializeField]
	private CanvasGroup canvasGroup;

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
		Debug.Log("OnEndDrag");
		Debug.Log(transform.parent.name);
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
			MVInputWrapper.IsInputSuppressed = true;
		}
	}
}
