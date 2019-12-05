using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouristAdPopup : TouristPromotion
{
	[SerializeField]
	private Button continueButton;

	[SerializeField]
	private GameObject redirectButton;

	[SerializeField]
	private Text redirectButtonURLText;

	[SerializeField]
	private GameObject goToKogamaPopupPrefab;

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

	public void KogamaRedirect()
	{
		if (MVGameControllerBase.Game.LocalPlayer.IsTourist)
		{
			if (MVGameControllerBase.GameSessionData.GetIsRedirectAllowed())
			{
				BrowserCommGotoRequests.GotoMainpage(newTab: true);
			}
			else
			{
				ShowGoToKogamaPopup();
			}
		}
	}

	protected override void Start()
	{
		base.Start();
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
		Debug.Log("Referrer: " + MVGameControllerBase.GameSessionData.referrer);
		redirectButton.SetActive(MVGameControllerBase.GameSessionData.embedded);
		Uri uri = new Uri(MVGameControllerBase.Game.KogamaMainpageURL);
		redirectButtonURLText.text = uri.Host.Replace("www.", string.Empty).ToUpper();
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

	private void OnWinningConditionFulfilled(IWinningCondition winningCondition)
	{
		MVGameControllerDesktop.LockCursorManager.CursorLock = false;
	}

	private void OnDestroy()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
	}

	private void ShowGoToKogamaPopup()
	{
		GameObject popUp = UnityEngine.Object.Instantiate(goToKogamaPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popUp, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
	}
}
