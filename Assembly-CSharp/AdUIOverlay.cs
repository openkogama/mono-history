using System;
using Assets.Scripts.AdIntegration;
using UnityEngine;
using UnityEngine.EventSystems;

public class AdUIOverlay : MonoBehaviour, IAdUIManager
{
	[SerializeField]
	private GameObject adBackground;

	private bool popupShowing;

	private void Awake()
	{
		MVGameControllerBase.AdManager.InitializeCallbackManager(this);
	}

	public void ShowInterstitial(Action<InterstitialAdResult> callbackFunction)
	{
		CreatePopup();
	}

	public void ShowRewardedVideo(Action<RewardedAdResult> callbackFunction)
	{
		CreatePopup();
	}

	public void PopInterstitial(InterstitialAdResult adResult)
	{
		Pop();
	}

	public void PopRewardedVideo(RewardedAdResult adResult)
	{
		Pop();
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
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(UnityEngine.Object.Instantiate(adBackground), UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
			});
		}
	}

	private void Pop()
	{
		if (popupShowing)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
		}
	}
}
