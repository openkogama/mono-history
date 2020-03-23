using System;
using System.Reflection;
using GoogleMobileAds.Api;
using UnityEngine;

namespace GoogleMobileAdsMediationTestSuite.Common;

public class DummyClient : IMediationTestClient
{
	public AdRequest AdRequest
	{
		set
		{
		}
	}

	public event EventHandler<EventArgs> OnMediationTestSuiteDismissed;

	public DummyClient()
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public void Show(string appId)
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}

	public void Show()
	{
		Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
	}
}
