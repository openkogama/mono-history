using UnityEngine.EventSystems;

public class TouristAdPopup : TouristPromotionDesktop
{
	public void OnViewAdClicked()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}
}
