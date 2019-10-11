using GoogleMobileAds.Common.Mediation.AdColony;
using GoogleMobileAds.Mediation;

namespace GoogleMobileAds.Api.Mediation.AdColony;

public class AdColonyAppOptions
{
	public static readonly IAdColonyAppOptionsClient client = GetAdColonyAppOptionsClient();

	private static IAdColonyAppOptionsClient GetAdColonyAppOptionsClient()
	{
		return AdColonyAppOptionsClientFactory.getAdColonyAppOptionsInstance();
	}

	public static void SetGDPRConsentString(string consentString)
	{
		client.SetGDPRConsentString(consentString);
	}

	public static void SetGDPRRequired(bool gdprRequired)
	{
		client.SetGDPRRequired(gdprRequired);
	}

	public static void SetUserId(string userId)
	{
		client.SetUserId(userId);
	}

	public static void SetTestMode(bool isTestMode)
	{
		client.SetTestMode(isTestMode);
	}

	public static string GetGDPRConsentString()
	{
		return client.GetGDPRConsentString();
	}

	public static bool IsGDPRRequired()
	{
		return client.IsGDPRRequired();
	}

	public static string GetUserId()
	{
		return client.GetUserId();
	}

	public static bool IsTestMode()
	{
		return client.IsTestMode();
	}
}
