using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IEventSystemHandler
{
	private Vector3 startPos;

	private static bool dragging;

	[SerializeField]
	private CanvasGroup canvasGroup;

	public static bool Dragging => dragging;

	public void OnBeginDrag(PointerEventData eventData)
	{
		transform.SetParent(transform.root);
		startPos = transform.position;
		canvasGroup.blocksRaycasts = false;
		EventSystem.current.SetSelectedGameObject(gameObject);
		dragging = true;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		canvasGroup.blocksRaycasts = true;
		if (transform.parent == transform.root)
		{
			transform.position = startPos;
		}
		dragging = false;
	}

	private void Update()
	{
		if (dragging)
		{
			MVInputWrapper.SuppressAllInput();
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		transform.position = eventData.position;
	}
}
