using System;
using GoogleMobileAds.Api;

namespace GoogleMobileAdsMediationTestSuite.Common;

public interface IMediationTestClient
{
	AdRequest AdRequest { set; }

	event EventHandler<EventArgs> OnMediationTestSuiteDismissed;

	void Show(string appId);

	void Show();
}
