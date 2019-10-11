using GoogleMobileAds.Common.Mediation.UnityAds;

namespace GoogleMobileAds.Mediation;

public class UnityAdsClientFactory
{
	public static IUnityAdsClient UnityAdsInstance()
	{
		return new DummyClient();
	}
}
