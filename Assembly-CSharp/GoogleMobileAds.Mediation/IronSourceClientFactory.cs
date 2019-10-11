using GoogleMobileAds.Common.Mediation.IronSource;

namespace GoogleMobileAds.Mediation;

public class IronSourceClientFactory
{
	public static IIronSourceClient IronSourceInstance()
	{
		return new DummyClient();
	}
}
