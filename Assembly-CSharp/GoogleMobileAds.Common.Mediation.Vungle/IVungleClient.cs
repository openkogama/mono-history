using GoogleMobileAds.Api.Mediation.Vungle;

namespace GoogleMobileAds.Common.Mediation.Vungle;

public interface IVungleClient
{
	void UpdateConsentStatus(VungleConsent consentStatus);

	void UpdateConsentStatus(VungleConsent consentStatus, string consentMessageVersion);

	VungleConsent GetCurrentConsentStatus();

	string GetCurrentConsentMessageVersion();
}
