using UnityEngine;
using UnityEngine.EventSystems;

public class GizmoButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	[SerializeField]
	private GizmoAction gizmoAction;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IGizmoHandler handler, BaseEventData data) =>
			{
				handler.Handle(gizmoAction);
			});
		}
	}
}
