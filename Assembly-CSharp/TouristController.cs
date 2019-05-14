using UnityEngine;

public class TouristController : MonoBehaviour
{
	public void OnClick()
	{
		BrowserCommGotoRequests.GotoSignup(newTab: false, modalPopup: true);
	}
}
