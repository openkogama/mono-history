using GoogleMobileAds.Common.Mediation.UnityAds;
using GoogleMobileAds.Mediation;

namespace GoogleMobileAds.Api.Mediation.UnityAds;

public class UnityAds
{
	public static readonly IUnityAdsClient client = GetUnityAdsClient();

	private static IUnityAdsClient GetUnityAdsClient()
	{
		return UnityAdsClientFactory.UnityAdsInstance();
	}

	public static void SetGDPRConsentMetaData(bool consent)
	{
		client.SetGDPRConsentMetaData(consent);
	}
}
