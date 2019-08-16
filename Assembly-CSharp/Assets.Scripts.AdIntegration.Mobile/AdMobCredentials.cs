namespace Assets.Scripts.AdIntegration.Mobile;

public class AdMobCredentials
{
	public readonly string AppId;

	public readonly string RewardedAdUnitId;

	public readonly string InterstitialAdUnitId;

	public readonly string BannerAdUnitId;

	public AdMobCredentials(string appId, string rewardedAdUnitId, string interstitialAdUnitId, string bannerAdUnitId)
	{
		AppId = appId;
		RewardedAdUnitId = rewardedAdUnitId;
		InterstitialAdUnitId = interstitialAdUnitId;
		BannerAdUnitId = bannerAdUnitId;
	}

	public override string ToString()
	{
		return $"AppId {AppId}. RewardedAdUnitId {RewardedAdUnitId}. InterstitialAdUnitId {InterstitialAdUnitId}. BannerAdUnitId {BannerAdUnitId}.";
	}
}
