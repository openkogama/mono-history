using UnityEngine.EventSystems;

public class TouristAdPopup : TouristPromotion
{
	public void OnViewAdClicked()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	public void SignupCallback()
	{
		BrowserCommGotoRequests.GotoSignup(newTab: false, modalPopup: true);
	}

	public void LoginCallback()
	{
		BrowserCommGotoRequests.GotoLogin(newTab: false, modalPopup: true);
	}
}
