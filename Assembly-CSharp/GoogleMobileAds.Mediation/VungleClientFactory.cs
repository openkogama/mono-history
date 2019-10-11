using GoogleMobileAds.Common.Mediation.Vungle;

namespace GoogleMobileAds.Mediation;

public class VungleClientFactory
{
	public static IVungleClient VungleInstance()
	{
		return new DummyClient();
	}
}
