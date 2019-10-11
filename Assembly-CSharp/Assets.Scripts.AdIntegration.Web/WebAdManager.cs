using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.AdIntegration.Web;

public class WebAdManager : IAdManager, IUpdatecontrollerSubscriberUpdate, IUpdatecontrollerSubscriberBase
{
	private struct JSONAdReturnedData
	{
		public bool adAvailable;
	}

	private struct JSONRewardedAdSuccessful
	{
		public bool status;
	}

	private bool webReturnedAvailabilityInterstitial;

	private bool webReturnedAvailabilityRewardedAd;

	private static readonly float refreshTimer = 60f;

	private float updateTime = 0f - refreshTimer;

	private DateTime prevInterstitialTime = DateTime.MinValue;

	private bool showingAd;

	private IAdUIManager adUIManager;

	private IAdManager sdkManager;

	private bool embeddedSiteSDKAvailable;

	public TimeSpan TimeSinceLastAd => new TimeSpan(Math.Min(TimeSinceLastInterstitial.Ticks, TimeSinceLastRewarded.Ticks));

	public TimeSpan TimeSinceLastInterstitial => DateTime.Now - prevInterstitialTime;

	public TimeSpan TimeSinceLastRewarded => TimeSpan.MaxValue;

	public bool ReadyForRewardedAdRequest => webReturnedAvailabilityRewardedAd && !showingAd;

	public bool ReadyForInterstitialAdRequest => webReturnedAvailabilityInterstitial && !showingAd;

	public WebAdManager()
	{
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
	}

	public void InitializeCallbackManager(IAdUIManager adUIManager)
	{
		this.adUIManager = adUIManager;
		if (MVGameControllerBase.GameSessionData.embedded && MVClientSettings.WebAdSDKsEnabled)
		{
			EmbeddedSite embeddedSite = EmbeddedSiteDetector.GetEmbeddedSite();
			if (embeddedSite != EmbeddedSite.None)
			{
				sdkManager = new AdSDKManager(embeddedSite);
				embeddedSiteSDKAvailable = true;
			}
		}
	}

	public void CreateAdManagerHack()
	{
		Debug.Log("Creating an ad manager that shouldn't be there, through a chat command ignoring embedded and webadsdksenabled check.");
		EmbeddedSite embeddedSite = EmbeddedSiteDetector.GetEmbeddedSite();
		if (embeddedSite != EmbeddedSite.None)
		{
			sdkManager = new AdSDKManager(embeddedSite);
			embeddedSiteSDKAvailable = true;
		}
	}

	public void ForceCreateCrazygamesSDK()
	{
		Debug.Log("Creating crazygames admanager forcefully through chat command. ");
		sdkManager = new AdSDKManager(EmbeddedSite.CrazyGames);
		embeddedSiteSDKAvailable = true;
	}

	public void RequestRewardedAd(Action<RewardedAdResult> rewardedAdCallback, AdContext context)
	{
		if (showingAd)
		{
			rewardedAdCallback(RewardedAdResult.ErrorClient);
			return;
		}
		SendStat("Ad.RewardRequest." + context);
		showingAd = true;
		adUIManager.ShowRewardedVideo(rewardedAdCallback);
		if (embeddedSiteSDKAvailable && sdkManager.ReadyForRewardedAdRequest)
		{
			sdkManager.RequestRewardedAd(RewardedAdShownSDKCallback, context);
		}
		else
		{
			RequestNonEmbeddedRewardedAd();
		}
	}

	public void RequestInterstitial(Action<InterstitialAdResult> interstitialCB, AdContext context)
	{
		if (showingAd)
		{
			interstitialCB(InterstitialAdResult.ErrorClient);
			return;
		}
		SendStat("Ad.InterstitialRequest." + context);
		showingAd = true;
		adUIManager.ShowInterstitial(interstitialCB);
		if (embeddedSiteSDKAvailable && sdkManager.ReadyForInterstitialAdRequest)
		{
			sdkManager.RequestInterstitial(InterstitialAdShownSDKCallback, context);
		}
		else
		{
			RequestNonEmbeddedInterstitialAd();
		}
	}

	private void InterstitialAdShownSDKCallback(InterstitialAdResult result)
	{
		if (result != InterstitialAdResult.Done)
		{
			RequestNonEmbeddedInterstitialAd();
			return;
		}
		showingAd = false;
		adUIManager.PopInterstitial(InterstitialAdResult.Done);
		Debug.Log("Interstitial ad shown.");
	}

	private void RewardedAdShownSDKCallback(RewardedAdResult result)
	{
		if (result != RewardedAdResult.RewardNotUnlocked && result != RewardedAdResult.RewardUnlocked)
		{
			RequestNonEmbeddedRewardedAd();
			return;
		}
		showingAd = false;
		adUIManager.PopRewardedVideo(RewardedAdResult.RewardUnlocked);
		Debug.Log("Rewarded ad shown.");
	}

	private void RequestNonEmbeddedInterstitialAd()
	{
		BrowserComm.ToJavaScript.ExternalCall("showVideoAd", OnInterstitialShownCallback);
	}

	private void RequestNonEmbeddedRewardedAd()
	{
		BrowserComm.ToJavaScript.ExternalCall("showRewardedVideoAd", OnRewardedAdShownCallback);
	}

	private static void SendStat(string stat)
	{
		StatHatWrapper.Count(stat, 1);
	}

	public void UpdateControllerUpdate()
	{
		if (Time.time - updateTime >= refreshTimer)
		{
			Debug.Log("Requesting ad");
			updateTime = Time.time;
			BrowserComm.ToJavaScript.ExternalCall("requestVideoAd", WebCallbackAdAvailable);
			BrowserComm.ToJavaScript.ExternalCall("requestRewardedVideoAd", WebCallbackRewardedAdAvailable);
		}
	}

	private void OnInterstitialShownCallback(bool ok, string json)
	{
		showingAd = false;
		adUIManager.PopInterstitial(InterstitialAdResult.Done);
		Debug.Log("Interstitial ad shown.");
	}

	private void OnRewardedAdShownCallback(bool ok, string json)
	{
		showingAd = false;
		RewardedAdResult adResult = RewardedAdResult.RewardNotUnlocked;
		if (ok)
		{
			try
			{
				if (JsonConvert.DeserializeObject<JSONRewardedAdSuccessful>(json).status)
				{
					adResult = RewardedAdResult.RewardUnlocked;
				}
			}
			catch
			{
				Debug.Log("Ad not finished. No reward given");
			}
		}
		adUIManager.PopRewardedVideo(adResult);
	}

	private void WebCallbackAdAvailable(bool ok, string jsonData)
	{
		webReturnedAvailabilityInterstitial = false;
		try
		{
			if (ok)
			{
				webReturnedAvailabilityInterstitial = JsonConvert.DeserializeObject<JSONAdReturnedData>(jsonData).adAvailable;
			}
		}
		catch
		{
			webReturnedAvailabilityInterstitial = true;
		}
	}

	private void WebCallbackRewardedAdAvailable(bool ok, string jsonData)
	{
		webReturnedAvailabilityRewardedAd = false;
		try
		{
			if (ok)
			{
				webReturnedAvailabilityRewardedAd = JsonConvert.DeserializeObject<JSONAdReturnedData>(jsonData).adAvailable;
			}
		}
		catch
		{
			webReturnedAvailabilityRewardedAd = true;
		}
	}
}
