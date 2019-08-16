using GoogleMobileAds.Common.Mediation.Vungle;
using GoogleMobileAds.Mediation;

namespace GoogleMobileAds.Api.Mediation.Vungle;

public class Vungle
{
	public static readonly IVungleClient client = GetVungleClient();

	public static void UpdateConsentStatus(VungleConsent consentStatus)
	{
		client.UpdateConsentStatus(consentStatus);
	}

	public static void UpdateConsentStatus(VungleConsent consentStatus, string consentMessageVersion)
	{
		client.UpdateConsentStatus(consentStatus, consentMessageVersion);
	}

	public static VungleConsent GetCurrentConsentStatus()
	{
		return client.GetCurrentConsentStatus();
	}

	public static string GetCurrentConsentMessageVersion()
	{
		return client.GetCurrentConsentMessageVersion();
	}

	private static IVungleClient GetVungleClient()
	{
		return VungleClientFactory.VungleInstance();
	}
}
