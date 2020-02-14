using System;
using MV.Common;
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

	private IAdUIManager adUIManager;

	private IAdManager sdkManager;

	private bool embeddedSiteSDKAvailable;

	private EmbeddedSite embeddedSite;

	private bool probablyWatchingAd;

	private float probablyWatchingAdDelay = 3f;

	private float probablyWatchingAdStarted;

	private AdContext currentAdType;

	private bool showingAd;

	public string RewardedAdNotAvailableText => TM._("Please ensure AdBlock is disabled, and be sure to watch the ad from start to finish.");

	public TimeSpan TimeSinceLastAd => new TimeSpan(Math.Min(TimeSinceLastInterstitial.Ticks, TimeSinceLastRewarded.Ticks));

	public TimeSpan TimeSinceLastInterstitial => DateTime.Now - prevInterstitialTime;

	public TimeSpan TimeSinceLastRewarded => TimeSpan.MaxValue;

	public bool ReadyForRewardedAdRequest => webReturnedAvailabilityRewardedAd && !adUIManager.AdShowing();

	public bool ReadyForInterstitialAdRequest => webReturnedAvailabilityInterstitial && !adUIManager.AdShowing();

	public WebAdManager()
	{
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
	}

	public void InitializeCallbackManager(IAdUIManager adUIManager)
	{
		this.adUIManager = adUIManager;
		if (MVGameControllerBase.GameSessionData.embedded && MVClientSettings.WebAdSDKsEnabled)
		{
			embeddedSite = EmbeddedSiteDetector.GetEmbeddedSite();
			sdkManager = new AdSDKManager(embeddedSite);
			embeddedSiteSDKAvailable = true;
		}
	}

	public void CreateAdManagerHack()
	{
		Debug.Log("Creating an ad manager that shouldn't be there, through a chat command ignoring embedded and webadsdksenabled check.");
		embeddedSite = EmbeddedSiteDetector.GetEmbeddedSite();
		sdkManager = new AdSDKManager(embeddedSite);
		embeddedSiteSDKAvailable = true;
	}

	public void ForceCreateEmbeddedSiteSDK(EmbeddedSite site)
	{
		Debug.Log("Creating " + site.ToString() + " admanager forcefully through chat command. ");
		sdkManager = new AdSDKManager(site);
		embeddedSite = site;
		embeddedSiteSDKAvailable = true;
	}

	public void RequestRewardedAd(Action<RewardedAdResult> rewardedAdCallback, AdContext context)
	{
		if (adUIManager.AdShowing())
		{
			Debug.Log("Ad already showing, aborting");
			rewardedAdCallback(RewardedAdResult.ErrorClient);
			return;
		}
		currentAdType = context;
		StartedWatchingAd();
		SendRewardRequestStats(context);
		adUIManager.ShowRewardedVideo(rewardedAdCallback);
		if (embeddedSiteSDKAvailable && sdkManager.ReadyForRewardedAdRequest)
		{
			Debug.Log("embedded sdk available and initialized");
			sdkManager.RequestRewardedAd(RewardedAdShownSDKCallback, context);
			return;
		}
		Debug.Log("Embedded sdk not available.");
		if (embeddedSite != EmbeddedSite.GameDistribution)
		{
			RequestNonEmbeddedRewardedAd();
		}
		else
		{
			adUIManager.PopRewardedVideo(RewardedAdResult.RewardNotUnlocked);
		}
	}

	private void StartedWatchingAd()
	{
		showingAd = true;
		probablyWatchingAd = false;
		probablyWatchingAdStarted = Time.time;
	}

	private void SendRewardRequestStats(AdContext context)
	{
		SendStat("Ad.RewardRequest");
		SendStat("Ad.RewardRequest." + context);
		MVGameControllerBase.OperationRequests.IncrementStatRequest(IncrementStatRequestType.RewardedAdRequest);
	}

	public void RequestInterstitial(Action<InterstitialAdResult> interstitialCB, AdContext context)
	{
		if (adUIManager.AdShowing())
		{
			Debug.Log("Ad already showing, aborting");
			interstitialCB(InterstitialAdResult.ErrorClient);
			return;
		}
		currentAdType = context;
		StartedWatchingAd();
		SendInterstitialAdRequestStats(context);
		Debug.Log("RequestInterstitial");
		adUIManager.ShowInterstitial(interstitialCB);
		if (embeddedSiteSDKAvailable && sdkManager.ReadyForInterstitialAdRequest)
		{
			Debug.Log("embedded sdk available and initialized");
			sdkManager.RequestInterstitial(InterstitialAdShownSDKCallback, context);
			return;
		}
		Debug.Log("Embedded sdk not available.");
		if (embeddedSite != EmbeddedSite.GameDistribution)
		{
			RequestNonEmbeddedInterstitialAd();
		}
		else
		{
			adUIManager.PopInterstitial(InterstitialAdResult.Done);
		}
	}

	private void SendInterstitialAdRequestStats(AdContext context)
	{
		SendStat("Ad.InterstitialRequest." + context);
		SendStat("Ad.InterstitialRequest");
		MVGameControllerBase.OperationRequests.IncrementStatRequest(IncrementStatRequestType.InterstitialAdRequest);
	}

	private void InterstitialAdShownSDKCallback(InterstitialAdResult result)
	{
		Debug.Log("InterstitialAdShownSDKCallback: " + result);
		if (result != InterstitialAdResult.Done && embeddedSite != EmbeddedSite.GameDistribution)
		{
			RequestNonEmbeddedInterstitialAd();
			return;
		}
		Debug.Log("Interstitial ad shown.");
		SendStat("Ad.InterstitialShown");
		SendStat("Ad.InterstitialShown." + embeddedSite);
		SetFinishedWatchingAd("Interstitial");
		adUIManager.PopInterstitial(InterstitialAdResult.Done);
	}

	private void RewardedAdShownSDKCallback(RewardedAdResult result)
	{
		Debug.Log("RewardedAdShownSDKCallback: " + result);
		if (result != RewardedAdResult.RewardNotUnlocked && result != RewardedAdResult.RewardUnlocked && embeddedSite != EmbeddedSite.GameDistribution)
		{
			RequestNonEmbeddedRewardedAd();
			return;
		}
		Debug.Log("Rewarded ad shown.");
		SendStat("Ad.RewardedShown");
		SendStat("Ad.RewardedShown." + embeddedSite);
		SetFinishedWatchingAd("Rewarded");
		adUIManager.PopRewardedVideo(result);
	}

	private void RequestNonEmbeddedInterstitialAd()
	{
		Debug.Log("no embedded sdk available/initialized. requesting kogama interstitial ad");
		BrowserComm.ToJavaScript.ExternalCall("showVideoAd", OnInterstitialShownCallback);
	}

	private void RequestNonEmbeddedRewardedAd()
	{
		Debug.Log("no embedded sdk available/initialized. requesting kogama rewarded ad");
		BrowserComm.ToJavaScript.ExternalCall("showRewardedVideoAd", OnRewardedAdShownCallback);
	}

	private void SetFinishedWatchingAd(string adType)
	{
		showingAd = false;
		SendStat("Ad." + adType + "Finished." + currentAdType);
		if (probablyWatchingAd)
		{
			probablyWatchingAd = false;
			SendStat(string.Concat("Ad.", adType, "Finished.", currentAdType, ".Success"));
		}
		else
		{
			SendStat(string.Concat("Ad.", adType, "Finished.", currentAdType, ".Failure"));
		}
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
		if (showingAd && !probablyWatchingAd && Time.time - probablyWatchingAdStarted >= probablyWatchingAdDelay)
		{
			probablyWatchingAd = true;
		}
	}

	private void OnInterstitialShownCallback(bool ok, string json)
	{
		SendStat("Ad.InterstitialShown");
		SendStat("Ad.InterstitialShown.Kogama");
		SetFinishedWatchingAd("Interstitial");
		adUIManager.PopInterstitial(InterstitialAdResult.Done);
		Debug.Log("Interstitial ad shown.");
	}

	private void OnRewardedAdShownCallback(bool ok, string json)
	{
		RewardedAdResult adResult = RewardedAdResult.RewardNotUnlocked;
		if (ok)
		{
			try
			{
				if (JsonConvert.DeserializeObject<JSONRewardedAdSuccessful>(json).status)
				{
					SendStat("Ad.RewardedShown");
					SendStat("Ad.RewardedShown.Kogama");
					adResult = RewardedAdResult.RewardUnlocked;
				}
			}
			catch
			{
				Debug.Log("Ad not finished. No reward given");
			}
		}
		SetFinishedWatchingAd("Rewarded");
		adUIManager.PopRewardedVideo(adResult);
	}

	private void WebCallbackAdAvailable(bool ok, string jsonData)
	{
		webReturnedAvailabilityInterstitial = false;
		Debug.Log("WebCallbackAdAvailable");
		try
		{
			if (ok)
			{
				webReturnedAvailabilityInterstitial = JsonConvert.DeserializeObject<JSONAdReturnedData>(jsonData).adAvailable;
				Debug.Log("WebCallbackAdAvailable: " + webReturnedAvailabilityInterstitial);
			}
		}
		catch (Exception message)
		{
			Debug.Log(message);
			Debug.Log("Error occurred while requesting web interstitial. webReturnedAvailabilityInterstitial set to true");
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
				Debug.Log("WebCallbackAdAvailable: " + webReturnedAvailabilityRewardedAd);
			}
		}
		catch (Exception message)
		{
			Debug.Log(message);
			Debug.Log("Error occurred while requesting web rewarded ad. webReturnedAvailabilityRewardedAd set to true");
			webReturnedAvailabilityRewardedAd = true;
		}
	}
}
