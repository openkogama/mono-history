using UnityEngine;
using UnityEngine.EventSystems;

public class GamePassesTouristInformationPopup : MonoBehaviour
{
	[SerializeField]
	private EmbeddedPlayerConfig embeddedPlayerConfig;

	public void Exit()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	public void OnSignupClicked()
	{
		EmbeddedSiteConfigData currentSiteData = embeddedPlayerConfig.GetCurrentSiteData();
		if (currentSiteData.allowsModals)
		{
			BrowserCommGotoRequests.GotoSignup(newTab: false, modalPopup: true);
			return;
		}
		if (currentSiteData.allowsOpenInNewTab)
		{
			BrowserCommGotoRequests.GotoSignup(newTab: true);
			return;
		}
		if (currentSiteData.allowsRedirectToWebpage)
		{
			BrowserCommGotoRequests.GotoSignup();
			return;
		}
		Debug.Log(currentSiteData.siteEnum);
		Debug.LogError("Signup not permitted for site.");
	}
}
