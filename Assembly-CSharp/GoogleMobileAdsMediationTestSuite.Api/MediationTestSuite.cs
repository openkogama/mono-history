using System;
using GoogleMobileAds.Api;
using GoogleMobileAdsMediationTestSuite.Common;

namespace GoogleMobileAdsMediationTestSuite.Api;

public class MediationTestSuite
{
	private readonly IMediationTestClient client;

	private static MediationTestSuite instance = new MediationTestSuite();

	private static MediationTestSuite Instance => instance;

	public static AdRequest AdRequest
	{
		set
		{
			Instance.AdRequestImpl = value;
		}
	}

	private AdRequest AdRequestImpl
	{
		set
		{
			client.AdRequest = value;
		}
	}

	public static event EventHandler<EventArgs> OnMediationTestSuiteDismissed;

	private MediationTestSuite()
	{
		client = GetMediationTestClient();
		client.OnMediationTestSuiteDismissed += HandleMediationTestSuiteDismissed;
	}

	public static void Show(string appId)
	{
		Instance.CallShow(appId);
	}

	private static IMediationTestClient GetMediationTestClient()
	{
		return MediationTestSuiteClientFactory.MediationTestSuiteInstance();
	}

	private void HandleMediationTestSuiteDismissed(object sender, EventArgs args)
	{
		if (OnMediationTestSuiteDismissed != null)
		{
			OnMediationTestSuiteDismissed(this, args);
		}
	}

	private void CallShow(string appId)
	{
		client.Show(appId);
	}
}
