using System;
using UnityEngine;

namespace Assets.Scripts.AdIntegration.Web;

public class AdSDKManager : IAdManager
{
	private Action<InterstitialAdResult> interstitialCallback;

	private Action<RewardedAdResult> rewardedCallback;

	private IAdSDK adSDK;

	private bool wasInitializedSuccessfully;

	public TimeSpan TimeSinceLastAd => new TimeSpan(Math.Min(TimeSinceLastInterstitial.Ticks, TimeSinceLastRewarded.Ticks));

	public TimeSpan TimeSinceLastInterstitial => TimeSpan.MaxValue;

	public TimeSpan TimeSinceLastRewarded => TimeSpan.MaxValue;

	public bool ReadyForRewardedAdRequest => wasInitializedSuccessfully;

	public bool ReadyForInterstitialAdRequest => wasInitializedSuccessfully;

	public AdSDKManager(EmbeddedSite site)
	{
	}

	public void RequestInterstitial(Action<InterstitialAdResult> interstitialCallback, AdContext context)
	{
		if (!wasInitializedSuccessfully)
		{
			interstitialCallback(InterstitialAdResult.ErrorInternal);
		}
		else
		{
			adSDK.ShowInterstitial(interstitialCallback);
		}
	}

	public void RequestRewardedAd(Action<RewardedAdResult> rewardedAdCallback, AdContext context)
	{
		if (!wasInitializedSuccessfully)
		{
			rewardedAdCallback(RewardedAdResult.ErrorInternal);
		}
		else
		{
			adSDK.ShowRewardedAd(rewardedAdCallback);
		}
	}

	public void InitializeCallbackManager(IAdUIManager handler)
	{
		Debug.Log("AdSDKManager is used by the WebAdManager and shouldn't rely on the ui handler itself. the web ad manager handles this");
		throw new NotImplementedException();
	}
}
