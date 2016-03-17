using UnityEngine;
using UnityEngine.EventSystems;

public class DragSuppress : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	private bool isDragging;

	private void Update()
	{
		if (isDragging)
		{
			MVInputWrapper.IsInputSuppressed = true;
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject.transform.parent.gameObject, eventData, (IBeginDragHandler handler, BaseEventData data) =>
		{
			handler.OnBeginDrag(eventData);
		});
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject.transform.parent.gameObject, eventData, (IEndDragHandler handler, BaseEventData data) =>
		{
			handler.OnEndDrag(eventData);
		});
	}

	public void OnDrag(PointerEventData eventData)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject.transform.parent.gameObject, eventData, (IDragHandler handler, BaseEventData data) =>
		{
			handler.OnDrag(eventData);
		});
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject.transform.parent.gameObject, eventData, (IPointerDownHandler handler, BaseEventData data) =>
		{
			handler.OnPointerDown(eventData);
		});
		isDragging = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject.transform.parent.gameObject, eventData, (IPointerUpHandler handler, BaseEventData data) =>
		{
			handler.OnPointerUp(eventData);
		});
		isDragging = false;
		EventSystem.current.SetSelectedGameObject(null);
	}
}
