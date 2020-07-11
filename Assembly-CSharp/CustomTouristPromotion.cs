using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CustomTouristPromotion : TouristPromotion
{
	[SerializeField]
	private GameObject adContinueButton;

	[SerializeField]
	private Image graphics;

	protected override void Start()
	{
		Debug.Log("Custom promotion: Start()");
		adContinueButton.SetActive(promotionShowsAd);
		string path = Urls.StreamingAssets + MVGameControllerBase.Game.CustomTouristPromotionSettings.AssetURL;
		AsyncWWWManager.WWWRequest(new CachedGetRequest(path, StreamingAssetCallback, WWWRequestPriority.WaitUntilSyncronizingIsDone));
		StatHatWrapper.Count("TouristPromotion.Custom", 1);
	}

	private void StreamingAssetCallback(UnityWebRequest www)
	{
		if (www == null || www.isNetworkError || www.isHttpError)
		{
			Debug.LogWarning("PNG get request callback failed for custom tourist promotion graphics: " + www.error);
			return;
		}
		byte[] data = www.downloadHandler.data;
		Texture2D texture2D = new Texture2D(2, 2);
		texture2D.LoadImage(data);
		graphics.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
	}

	public void Continue()
	{
		if (promotionShowsAd)
		{
			Debug.Log("Custom promotion: Continue() with ad");
			StatHatWrapper.Count("TouristPromotion.Custom.ContinueAd", 1);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (ITouristAdController x, BaseEventData y) =>
			{
				x.ShowAd();
			});
		}
		else
		{
			Debug.Log("Custom promotion: Continue() without ad");
			StatHatWrapper.Count("TouristPromotion.Custom.Continue", 1);
			OnContinueClicked();
		}
	}

	public void Signup()
	{
		Debug.Log("Custom promotion: Signup()");
		StatHatWrapper.Count("TouristPromotion.Custom.Signup", 1);
		bool redirect = MVGameControllerBase.Game.CustomTouristPromotionSettings.Redirect;
		BrowserCommGotoRequests.GotoURL(MVGameControllerBase.Game.CustomTouristPromotionSettings.URL, !redirect);
	}

	public void CustomRedirect()
	{
		Debug.Log("Custom promotion: CustomRedirect()");
		StatHatWrapper.Count("TouristPromotion.Custom.Redirect", 1);
		bool redirect = MVGameControllerBase.Game.CustomTouristPromotionSettings.Redirect;
		BrowserCommGotoRequests.GotoURL(MVGameControllerBase.Game.CustomTouristPromotionSettings.URL, !redirect);
	}
}
