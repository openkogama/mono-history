using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouristPromotionDesktop : TouristPromotion
{
	[SerializeField]
	private GameObject visitKogamaPopupPrefab;

	[SerializeField]
	private GameObject redirectButton;

	[SerializeField]
	private Text redirectButtonURLText;

	[SerializeField]
	private GameObject goToKogamaPopupPrefab;

	[SerializeField]
	private GameObject signupButton;

	[SerializeField]
	private EmbeddedPlayerConfig embeddedPlayerConfig;

	protected override void Start()
	{
		EmbeddedSiteConfigData currentSiteData = embeddedPlayerConfig.GetCurrentSiteData();
		embedded = currentSiteData.siteEnum != EmbeddedSite.None;
		base.Start();
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
		Debug.Log("Referrer: " + MVGameControllerBase.GameSessionData.referrer);
		redirectButton.SetActive(MVGameControllerBase.GameSessionData.embedded);
		signupButton.SetActive(currentSiteData.allowsRedirectToWebpage || currentSiteData.allowsOpenInNewTab || currentSiteData.allowsModals);
		Uri uri = new Uri(MVGameControllerBase.Game.KogamaMainpageURL);
		redirectButtonURLText.text = uri.Host.Replace("www.", string.Empty).ToUpper();
	}

	public void SignupCallback()
	{
		StatHatWrapper.Count("TouristPromotion.Kogama.Signup", 1);
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

	public void LoginCallback()
	{
		BrowserCommGotoRequests.GotoLogin(newTab: false, modalPopup: true);
	}

	public void KogamaRedirect()
	{
		if (MVGameControllerBase.Game.LocalPlayer.IsTourist)
		{
			if (MVGameControllerBase.GameSessionData.GetIsRedirectAllowed() && (embeddedPlayerConfig.GetCurrentSiteData().allowsRedirectToWebpage || embeddedPlayerConfig.GetCurrentSiteData().allowsOpenInNewTab))
			{
				StatHatWrapper.Count("TouristPromotion.Kogama.Redirect", 1);
				BrowserCommGotoRequests.GotoMainpage(!embeddedPlayerConfig.GetCurrentSiteData().allowsRedirectToWebpage);
			}
			else
			{
				ShowGoToKogamaPopup();
			}
		}
	}

	public override void OnContinueClicked()
	{
		StatHatWrapper.Count("TouristPromotion.Kogama.Continue", 1);
		base.OnContinueClicked();
	}

	private void ShowGoToKogamaPopup()
	{
		GameObject popUp = UnityEngine.Object.Instantiate(goToKogamaPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popUp, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
	}

	private void OnWinningConditionFulfilled(IWinningCondition winningCondition)
	{
		MVGameControllerDesktop.LockCursorManager.CursorLock = false;
	}

	private void OnDestroy()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
	}
}
