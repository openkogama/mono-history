public class TouristPromotionAndroid : TouristPromotion
{
	public void SignupCallback()
	{
		BrowserCommGotoRequests.GotoSignup();
	}

	public void LoginCallback()
	{
		BrowserCommGotoRequests.GotoLogin();
	}
}
