using System;
using Assets.Scripts.AdIntegration;
using UnityEngine;
using UnityEngine.EventSystems;

public class AdUIWithTimeout : MonoBehaviour, IAdUIManager
{
	[SerializeField]
	private ShowingAdsPopup showingAdPopup;

	private static readonly float timeout = 20f;

	private bool popupShowing;

	private bool wantsToPop;

	private GameObject overlayPushedToStack;

	private InterstitialAdResult interstitialResultLatePop;

	private RewardedAdResult rewardedAdResultLatePop;

	private Action<InterstitialAdResult> interstitialCallback;

	private Action<RewardedAdResult> rewardedCallback;

	public bool AdShowing()
	{
		return popupShowing;
	}

	private void Awake()
	{
		MVGameControllerBase.AdManager.InitializeCallbackManager(this);
	}

	private void Update()
	{
		if (!wantsToPop)
		{
			return;
		}
		bool stackBlocked = true;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			stackBlocked = x.Peak() != overlayPushedToStack;
		});
		if (!stackBlocked)
		{
			Debug.Log("AdUIWithTimeout wantsToPop.");
			TryPopOverlay();
			if (interstitialCallback != null)
			{
				interstitialCallback(interstitialResultLatePop);
				interstitialCallback = null;
			}
			if (rewardedCallback != null)
			{
				rewardedCallback(rewardedAdResultLatePop);
				rewardedCallback = null;
			}
			wantsToPop = false;
			overlayPushedToStack = null;
		}
	}

	public void ShowInterstitial(Action<InterstitialAdResult> callbackFunction)
	{
		if (interstitialCallback != null)
		{
			Debug.LogError("Requesting interstitial twice.");
			return;
		}
		Debug.Log("AdUIWithTimeout ShowInterstitial.");
		interstitialCallback = callbackFunction;
		CreatePopup();
	}

	public void ShowRewardedVideo(Action<RewardedAdResult> callbackFunction)
	{
		if (rewardedCallback != null)
		{
			Debug.LogError("Requesting rewarded ad twice.");
			return;
		}
		Debug.Log("AdUIWithTimeout ShowRewardedVideo.");
		rewardedCallback = callbackFunction;
		CreatePopup();
	}

	public void PopInterstitial(InterstitialAdResult adResult)
	{
		Debug.Log("AdUIWithTimeout PopInterstitial.");
		if (interstitialCallback == null)
		{
			Debug.LogError("Closing interstitial manager twice.");
			return;
		}
		bool stackBlocked = true;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			stackBlocked = x.Peak() != overlayPushedToStack;
		});
		if (stackBlocked)
		{
			Debug.Log("AdUIWithTimeout stack is blocked. waiting until stack isn't blocked to pop");
			interstitialResultLatePop = adResult;
			wantsToPop = true;
		}
		else
		{
			Debug.Log("AdUIWithTimeout Pop");
			TryPopOverlay();
			interstitialCallback(adResult);
			interstitialCallback = null;
		}
	}

	public void PopRewardedVideo(RewardedAdResult adResult)
	{
		Debug.Log("AdUIWithTimeout PopRewardedVideo.");
		if (rewardedCallback == null)
		{
			Debug.LogError("Closing rewarded ad manager twice.");
			return;
		}
		bool stackBlocked = true;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			stackBlocked = x.Peak() != overlayPushedToStack;
		});
		if (stackBlocked)
		{
			Debug.Log("AdUIWithTimeout stack is blocked. waiting until stack isn't blocked to pop");
			rewardedAdResultLatePop = adResult;
			wantsToPop = true;
		}
		else
		{
			Debug.Log("AdUIWithTimeout Pop");
			TryPopOverlay();
			rewardedCallback(adResult);
			rewardedCallback = null;
		}
	}

	private void TryPopOverlay()
	{
		Debug.Log("AdUIWithTimeout TryPopOverlay. Popup is showing: " + popupShowing);
		if (popupShowing)
		{
			popupShowing = false;
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
		}
	}

	private void CreatePopup()
	{
		bool stackReady = false;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			stackReady = x.StackReady;
		});
		Debug.Log("AdUIWithTimeout CreatePopup. Stack is ready: " + stackReady);
		if (stackReady)
		{
			popupShowing = true;
			ShowingAdsPopup popup = UnityEngine.Object.Instantiate(showingAdPopup);
			popup.Initialize(timeout, OnSkipClicked);
			overlayPushedToStack = popup.gameObject;
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
			});
		}
	}

	private void OnSkipClicked()
	{
		Debug.Log("AdUIWithTimeout OnSkipClicked.");
		if (interstitialCallback != null)
		{
			PopInterstitial(InterstitialAdResult.ErrorTimeout);
		}
		else if (rewardedCallback != null)
		{
			PopRewardedVideo(RewardedAdResult.ErrorTimeout);
		}
		else
		{
			TryPopOverlay();
		}
	}
}
