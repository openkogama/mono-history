using UnityEngine;

public class BoostTouristInformationPopup : MonoBehaviour
{
	[SerializeField]
	private EmbeddedPlayerConfig embeddedPlayerConfig;

	public void SignUp()
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
