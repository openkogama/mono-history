using UnityEngine;

public class AdOfferGold : MonoBehaviour
{
	public void Initialize()
	{
		AdRequestHandler.GetGoldAdAvailable(RewardAvailable);
	}

	private void RewardAvailable(bool available)
	{
		gameObject.SetActive(available);
	}

	public void OnClick()
	{
		gameObject.SetActive(value: false);
		AdRequestHandler.GetGoldAdAvailable(OnTryClickGoldAd);
	}

	private void OnTryClickGoldAd(bool available)
	{
		if (available)
		{
			AdRequestHandler.ShowGoldVideoAd(OnShownGoldAd);
		}
	}

	public static void OnShownGoldAd(bool shouldReward)
	{
		if (shouldReward)
		{
			BrowserComm.ToJavaScript.ExternalCall("refreshCredentials");
			NotificationController.PushNotification(TM._("Thank you for watching! Enjoy your gold!"));
			Debug.LogWarning("State notification: This should be handle by the avatar it self");
			Debug.LogError("Not implemented");
		}
	}
}
