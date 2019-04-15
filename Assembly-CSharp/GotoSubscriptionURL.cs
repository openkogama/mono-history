using UnityEngine;

public class GotoSubscriptionURL : MonoBehaviour
{
	public void OnGotoSubscription()
	{
		BrowserComm.ToJavaScript.ExternalCall("goToEliteUpgrade");
	}
}
