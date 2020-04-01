using UnityEngine;
using UnityEngine.EventSystems;

public class TouristSignupClickable : MonoBehaviour
{
	[SerializeField]
	private GameObject redirectNotAllowedPopup;

	[SerializeField]
	private EmbeddedPlayerConfig embeddedPlayerConfig;

	[SerializeField]
	private GameObject signupBtn;

	public void Start()
	{
		EmbeddedSiteConfigData currentSiteData = embeddedPlayerConfig.GetCurrentSiteData();
		signupBtn.SetActive(currentSiteData.allowsModals || currentSiteData.allowsOpenInNewTab || currentSiteData.allowsRedirectToWebpage);
	}

	public void OnClick()
	{
		if (MVGameControllerBase.GameSessionData.IsPlayedFromPoki)
		{
			GameObject popup = Object.Instantiate(redirectNotAllowedPopup);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popup, UIPushOption.Blocking, null, UIGroupFlags.Popup);
			});
		}
		else
		{
			BrowserCommGotoRequests.GotoSignup(newTab: false, modalPopup: true);
		}
	}
}
