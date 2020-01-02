public class TouristPromotionAndroid : TouristPromotion
{
	public override void SkipCallback()
	{
		StatHatWrapper.Count("TouristPromotion.Kogama.Continue", 1);
		base.SkipCallback();
	}

	public void SignupCallback()
	{
		BrowserCommGotoRequests.GotoSignup();
	}

	public void LoginCallback()
	{
		BrowserCommGotoRequests.GotoLogin();
	}
}
