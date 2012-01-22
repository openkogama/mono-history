using UnityEngine;

[AddComponentMenu("UX/Handlers/Drag object")]
public class UXDragObject : MonoBehaviour
{
	public delegate bool OnDragStartDelegate(Vector3 position);

	public delegate void OnDragDelegate(Vector3 position);

	public delegate void OnDragStopDelegate(Vector3 position, bool isDrop);

	public OnDragStartDelegate OnDragStart;

	public OnDragDelegate OnDrag;

	public OnDragStopDelegate OnDragStop;
}
