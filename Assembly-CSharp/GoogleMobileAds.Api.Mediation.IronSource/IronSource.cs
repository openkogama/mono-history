using GoogleMobileAds.Common.Mediation.IronSource;
using GoogleMobileAds.Mediation;

namespace GoogleMobileAds.Api.Mediation.IronSource;

public class IronSource
{
	public static readonly IIronSourceClient client = GetIronSourceClient();

	public static void SetConsent(bool consent)
	{
		client.SetConsent(consent);
	}

	private static IIronSourceClient GetIronSourceClient()
	{
		return IronSourceClientFactory.IronSourceInstance();
	}
}
