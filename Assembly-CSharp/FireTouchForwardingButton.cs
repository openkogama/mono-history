using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class FireTouchForwardingButton : MonoBehaviour, IDragHandler, IPointerUpHandler, IEventSystemHandler
{
	private enum State
	{
		NotDragging,
		Dragging,
		ActivatedTouchPad
	}

	private const float degreesToActivateTouchPad = 1f;

	private Vector3 firstDragPos = Vector3.zero;

	private State state;

	[SerializeField]
	private TouchPadAbsolute touchPadAbsolute;

	public void OnPointerUp(PointerEventData eventData)
	{
		state = State.NotDragging;
		touchPadAbsolute.OnPointerUp(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		switch (state)
		{
		case State.NotDragging:
			firstDragPos = eventData.position;
			state = State.Dragging;
			break;
		case State.Dragging:
		{
			Vector3 direction = Camera.main.ScreenPointToRay(firstDragPos).direction;
			Vector3 direction2 = Camera.main.ScreenPointToRay(eventData.position).direction;
			if (Vector3.Angle(direction, direction2) >= 1f)
			{
				state = State.ActivatedTouchPad;
				touchPadAbsolute.OnPointerDown(eventData);
			}
			break;
		}
		}
	}
}
