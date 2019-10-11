using GoogleMobileAds.Common.Mediation.Tapjoy;
using GoogleMobileAds.Mediation;

namespace GoogleMobileAds.Api.Mediation.Tapjoy;

public class Tapjoy
{
	public static readonly ITapjoyClient client = GetTapjoyClient();

	private static ITapjoyClient GetTapjoyClient()
	{
		return TapjoyClientFactory.TapjoyInstance();
	}

	public static void SetUserConsent(string consentString)
	{
		client.SetUserConsent(consentString);
	}

	public static void SubjectToGDPR(bool gdprApplicability)
	{
		client.SubjectToGDPR(gdprApplicability);
	}
}
