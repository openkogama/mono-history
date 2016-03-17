using UnityEngine;
using UnityEngine.EventSystems;

public class ActivateEvent : MonoBehaviour
{
	[SerializeField]
	private ActivateUIElement activateTarget;

	public void Activate()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IActivateUIElement x, BaseEventData y) =>
		{
			x.Activate(activateTarget);
		});
	}
}
