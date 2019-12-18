namespace GoogleMobileAds.Common.Mediation.AppLovin;

public interface IAppLovinClient
{
	void Initialize();

	void SetHasUserConsent(bool hasUserConsent);

	void SetIsAgeRestrictedUser(bool isAgeRestrictedUser);
}
