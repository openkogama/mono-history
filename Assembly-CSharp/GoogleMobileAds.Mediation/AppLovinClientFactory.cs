using GoogleMobileAds.Common.Mediation.AppLovin;

namespace GoogleMobileAds.Mediation;

public class AppLovinClientFactory
{
	public static IAppLovinClient AppLovinInstance()
	{
		return new DummyClient();
	}
}
