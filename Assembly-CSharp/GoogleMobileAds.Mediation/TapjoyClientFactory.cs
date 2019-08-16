using GoogleMobileAds.Common.Mediation.Tapjoy;

namespace GoogleMobileAds.Mediation;

public class TapjoyClientFactory
{
	public static ITapjoyClient TapjoyInstance()
	{
		return new DummyClient();
	}
}
