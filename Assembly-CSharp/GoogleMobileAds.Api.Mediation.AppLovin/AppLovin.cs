using GoogleMobileAds.Common.Mediation.AppLovin;
using GoogleMobileAds.Mediation;

namespace GoogleMobileAds.Api.Mediation.AppLovin;

public class AppLovin
{
	private static readonly IAppLovinClient client = GetAppLovinClient();

	public static void Initialize()
	{
		client.Initialize();
	}

	public static void SetHasUserConsent(bool hasUserConsent)
	{
		client.SetHasUserConsent(hasUserConsent);
	}

	public static void SetIsAgeRestrictedUser(bool isAgeRestrictedUser)
	{
		client.SetIsAgeRestrictedUser(isAgeRestrictedUser);
	}

	private static IAppLovinClient GetAppLovinClient()
	{
		return AppLovinClientFactory.AppLovinInstance();
	}
}
