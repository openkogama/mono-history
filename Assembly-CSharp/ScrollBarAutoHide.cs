using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollBarAutoHide : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IEventSystemHandler
{
	[SerializeField]
	private NotificationFade scrollBarFader;

	[SerializeField]
	private CanvasGroup scrollbarCanvasGroup;

	public void OnBeginDrag(PointerEventData eventData)
	{
		scrollbarCanvasGroup.alpha = 1f;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		scrollBarFader.Activate();
	}
}
