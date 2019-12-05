using System;
using UnityEngine;

namespace Assets.Scripts.AdIntegration.Dummy;

public class DummyAdManager : IAdManager
{
	public string RewardedAdNotAvailableText => TM._("Ads not set up for this build target.");

	public TimeSpan TimeSinceLastAd => TimeSpan.MaxValue;

	public TimeSpan TimeSinceLastInterstitial => TimeSpan.MaxValue;

	public TimeSpan TimeSinceLastRewarded => TimeSpan.MaxValue;

	public bool ReadyForRewardedAdRequest => false;

	public bool ReadyForInterstitialAdRequest => false;

	public void InitializeCallbackManager(IAdUIManager handler)
	{
	}

	public void RequestRewardedAd(Action<RewardedAdResult> rewardedAdCallback, AdContext context)
	{
		Debug.LogErrorFormat("Requesting ad from dummy manager will always return {0}. Context: {1}.", RewardedAdResult.ErrorClient, context.ToString());
		rewardedAdCallback(RewardedAdResult.ErrorClient);
	}

	public void RequestInterstitial(Action<InterstitialAdResult> interstitialCallback, AdContext context)
	{
		Debug.LogErrorFormat("Requesting ad from dummy manager will always return {0}. Context: {1}.", InterstitialAdResult.ErrorClient, context.ToString());
		interstitialCallback(InterstitialAdResult.ErrorClient);
	}
}
