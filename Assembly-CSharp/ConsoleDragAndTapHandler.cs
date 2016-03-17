using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConsoleDragAndTapHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	public UnityAction OnClick;

	private bool dragging;

	[SerializeField]
	private ScrollRect scrollRect;

	public void OnDrag(PointerEventData eventData)
	{
		scrollRect.OnDrag(eventData);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		dragging = true;
		scrollRect.OnBeginDrag(eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		dragging = false;
		scrollRect.OnEndDrag(eventData);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!dragging && OnClick != null)
		{
			OnClick();
		}
	}
}
