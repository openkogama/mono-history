using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryTurnPage : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
{
	[SerializeField]
	private bool pageForward;

	[SerializeField]
	private Button button;

	private void Start()
	{
		button.onClick.AddListener(() =>
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IPagedTurned x, BaseEventData y) =>
			{
				TurnPage(x);
			});
		});
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (DragHandler.Dragging || InventoryItemDragHandler.Dragging)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IPagedTurned x, BaseEventData y) =>
			{
				TurnPage(x);
			});
		}
	}

	private void TurnPage(IPagedTurned pagedTurned)
	{
		int dir = -1;
		if (pageForward)
		{
			dir = 1;
		}
		pagedTurned.PageTurned(dir);
	}
}
