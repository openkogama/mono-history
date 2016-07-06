using System;
using Newtonsoft.Json;
using UnityEngine;

public static class AdRequestHandler
{
	private class Available
	{
		public bool available { get; set; }
	}

	public class ShouldReward
	{
		public bool shouldReward { get; set; }
	}

	private static Action<bool> OnHealthAdAvailableCallback;

	private static Action<bool> OnHealthAdShownCallback;

	private static Action<bool> OnGoldAdAvailableCallback;

	private static Action<bool> OnGoldAdShownCallback;

	public static void GetHealthAdAvailable(Action<bool> OnAdAvailable)
	{
		OnHealthAdAvailableCallback = OnAdAvailable;
		BrowserComm.ToJavaScript.ExternalCall("requestVideoAd", HealthAdAvailable);
	}

	private static void HealthAdAvailable(bool success, string availableJsonString)
	{
		if (!success)
		{
			Debug.Log("Ad not available");
			return;
		}
		Available available = JsonConvert.DeserializeObject<Available>(availableJsonString);
		OnHealthAdAvailableCallback(available.available);
		OnHealthAdAvailableCallback = null;
	}

	public static void ShowHealthVideoAd(Action<bool> OnAdShown)
	{
		OnHealthAdShownCallback = OnAdShown;
		BrowserComm.ToJavaScript.ExternalCall("showVideoAd", ShowHealthVideoAdCallback);
	}

	private static void ShowHealthVideoAdCallback(bool success, string showVideoJsonString)
	{
		if (!success)
		{
			Debug.Log("Ad not shown");
			return;
		}
		ShouldReward shouldReward = JsonConvert.DeserializeObject<ShouldReward>(showVideoJsonString);
		if (!shouldReward.shouldReward)
		{
			Debug.Log("User did not finish watching ad");
		}
		else
		{
			OnHealthAdShownCallback(shouldReward.shouldReward);
		}
	}

	public static void GetGoldAdAvailable(Action<bool> OnAdAvailable)
	{
		OnGoldAdAvailableCallback = OnAdAvailable;
		BrowserComm.ToJavaScript.ExternalCall("requestGoldVideoAd", GoldAdAvailable);
	}

	private static void GoldAdAvailable(bool success, string availableJsonString)
	{
		if (!success)
		{
			Debug.Log("Ad not available");
			return;
		}
		Available available = JsonConvert.DeserializeObject<Available>(availableJsonString);
		if (OnGoldAdAvailableCallback != null)
		{
			OnGoldAdAvailableCallback(available.available);
			OnGoldAdAvailableCallback = null;
		}
	}

	public static void ShowGoldVideoAd(Action<bool> OnAdShown)
	{
		OnGoldAdShownCallback = OnAdShown;
		BrowserComm.ToJavaScript.ExternalCall("showGoldVideoAd", ShowGoldVideoAdCallback);
	}

	private static void ShowGoldVideoAdCallback(bool success, string showVideoJsonString)
	{
		if (!success)
		{
			Debug.Log("Ad not shown");
			return;
		}
		ShouldReward shouldReward = JsonConvert.DeserializeObject<ShouldReward>(showVideoJsonString);
		if (!shouldReward.shouldReward)
		{
			Debug.Log("User did not finish watching ad");
		}
		else if (OnGoldAdShownCallback != null)
		{
			OnGoldAdShownCallback(shouldReward.shouldReward);
			OnGoldAdShownCallback = null;
		}
	}
}
