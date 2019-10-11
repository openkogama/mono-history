public static class BrowserCommGotoRequests
{
	private struct Options(bool newTab = false, bool modalPopup = false)
	{
		public bool openInNewTab = newTab;

		public bool openInModal = modalPopup;
	}

	private const string eliteUpgrade = "goToEliteUpgrade_v2";

	private const string purchaseGold = "gotoPurchaseGold_v2";

	private const string playerProfile = "gotoPlayerProfile_v2";

	private const string login = "gotoLogin_v2";

	private const string signup = "gotoSignup_v2";

	private const string signout = "gotoSignout";

	private const string idle = "gotoIdlePage";

	private const string disconnected = "gotoDisconnectedPage";

	public static void GotoPurchaseGold(bool newTab = false, bool modalPopup = false)
	{
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.purchaseGoldURL);
	}

	public static void GotoEliteUpgrade(bool newTab = false, bool modalPopup = false)
	{
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.eliteUpgradeURL);
	}

	public static void GotoPlayerProfile(int profileId, bool newTab = false, bool modalPopup = false)
	{
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.playerProfileURL + profileId + "/");
	}

	public static void GotoLogin(bool newTab = false, bool modalPopup = false)
	{
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.loginURL);
	}

	public static void GotoMainpage(bool newTab = false, bool modalPopup = false)
	{
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.Game.KogamaMainpageURL);
	}

	public static void GotoSignup(bool newTab = false, bool modalPopup = false)
	{
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.signupURL);
	}

	public static void GotoSignout()
	{
		MVGameControllerBase.ApplicationQuit(null);
	}

	public static void GotoIdle()
	{
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.idleURL);
	}

	public static void GotoDisconnected()
	{
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.disconnectedURL);
	}
}
