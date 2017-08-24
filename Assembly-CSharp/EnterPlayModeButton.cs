using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnterPlayModeButton : MonoBehaviour
{
	public Action enteringPlayMode;

	public void Execute()
	{
		if (enteringPlayMode != null)
		{
			enteringPlayMode();
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ISetEditState x, BaseEventData y) =>
		{
			x.SetState(EditorEvent.ESWalkMode);
		});
	}
}
