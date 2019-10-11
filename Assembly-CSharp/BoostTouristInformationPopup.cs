using UnityEngine;

public class BoostTouristInformationPopup : MonoBehaviour
{
	public void SignUp()
	{
		BrowserCommGotoRequests.GotoSignup(newTab: false, modalPopup: true);
	}
}
