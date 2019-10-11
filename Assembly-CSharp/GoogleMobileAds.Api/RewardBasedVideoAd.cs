using System;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api;

public class RewardBasedVideoAd
{
	private IRewardBasedVideoAdClient client;

	private static readonly RewardBasedVideoAd instance = new RewardBasedVideoAd();

	public static RewardBasedVideoAd Instance => instance;

	public event EventHandler<EventArgs> OnAdLoaded;

	public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

	public event EventHandler<EventArgs> OnAdOpening;

	public event EventHandler<EventArgs> OnAdStarted;

	public event EventHandler<EventArgs> OnAdClosed;

	public event EventHandler<Reward> OnAdRewarded;

	public event EventHandler<EventArgs> OnAdLeavingApplication;

	public event EventHandler<EventArgs> OnAdCompleted;

	private RewardBasedVideoAd()
	{
		client = GoogleMobileAdsClientFactory.BuildRewardBasedVideoAdClient();
		client.CreateRewardBasedVideoAd();
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
		client.OnAdStarted += (object sender, EventArgs args) =>
		{
			if (OnAdStarted != null)
			{
				OnAdStarted(this, args);
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
		client.OnAdRewarded += (object sender, Reward args) =>
		{
			if (OnAdRewarded != null)
			{
				OnAdRewarded(this, args);
			}
		};
		client.OnAdCompleted += (object sender, EventArgs args) =>
		{
			if (OnAdCompleted != null)
			{
				OnAdCompleted(this, args);
			}
		};
	}

	public void LoadAd(AdRequest request, string adUnitId)
	{
		client.LoadAd(request, adUnitId);
	}

	public bool IsLoaded()
	{
		return client.IsLoaded();
	}

	public void Show()
	{
		client.ShowRewardBasedVideoAd();
	}

	public void SetUserId(string userId)
	{
		client.SetUserId(userId);
	}

	public string MediationAdapterClassName()
	{
		return client.MediationAdapterClassName();
	}
}
