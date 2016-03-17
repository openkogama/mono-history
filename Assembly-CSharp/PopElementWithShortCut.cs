using UnityEngine;
using UnityEngine.EventSystems;

public class PopElementWithShortCut : MonoBehaviour
{
	[SerializeField]
	private KogamaControls kogamaControl;

	private void Update()
	{
		if (MVInputWrapper.GetBooleanControlUp(kogamaControl))
		{
			Pop();
		}
	}

	public void Pop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}
}
