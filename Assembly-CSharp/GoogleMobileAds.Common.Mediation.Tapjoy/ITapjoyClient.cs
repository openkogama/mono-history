namespace GoogleMobileAds.Common.Mediation.Tapjoy;

public interface ITapjoyClient
{
	void SetUserConsent(string consentString);

	void SubjectToGDPR(bool gdprApplicability);
}
