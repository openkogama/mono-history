using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ToggleSnapToGrid : ToggleHandler
{
	public override void ExecuteToggleState(bool toggleState, UnityAction<bool> toggleCallback)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IGridSnapHandler handler, BaseEventData data) =>
		{
			handler.Set(toggleState);
		});
		toggleCallback(toggleState);
	}
}
