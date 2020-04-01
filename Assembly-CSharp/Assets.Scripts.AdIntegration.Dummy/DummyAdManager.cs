using System;
using UnityEngine;

namespace Assets.Scripts.AdIntegration.Dummy;

public class DummyAdManager : IAdManager, IUpdatecontrollerSubscriberUpdate, IUpdatecontrollerSubscriberBase
{
	private IAdUIManager adUIHandler;

	private float startTime;

	private float delay = 1.5f;

	private bool rewarded;

	private bool timeoutAsEnabled;

	private int timeoutSuccessDelay = 30;

	public string RewardedAdNotAvailableText => TM._("Ads not set up for this build target.");

	public TimeSpan TimeSinceLastAd => TimeSpan.MaxValue;

	public TimeSpan TimeSinceLastInterstitial => TimeSpan.MaxValue;

	public TimeSpan TimeSinceLastRewarded => TimeSpan.MaxValue;

	public bool ReadyForRewardedAdRequest => true;

	public bool ReadyForInterstitialAdRequest => true;

	public void InitializeAdConfigSettings(AdConfigSettings config)
	{
		timeoutAsEnabled = config.AdTimeoutAsSuccess;
		timeoutSuccessDelay = config.AdTimeoutAsSuccessDelay;
	}

	public void InitializeCallbackManager(IAdUIManager handler)
	{
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
		adUIHandler = handler;
	}

	public void RequestRewardedAd(Action<RewardedAdResult> rewardedAdCallback, AdContext context)
	{
		if (adUIHandler.AdShowing())
		{
			Debug.LogError("Ad showing already!");
			rewardedAdCallback(RewardedAdResult.ErrorClient);
		}
		else
		{
			startTime = Time.time;
			adUIHandler.ShowRewardedVideo(rewardedAdCallback);
			rewarded = true;
		}
	}

	public void RequestInterstitial(Action<InterstitialAdResult> interstitialCallback, AdContext context)
	{
		if (adUIHandler.AdShowing())
		{
			Debug.LogError("Ad showing already!");
			interstitialCallback(InterstitialAdResult.ErrorClient);
		}
		else
		{
			startTime = Time.time;
			adUIHandler.ShowInterstitial(interstitialCallback);
			rewarded = false;
		}
	}

	public void UpdateControllerUpdate()
	{
		if (adUIHandler.AdShowing() && Time.time > startTime + delay)
		{
			RewardedAdResult adResult = RewardedAdResult.RewardUnlocked;
			Debug.Log($"rewarded {rewarded}, timeoutAsEnabled {timeoutAsEnabled}, Time.time - startTime {Time.time - startTime}, timeoutSuccessDelay {timeoutSuccessDelay}");
			if (rewarded && timeoutAsEnabled && Time.time - startTime >= (float)timeoutSuccessDelay)
			{
				Debug.Log("timeout");
				adResult = RewardedAdResult.RewardUnlocked;
			}
			Debug.Log("ad finished");
			if (!rewarded)
			{
				adUIHandler.PopInterstitial(InterstitialAdResult.Done);
			}
			else
			{
				adUIHandler.PopRewardedVideo(adResult);
			}
		}
	}
}
