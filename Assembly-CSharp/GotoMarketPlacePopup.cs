using UnityEngine;

public class GotoMarketPlacePopup : MonoBehaviour
{
	public void GoToMarketPlace()
	{
		BrowserComm.ExecuteBrowserRequest(AndroidUrls.MarketPlaceUrl);
		MVGameControllerBase.ApplicationQuit(null);
		Debug.Log("Goto marketplace");
	}
}
