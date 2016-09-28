public class TouristPromotionAndroid : TouristPromotion
{
	public void SignupCallback()
	{
		MVGameControllerBase.ApplicationQuit(new QuitBrowserRequest(MVGameControllerBase.GameSessionData.signupURL));
	}

	public void LoginCallback()
	{
		MVGameControllerBase.ApplicationQuit(new QuitBrowserRequest(MVGameControllerBase.GameSessionData.loginURL));
	}
}
