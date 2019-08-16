using System.Reflection;
using GoogleMobileAds.Api.Mediation.Vungle;
using UnityEngine;

namespace GoogleMobileAds.Common.Mediation.Vungle;

public class DummyClient : IVungleClient
{
	public DummyClient()
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public void UpdateConsentStatus(VungleConsent consentStatus)
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public void UpdateConsentStatus(VungleConsent consentStatus, string consentMessageVersion)
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public VungleConsent GetCurrentConsentStatus()
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		return VungleConsent.UNKNOWN;
	}

	public string GetCurrentConsentMessageVersion()
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		return string.Empty;
	}
}
