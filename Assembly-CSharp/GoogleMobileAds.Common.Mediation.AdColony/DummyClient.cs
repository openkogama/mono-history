using System.Reflection;
using UnityEngine;

namespace GoogleMobileAds.Common.Mediation.AdColony;

public class DummyClient : IAdColonyAppOptionsClient
{
	public DummyClient()
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public void SetGDPRConsentString(string consentString)
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public void SetGDPRRequired(bool gdprRequired)
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public void SetUserId(string userId)
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public void SetTestMode(bool isTestMode)
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public string GetGDPRConsentString()
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		return string.Empty;
	}

	public bool IsGDPRRequired()
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		return false;
	}

	public string GetUserId()
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		return string.Empty;
	}

	public bool IsTestMode()
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		return false;
	}
}
