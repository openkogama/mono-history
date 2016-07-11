using FyberPlugin;
using MV.WorldObject.AndroidAdData;
using UnityEngine;

public static class AndroidAdManager
{
	private static Ad currentAd;

	public static void Initialize(int profileId, string adData)
	{
		AndroidAdDataFyber androidAdDataFyber = new AndroidAdDataFyber(45646, "1d31fdc4acb20b24a2ab3f168f445c1e");
		SetupFyber(profileId, androidAdDataFyber);
	}

	public static void RequestInterstitial()
	{
		Debug.Log("AndroidAdManager.RequestInterstitial");
		InterstitialRequester.Create().Request();
	}

	private static void SetupFyber(int profileId, AndroidAdDataFyber androidAdDataFyber)
	{
		FyberCallback.NativeError += OnNativeExceptionReceivedFromSDK;
		FyberCallback.AdAvailable += OnAdAvailable;
		FyberCallback.AdNotAvailable += OnAdNotAvailable;
		FyberCallback.RequestFail += OnRequestFail;
		if (profileId > 0)
		{
			Fyber.With(androidAdDataFyber.appId.ToString()).WithUserId(profileId.ToString()).WithSecurityToken(androidAdDataFyber.clientSecurityToken)
				.Start();
			Debug.Log("AndroidAdManager.SetupFyber");
		}
		else
		{
			Fyber.With(androidAdDataFyber.appId.ToString()).Start();
		}
	}

	private static void OnNativeExceptionReceivedFromSDK(string message)
	{
		Debug.Log("AndroidAdManager.OnNativeExceptionReceivedFromSDK " + message);
	}

	private static void OnRequestFail(RequestError error)
	{
		Debug.Log("AndroidAdManager.OnRequestFail " + error.Description);
	}

	private static void OnAdNotAvailable(AdFormat adFormat)
	{
		Debug.Log("AndroidAdManager.OnAdNotAvailable");
		if (adFormat == AdFormat.INTERSTITIAL)
		{
			Debug.Log("OnAdNotAvailable");
			currentAd = null;
		}
	}

	private static void OnAdAvailable(Ad ad)
	{
		Debug.Log("AndroidAdManager.OnAdAvailable");
		AdFormat adFormat = ad.AdFormat;
		if (adFormat == AdFormat.INTERSTITIAL)
		{
			Debug.Log("Got ad");
			currentAd = ad;
			currentAd.Start();
			currentAd = null;
		}
	}
}
