using System.Reflection;
using UnityEngine;

namespace GoogleMobileAds.Common.Mediation.AppLovin;

public class DummyClient : IAppLovinClient
{
	public DummyClient()
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public void Initialize()
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public void SetHasUserConsent(bool hasUserConsent)
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public void SetIsAgeRestrictedUser(bool isAgeRestrictedUser)
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}
}
