using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeRequestActivatableElementsEvaluation : MonoBehaviour
{
	private void OnEnable()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IFirstTimeElementActivator x, BaseEventData y) =>
		{
			x.RequestEvaluateActivatableElements();
		});
	}
}
