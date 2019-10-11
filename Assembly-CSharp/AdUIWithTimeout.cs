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

	private Action<InterstitialAdResult> interstitialCallback;

	private Action<RewardedAdResult> rewardedCallback;

	private void Awake()
	{
		MVGameControllerBase.AdManager.InitializeCallbackManager(this);
	}

	public void ShowInterstitial(Action<InterstitialAdResult> callbackFunction)
	{
		if (interstitialCallback != null)
		{
			Debug.LogError("Requesting interstitial twice.");
			return;
		}
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
		rewardedCallback = callbackFunction;
		CreatePopup();
	}

	public void PopInterstitial(InterstitialAdResult adResult)
	{
		if (interstitialCallback == null)
		{
			Debug.LogError("Closing interstitial manager twice.");
			return;
		}
		TryPopOverlay();
		interstitialCallback(adResult);
		interstitialCallback = null;
	}

	public void PopRewardedVideo(RewardedAdResult adResult)
	{
		if (rewardedCallback == null)
		{
			Debug.LogError("Closing rewarded ad manager twice.");
			return;
		}
		TryPopOverlay();
		rewardedCallback(adResult);
		rewardedCallback = null;
	}

	private void TryPopOverlay()
	{
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
		if (stackReady)
		{
			popupShowing = true;
			ShowingAdsPopup popup = UnityEngine.Object.Instantiate(showingAdPopup);
			popup.Initialize(timeout, OnSkipClicked);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
			});
		}
	}

	private void OnSkipClicked()
	{
		if (interstitialCallback != null)
		{
			PopInterstitial(InterstitialAdResult.ErrorTimeout);
		}
		if (rewardedCallback != null)
		{
			PopRewardedVideo(RewardedAdResult.ErrorTimeout);
		}
	}
}
