using System.Reflection;
using UnityEngine;

namespace GoogleMobileAds.Common.Mediation.UnityAds;

public class DummyClient : IUnityAdsClient
{
	public DummyClient()
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public void SetGDPRConsentMetaData(bool consent)
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}
}
