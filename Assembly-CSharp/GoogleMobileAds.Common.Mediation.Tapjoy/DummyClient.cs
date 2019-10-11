using System.Reflection;
using UnityEngine;

namespace GoogleMobileAds.Common.Mediation.Tapjoy;

public class DummyClient : ITapjoyClient
{
	public DummyClient()
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public void SetUserConsent(string consentString)
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public void SubjectToGDPR(bool gdprApplicability)
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}
}
