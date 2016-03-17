using UnityEngine;
using UnityEngine.EventSystems;

public class EnterPlayModeButton : MonoBehaviour
{
	public void Execute()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ISetEditState x, BaseEventData y) =>
		{
			x.SetState(EditorEvent.ESWalkMode);
		});
	}
}
