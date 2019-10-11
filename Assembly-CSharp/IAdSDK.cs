using System;
using Assets.Scripts.AdIntegration;

public interface IAdSDK
{
	bool TryInitialize();

	void ShowInterstitial(Action<InterstitialAdResult> onAdFinished);

	void ShowRewardedAd(Action<RewardedAdResult> onAdFinished);
}
