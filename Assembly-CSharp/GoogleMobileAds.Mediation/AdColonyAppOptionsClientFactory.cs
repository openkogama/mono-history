using GoogleMobileAds.Common.Mediation.AdColony;

namespace GoogleMobileAds.Mediation;

public class AdColonyAppOptionsClientFactory
{
	public static IAdColonyAppOptionsClient getAdColonyAppOptionsInstance()
	{
		return new DummyClient();
	}
}
