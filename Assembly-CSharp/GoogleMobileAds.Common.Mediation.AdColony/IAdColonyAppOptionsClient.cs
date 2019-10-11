namespace GoogleMobileAds.Common.Mediation.AdColony;

public interface IAdColonyAppOptionsClient
{
	void SetGDPRConsentString(string consentString);

	void SetGDPRRequired(bool gdprRequired);

	void SetUserId(string userId);

	void SetTestMode(bool isTestMode);

	string GetGDPRConsentString();

	bool IsGDPRRequired();

	string GetUserId();

	bool IsTestMode();
}
