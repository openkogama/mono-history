using System;
using UnityEngine;

public class GotoMarketPlacePopup : MonoBehaviour
{
	public void GoToMarketPlace()
	{
		MVGameControllerBase.ApplicationQuit(new QuitBrowserRequest(AndroidUrls.MarketPlaceUrl + "&dateNow=" + DateTime.Now.ToFileTime()));
		Debug.Log("Goto marketplace");
	}
}
