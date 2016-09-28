using System.Collections.Generic;
using MV.WorldObject.Security;

public class TouristPromotionDesktop : TouristPromotion
{
	public void SignupCallback()
	{
		if (!LevelingManager.IsInitialized)
		{
			BrowserComm.ToJavaScript.ExternalCall("gotoSignup");
			BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.signupURL);
			return;
		}
		SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
		int xP = MVGameControllerBase.Game.LocalPlayer.XPProgressData.XP;
		sortedDictionary.Add("xp", xP.ToString());
		string mD5Hash = Encryption.GetMD5Hash(sortedDictionary, MVGameControllerBase.Game.XpKey);
		BrowserComm.ToJavaScript.ExternalCall("gotoSignupWithXP", xP, mD5Hash);
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.signupURL);
	}

	public void LoginCallback()
	{
		BrowserComm.ToJavaScript.ExternalCall("gotoLogin");
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.loginURL);
	}
}
