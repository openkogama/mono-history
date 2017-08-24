using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeRequestEvaluateElementsDelayed : MonoBehaviour
{
	private void OnEnable()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IFirstTimeElementActivator x, BaseEventData y) =>
		{
			x.RequestEvaluateActivatableElements();
		});
	}
}
