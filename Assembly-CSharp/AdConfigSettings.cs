public class AdConfigSettings
{
	public bool AdTimeoutAsSuccess { get; private set; }

	public int AdTimeoutAsSuccessDelay { get; private set; }

	public EmbeddedSiteConfigData EmbeddedSiteConfigData { get; private set; }

	public int InterstitialTimeoutAfterRewardedAd { get; private set; }

	public AdConfigSettings(EmbeddedSiteConfigData embeddedSiteConfigData, bool adAutoSuccessAfterDelayEnabled, int delayBeforeAdIsAutoSuccess, int interstitialTimeoutAfterRewardedAd)
	{
		EmbeddedSiteConfigData = embeddedSiteConfigData;
		AdTimeoutAsSuccess = adAutoSuccessAfterDelayEnabled;
		AdTimeoutAsSuccessDelay = delayBeforeAdIsAutoSuccess;
		InterstitialTimeoutAfterRewardedAd = interstitialTimeoutAfterRewardedAd;
	}
}
