using UnityEngine;
using UnityEngine.EventSystems;

public class PopElement : MonoBehaviour
{
	public void Pop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}
}
