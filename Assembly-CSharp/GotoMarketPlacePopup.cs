using System;
using UnityEngine;

public class GotoMarketPlacePopup : MonoBehaviour
{
	public void GoToMarketPlace()
	{
		BrowserComm.ExecuteBrowserRequest(AndroidUrls.MarketPlaceUrl + "&dateNow=" + DateTime.Now.ToFileTime());
		MVGameControllerBase.ApplicationQuit(null);
		Debug.Log("Goto marketplace");
	}
}
