using UnityEngine;

public class GotoSubscriptionURL : MonoBehaviour
{
	public void OnGotoSubscription()
	{
		BrowserCommGotoRequests.GotoEliteUpgrade(newTab: true);
	}
}
