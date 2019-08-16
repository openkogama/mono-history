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

	private bool webReturnedAdAvailability;

	private static readonly float refreshTimer = 60f;

	private float updateTime = 0f - refreshTimer;

	private DateTime prevInterstitialTime = DateTime.MinValue;

	private Action<InterstitialAdResult> interstitialCallback;

	public TimeSpan TimeSinceLastAd => new TimeSpan(Math.Min(TimeSinceLastInterstitial.Ticks, TimeSinceLastRewarded.Ticks));

	public TimeSpan TimeSinceLastInterstitial => DateTime.Now - prevInterstitialTime;

	public TimeSpan TimeSinceLastRewarded => TimeSpan.MaxValue;

	public bool ReadyForRewardedAdRequest => false;

	public bool ReadyForInterstitialAdRequest => webReturnedAdAvailability && interstitialCallback == null;

	public WebAdManager()
	{
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
	}

	public void RequestRewardedAd(Action<RewardedAdResult> rewardedAdCallback, string context)
	{
		throw new NotImplementedException();
	}

	public void RequestInterstitial(Action<InterstitialAdResult> interstitialCB, string context)
	{
		if (interstitialCallback != null)
		{
			interstitialCB(InterstitialAdResult.ErrorClient);
			return;
		}
		SendStat("Ad.InterstitialRequest." + context);
		interstitialCallback = interstitialCB;
		BrowserComm.ToJavaScript.ExternalCall("showVideoAd", OnAdShownCallback);
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
		}
	}

	private void OnAdShownCallback(bool ok, string json)
	{
		interstitialCallback(InterstitialAdResult.Done);
		interstitialCallback = null;
		Debug.Log("Ad shown: close tourist promotion.");
	}

	private void WebCallbackAdAvailable(bool ok, string jsonData)
	{
		webReturnedAdAvailability = false;
		if (ok)
		{
			webReturnedAdAvailability = JsonConvert.DeserializeObject<JSONAdReturnedData>(jsonData).adAvailable;
		}
	}
}
