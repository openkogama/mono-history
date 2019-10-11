using System;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api;

public class RewardedAd
{
	private IRewardedAdClient client;

	public event EventHandler<EventArgs> OnAdLoaded;

	public event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;

	public event EventHandler<AdErrorEventArgs> OnAdFailedToShow;

	public event EventHandler<EventArgs> OnAdOpening;

	public event EventHandler<EventArgs> OnAdClosed;

	public event EventHandler<Reward> OnUserEarnedReward;

	public RewardedAd(string adUnitId)
	{
		client = GoogleMobileAdsClientFactory.BuildRewardedAdClient();
		client.CreateRewardedAd(adUnitId);
		client.OnAdLoaded += (object sender, EventArgs args) =>
		{
			if (OnAdLoaded != null)
			{
				OnAdLoaded(this, args);
			}
		};
		client.OnAdFailedToLoad += (object sender, AdErrorEventArgs args) =>
		{
			if (OnAdFailedToLoad != null)
			{
				OnAdFailedToLoad(this, args);
			}
		};
		client.OnAdFailedToShow += (object sender, AdErrorEventArgs args) =>
		{
			if (OnAdFailedToShow != null)
			{
				OnAdFailedToShow(this, args);
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
		client.OnUserEarnedReward += (object sender, Reward args) =>
		{
			if (OnUserEarnedReward != null)
			{
				OnUserEarnedReward(this, args);
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
		client.Show();
	}

	public void SetServerSideVerificationOptions(ServerSideVerificationOptions serverSideVerificationOptions)
	{
		client.SetServerSideVerificationOptions(serverSideVerificationOptions);
	}

	public string MediationAdapterClassName()
	{
		return client.MediationAdapterClassName();
	}
}
