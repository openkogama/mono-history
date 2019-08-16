using System;

namespace Assets.Scripts.AdIntegration;

public interface IAdManager
{
	TimeSpan TimeSinceLastAd { get; }

	TimeSpan TimeSinceLastInterstitial { get; }

	TimeSpan TimeSinceLastRewarded { get; }

	bool ReadyForRewardedAdRequest { get; }

	bool ReadyForInterstitialAdRequest { get; }

	void RequestRewardedAd(Action<RewardedAdResult> rewardedAdCallback, string context);

	void RequestInterstitial(Action<InterstitialAdResult> interstitialCallback, string context);
}
