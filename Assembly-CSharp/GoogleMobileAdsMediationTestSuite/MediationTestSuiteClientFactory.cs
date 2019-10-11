using GoogleMobileAdsMediationTestSuite.Common;

namespace GoogleMobileAdsMediationTestSuite;

public class MediationTestSuiteClientFactory
{
	public static IMediationTestClient MediationTestSuiteInstance()
	{
		return new DummyClient();
	}
}
