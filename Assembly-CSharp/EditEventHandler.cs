using UnityEngine;
using UnityEngine.EventSystems;

public class EditEventHandler : MonoBehaviour, IEventSystemHandler
{
	public enum EventType
	{
		EditFace,
		EditEdge,
		EditCorner,
		Paint,
		Remove
	}

	public struct EditEvent
	{
		public Vector3 worldPosition;

		public EventType type;

		public EditEvent(EventType type, Vector3 worldPosition)
		{
			this.type = type;
			this.worldPosition = worldPosition;
		}

		public EditEvent(CubeModelingStateMachine.HoverType type, Vector3 worldPosition)
		{
			switch (type)
			{
			case CubeModelingStateMachine.HoverType.Corner:
				this.type = EventType.EditCorner;
				break;
			case CubeModelingStateMachine.HoverType.Edge:
				this.type = EventType.EditEdge;
				break;
			default:
				this.type = EventType.EditFace;
				break;
			}
			this.worldPosition = worldPosition;
		}
	}

	public void Notify(EditEvent e)
	{
		Debug.LogError(e.worldPosition);
		Debug.DrawLine(Vector3.zero, e.worldPosition);
	}
}
