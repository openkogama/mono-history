using System;
using Assets.Scripts.AdIntegration;
using UnityEngine;

public class TouristAdStateHandler
{
	private static readonly float adRateLimitTimer = 120f;

	private float adRateLimitCurrentTime;

	private Action OnAdShown;

	public void ShowAd(Action OnAdFinished)
	{
		OnAdShown = OnAdFinished;
		if (MVGameControllerBase.AdManager.ReadyForInterstitialAdRequest && Time.time - adRateLimitCurrentTime >= adRateLimitTimer)
		{
			Debug.Log("showVideoAd");
			MVGameControllerBase.AdManager.RequestInterstitial(InterstitialCallback, "TouristPromotion");
		}
		else if (OnAdShown != null)
		{
			OnAdShown();
		}
	}

	private void InterstitialCallback(InterstitialAdResult obj)
	{
		adRateLimitCurrentTime = Time.time;
		OnAdShown();
	}
}
