using UnityEngine;
using UnityEngine.EventSystems;

public class TouristPromotionCrey : TouristPromotion
{
	[SerializeField]
	private GameObject adContinueButton;

	protected override void Start()
	{
		adContinueButton.SetActive(popupWithAd);
		StatHatWrapper.Count("TouristPromotion.CreyGames", 1);
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
		bool touristPromotionCreyRedirect = MVGameControllerBase.Game.CreySettings.TouristPromotionCreyRedirect;
		BrowserCommGotoRequests.GotoURL(MVGameControllerBase.Game.CreySettings.TouristPromotionCreyURL, !touristPromotionCreyRedirect);
	}

	public void CreyRedirect()
	{
		StatHatWrapper.Count("TouristPromotion.CreyGames.Redirect", 1);
		bool touristPromotionCreyRedirect = MVGameControllerBase.Game.CreySettings.TouristPromotionCreyRedirect;
		BrowserCommGotoRequests.GotoURL(MVGameControllerBase.Game.CreySettings.TouristPromotionCreyURL, !touristPromotionCreyRedirect);
	}
}
