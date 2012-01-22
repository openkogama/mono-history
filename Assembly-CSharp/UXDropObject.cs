using UnityEngine;

[AddComponentMenu("UX/Handlers/Drop object")]
public class UXDropObject : MonoBehaviour
{
	public delegate void OnDragOverEnterDelegate(GameObject gameObject);

	public delegate void OnDragOverDelegate(GameObject gameObject);

	public delegate void OnDragOverExitDelegate(GameObject gameObject);

	public delegate bool AcceptDropDelegate(GameObject gameObject);

	public delegate void OnDropDelegate(GameObject gameObject);

	public OnDragOverEnterDelegate OnDragOverEnter;

	public OnDragOverDelegate OnDragOver;

	public OnDragOverExitDelegate OnDragOverExit;

	public AcceptDropDelegate AcceptDrop;

	public OnDropDelegate OnDrop;
}
