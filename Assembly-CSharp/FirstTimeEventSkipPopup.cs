using MV.WorldObject.MetaData;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeEventSkipPopup : MonoBehaviour
{
	private FirstTimeActivatableElementBase targetElement;

	private FirstTimeEvent eventToSkip;

	public void Initialize(FirstTimeEvent firstTimeEvent, FirstTimeActivatableElementBase elementToSkip)
	{
		eventToSkip = firstTimeEvent;
		targetElement = elementToSkip;
	}

	public void Ok()
	{
		FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent.SkipEvent);
		FirstTimeEventManager.SetFirstTimeEvent(eventToSkip);
		Object.Destroy(targetElement);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.Popup);
		});
	}

	public void Cancel()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}
}
