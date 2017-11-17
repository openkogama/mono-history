using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConsoleDragAndTapHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	public UnityAction OnClick;

	private bool dragging;

	private float dragStart;

	private const float minDragDurationForClick = 0.2f;

	private bool scrollingEnabled = true;

	[SerializeField]
	private ScrollRect scrollRect;

	public void OnChatModeClick()
	{
		if (OnClick != null)
		{
			OnClick();
		}
	}

	public void SetScrollingEnabled(bool scrollEnabled)
	{
		scrollingEnabled = scrollEnabled;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (scrollingEnabled)
		{
			scrollRect.OnDrag(eventData);
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		dragging = true;
		dragStart = Time.time;
		scrollRect.OnBeginDrag(eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		dragging = false;
		if (Time.time - dragStart < 0.2f)
		{
			OnChatModeClick();
		}
		scrollRect.OnEndDrag(eventData);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!dragging)
		{
			OnChatModeClick();
		}
	}
}
