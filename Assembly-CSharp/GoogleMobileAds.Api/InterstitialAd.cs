using System;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api;

public class InterstitialAd
{
	private IInterstitialClient client;

	public event EventHandler<EventArgs> OnAdLoaded;

	public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

	public event EventHandler<EventArgs> OnAdOpening;

	public event EventHandler<EventArgs> OnAdClosed;

	public event EventHandler<EventArgs> OnAdLeavingApplication;

	public InterstitialAd(string adUnitId)
	{
		client = GoogleMobileAdsClientFactory.BuildInterstitialClient();
		client.CreateInterstitialAd(adUnitId);
		client.OnAdLoaded += (object sender, EventArgs args) =>
		{
			if (OnAdLoaded != null)
			{
				OnAdLoaded(this, args);
			}
		};
		client.OnAdFailedToLoad += (object sender, AdFailedToLoadEventArgs args) =>
		{
			if (OnAdFailedToLoad != null)
			{
				OnAdFailedToLoad(this, args);
			}
		};
		client.OnAdOpening += (object sender, EventArgs args) =>
		{
			if (OnAdOpening != null)
			{
				OnAdOpening(this, args);
			}
		};
		client.OnAdClosed += (object sender, EventArgs args) =>
		{
			if (OnAdClosed != null)
			{
				OnAdClosed(this, args);
			}
		};
		client.OnAdLeavingApplication += (object sender, EventArgs args) =>
		{
			if (OnAdLeavingApplication != null)
			{
				OnAdLeavingApplication(this, args);
			}
		};
	}

	public void LoadAd(AdRequest request)
	{
		client.LoadAd(request);
	}

	public bool IsLoaded()
	{
		return client.IsLoaded();
	}

	public void Show()
	{
		client.ShowInterstitial();
	}

	public void Destroy()
	{
		client.DestroyInterstitial();
	}

	public string MediationAdapterClassName()
	{
		return client.MediationAdapterClassName();
	}
}
