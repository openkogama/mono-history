using UnityEngine;
using UnityEngine.EventSystems;

public class TouristAdPopup : MonoBehaviour
{
	public void OnViewAdClicked()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}
}
