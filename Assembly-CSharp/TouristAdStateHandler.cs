using System;
using Newtonsoft.Json;
using UnityEngine;

public class TouristAdStateHandler
{
	private struct JSONAdReturnedData
	{
		public bool adAvailable;
	}

	private bool webReturnedAdAvailability;

	private static readonly float refreshTimer = 60f;

	private float updateTime;

	private static readonly float adRateLimitTimer = 120f;

	private float adRateLimitCurrentTime;

	private Action OnAdShown;

	public void UpdateAdState()
	{
		if (Time.time - updateTime >= refreshTimer)
		{
			Debug.Log("Requesting ad");
			updateTime = Time.time;
			BrowserComm.ToJavaScript.ExternalCall("requestVideoAd", WebCallbackAdAvailable);
		}
	}

	public void ShowAd(Action OnAdFinished)
	{
		OnAdShown = OnAdFinished;
		if (webReturnedAdAvailability && Time.time - adRateLimitCurrentTime >= adRateLimitTimer)
		{
			Debug.Log("showVideoAd");
			BrowserComm.ToJavaScript.ExternalCall("showVideoAd", OnAdShownCallback);
		}
		else if (OnAdShown != null)
		{
			OnAdShown();
		}
	}

	private void OnAdShownCallback(bool ok, string json)
	{
		if (OnAdShown != null)
		{
			OnAdShown();
		}
		Debug.Log("Ad shown: close tourist promotion.");
		adRateLimitCurrentTime = Time.time;
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
