using System;
using UnityEngine;

namespace Assets.Scripts.AdIntegration.Web;

public class AdSDKManager : IAdManager
{
	private Action<InterstitialAdResult> interstitialCallback;

	private Action<RewardedAdResult> rewardedCallback;

	private IAdSDK adSDK;

	private bool wasInitializedSuccessfully;

	public string RewardedAdNotAvailableText => TM._("Unable to display rewarded ad. Please try again later.");

	public TimeSpan TimeSinceLastAd => new TimeSpan(Math.Min(TimeSinceLastInterstitial.Ticks, TimeSinceLastRewarded.Ticks));

	public TimeSpan TimeSinceLastInterstitial => TimeSpan.MaxValue;

	public TimeSpan TimeSinceLastRewarded => TimeSpan.MaxValue;

	public bool ReadyForRewardedAdRequest => wasInitializedSuccessfully;

	public bool ReadyForInterstitialAdRequest => wasInitializedSuccessfully;

	public AdSDKManager(EmbeddedSite site)
	{
	}

	private void OnAdSDKInitReady()
	{
		if (!adSDK.TryInitialize())
		{
			Debug.LogError("AdSDKManager failed to init adSDK for site.");
			return;
		}
		Debug.Log("adSDK initialized");
		wasInitializedSuccessfully = true;
	}

	public void RequestInterstitial(Action<InterstitialAdResult> interstitialCallback, AdContext context)
	{
		Debug.Log("RequestInterstitial in adsdkmanager: " + context);
		if (!wasInitializedSuccessfully)
		{
			Debug.Log("wasInitializedSuccessfully is false.");
			interstitialCallback(InterstitialAdResult.ErrorInternal);
		}
		else
		{
			Debug.Log("adSDK.RequestInterstitial.");
			adSDK.ShowInterstitial(interstitialCallback);
		}
	}

	public void RequestRewardedAd(Action<RewardedAdResult> rewardedAdCallback, AdContext context)
	{
		Debug.Log("RequestRewardedAd in adsdkmanager: " + context);
		if (!wasInitializedSuccessfully)
		{
			Debug.Log("wasInitializedSuccessfully is false.");
			rewardedAdCallback(RewardedAdResult.ErrorInternal);
		}
		else
		{
			Debug.Log("adSDK.ShowRewardedAd.");
			adSDK.ShowRewardedAd(rewardedAdCallback);
		}
	}

	public void InitializeAdConfigSettings(AdConfigSettings config)
	{
		Debug.Log("AdSDKManager is used by the WebAdManager and shouldn't be initialized in this way as of now");
		throw new NotImplementedException();
	}

	public void InitializeCallbackManager(IAdUIManager handler)
	{
		Debug.Log("AdSDKManager is used by the WebAdManager and shouldn't rely on the ui handler itself. the web ad manager handles this");
		throw new NotImplementedException();
	}
}
