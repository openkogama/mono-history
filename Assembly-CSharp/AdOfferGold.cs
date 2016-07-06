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

	private void OnShownGoldAd(bool shouldReward)
	{
		AdRequestHandler.GetGoldAdAvailable(RewardAvailable);
		if (!shouldReward)
		{
		}
	}
}
