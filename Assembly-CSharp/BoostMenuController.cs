using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoostMenuController : MonoBehaviour, IBoostAdController, IEventSystemHandler
{
	[SerializeField]
	private BoostMenuItem boostPrefab;

	[SerializeField]
	private RectTransform boostItemsScrollRect;

	[SerializeField]
	private RectTransform boostItemsContent;

	private BoostAdController boostAdController = new BoostAdController();

	private BoostType adRewardType;

	private Action<bool> boostUnlockedCallback;

	private bool requestingAd;

	private float timeSinceLastRequest;

	private static readonly float delayBetweenRequests = 15f;

	public void Initialize()
	{
		BoostController boostController = MVGameControllerBase.Game.LocalPlayer.BoostController;
		Dictionary<BoostType, Boost>.ValueCollection allBoosts = boostController.GetAllBoosts();
		foreach (Boost item in allBoosts)
		{
			BoostMenuItem boostMenuItem = UnityEngine.Object.Instantiate(boostPrefab);
			boostMenuItem.transform.SetParent(boostItemsContent, worldPositionStays: false);
			boostMenuItem.Initialize(item, boostController.IsBoostActive(item.Type));
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(boostItemsContent);
		if (boostItemsScrollRect.rect.width < boostItemsContent.rect.width)
		{
			boostItemsContent.pivot = new Vector2(0f, 0.5f);
		}
	}

	public void TryShowAd(BoostType type, Action<bool> OnUnlockedCallback)
	{
		if (!requestingAd || !(Time.time - timeSinceLastRequest < delayBetweenRequests))
		{
			timeSinceLastRequest = Time.time;
			requestingAd = true;
			boostUnlockedCallback = OnUnlockedCallback;
			adRewardType = type;
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create();
			});
			boostAdController.RequestAdAvailability(AdAvailable);
		}
	}

	private void AdAvailable(bool available)
	{
		requestingAd = false;
		if (available)
		{
			boostAdController.ShowAd(OnAdFinished);
		}
		else
		{
			OnAdFinished(adWasSuccessful: false);
		}
	}

	private void OnAdFinished(bool adWasSuccessful)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		if (adWasSuccessful)
		{
			MVGameControllerBase.Game.LocalPlayer.BoostController.ActivateBoost(adRewardType);
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("An error occurred. Try again later."), TM._("Error"));
			});
		}
		if (boostUnlockedCallback != null)
		{
			boostUnlockedCallback(adWasSuccessful);
		}
	}
}
