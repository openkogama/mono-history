using System;
using UnityEngine;

namespace Assets.Scripts.AdIntegration.Dummy;

public class DummyAdManager : IAdManager
{
	public TimeSpan TimeSinceLastAd => TimeSpan.MaxValue;

	public TimeSpan TimeSinceLastInterstitial => TimeSpan.MaxValue;

	public TimeSpan TimeSinceLastRewarded => TimeSpan.MaxValue;

	public bool ReadyForRewardedAdRequest => false;

	public bool ReadyForInterstitialAdRequest => false;

	public void RequestRewardedAd(Action<RewardedAdResult> rewardedAdCallback, string context)
	{
		Debug.LogErrorFormat("Requesting ad from dummy manager will always return {0}. Context: {1}.", RewardedAdResult.ErrorClient, context);
		rewardedAdCallback(RewardedAdResult.ErrorClient);
	}

	public void RequestInterstitial(Action<InterstitialAdResult> interstitialCallback, string context)
	{
		Debug.LogErrorFormat("Requesting ad from dummy manager will always return {0}. Context: {1}.", InterstitialAdResult.ErrorClient, context);
		interstitialCallback(InterstitialAdResult.ErrorClient);
	}
}
