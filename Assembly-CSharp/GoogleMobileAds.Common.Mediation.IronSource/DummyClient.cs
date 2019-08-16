using System.Reflection;
using UnityEngine;

namespace GoogleMobileAds.Common.Mediation.IronSource;

public class DummyClient : IIronSourceClient
{
	public DummyClient()
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public void SetConsent(bool consent)
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}
}
