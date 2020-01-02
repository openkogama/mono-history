using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouristPromotionCrey : TouristPromotion
{
	[SerializeField]
	private Text redirectButtonText;

	[SerializeField]
	private GameObject noAdContinueButton;

	[SerializeField]
	private GameObject adContinueButton;

	protected override void Start()
	{
		Debug.LogWarning("dont run base.start when crey promotion is enabled, use streaming asset");
		base.Start();
		noAdContinueButton.SetActive(!popupWithAd);
		adContinueButton.SetActive(popupWithAd);
		StatHatWrapper.Count("TouristPromotion.CreyGames", 1);
		Uri uri = new Uri(MVGameControllerBase.Game.TouristPromotionCreyURL);
		redirectButtonText.text = uri.Host.Replace("www.", string.Empty).ToUpper();
	}

	public void Continue()
	{
		if (popupWithAd)
		{
			StatHatWrapper.Count("TouristPromotion.CreyGames.ContinueAd", 1);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (ITouristAdController x, BaseEventData y) =>
			{
				x.ShowAd();
			});
		}
		else
		{
			StatHatWrapper.Count("TouristPromotion.CreyGames.Continue", 1);
			SkipCallback();
		}
	}

	public void Signup()
	{
		StatHatWrapper.Count("TouristPromotion.CreyGames.Signup", 1);
		bool touristPromotionCreyRedirect = MVGameControllerBase.Game.TouristPromotionCreyRedirect;
		BrowserCommGotoRequests.GotoURL(MVGameControllerBase.Game.TouristPromotionCreyURL, !touristPromotionCreyRedirect);
	}

	public void CreyRedirect()
	{
		StatHatWrapper.Count("TouristPromotion.CreyGames.Redirect", 1);
		bool touristPromotionCreyRedirect = MVGameControllerBase.Game.TouristPromotionCreyRedirect;
		BrowserCommGotoRequests.GotoURL(MVGameControllerBase.Game.TouristPromotionCreyURL, !touristPromotionCreyRedirect);
	}
}
