using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouristAdPopup : TouristPromotion
{
	[SerializeField]
	private Button continueButton;

	private bool waitingForAd;

	private float startTime;

	private float timeoutDelay = 20f;

	public void OnViewAdClicked()
	{
		Debug.Log("Showing Ad");
		waitingForAd = true;
		continueButton.interactable = false;
		startTime = Time.time;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ITouristAdController x, BaseEventData y) =>
		{
			x.ShowAd();
		});
	}

	private void Update()
	{
		if (waitingForAd && Time.time - startTime >= timeoutDelay)
		{
			Debug.Log("Show Ad timeout, try again");
			continueButton.interactable = true;
			waitingForAd = false;
		}
	}

	public void SignupCallback()
	{
		BrowserCommGotoRequests.GotoSignup(newTab: false, modalPopup: true);
	}

	public void LoginCallback()
	{
		BrowserCommGotoRequests.GotoLogin(newTab: false, modalPopup: true);
	}
}
