using System;
using System.Collections.Generic;
using Assets.Scripts.AdIntegration;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePassesProgressBarFreeTryHandler : MonoBehaviour
{
	[SerializeField]
	private GameTierProgressBar tierProgressBar;

	[SerializeField]
	private TierUnlockedPopupController TierUnlockedPopupControllerPrefab;

	[SerializeField]
	private GamePassesShop gamePassesShopPrefab;

	[SerializeField]
	private List<Image> buttonAdImages;

	private GamePassTier tierToTry;

	private bool isWaitingForFreeTryTier;

	private void OnEnable()
	{
		for (int i = 0; i < buttonAdImages.Count; i++)
		{
			buttonAdImages[i].enabled = !GamePassesManager.TogglePreviewState.FreeTryWithoutAdAvailable;
		}
	}

	public void OnFreeTryTier(int tierToTry)
	{
		this.tierToTry = (GamePassTier)tierToTry;
		GamePassesShop gamePassesShop = UnityEngine.Object.Instantiate(gamePassesShopPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(gamePassesShop.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		gamePassesShop.Initialize(this.tierToTry);
	}

	private void ShowTierUnlock(bool wasPurchased, bool wasTempUnlocked)
	{
		TierUnlockedPopupController tierUnlockedPopupController = UnityEngine.Object.Instantiate(TierUnlockedPopupControllerPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(tierUnlockedPopupController.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		tierUnlockedPopupController.Initialize(tierToTry, wasPurchased, wasTempUnlocked);
	}

	private void ShowAd()
	{
		if (GamePassesManager.TogglePreviewState.CanToggle)
		{
			if (GamePassesManager.TogglePreviewState.FreeTryWithoutAdAvailable)
			{
				PreviewTier();
				GamePassesManager.TogglePreviewState.FreeTryWithoutAdAvailable = false;
			}
			else
			{
				MVGameControllerBase.AdManager.RequestRewardedAd(RewardedAdCallback, AdContext.PreviewTier);
			}
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(MVGameControllerBase.AdManager.RewardedAdNotAvailableText, TM._("No Ad Available"));
			});
		}
	}

	private void RewardedAdCallback(RewardedAdResult result)
	{
		switch (result)
		{
		case RewardedAdResult.RewardUnlocked:
			PreviewTier();
			break;
		case RewardedAdResult.RewardNotUnlocked:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("The video was canceled. Your Free Try has not been activated."), TM._("Video canceled"));
			});
			break;
		case RewardedAdResult.ErrorClient:
		case RewardedAdResult.ErrorInternal:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(MVGameControllerBase.AdManager.RewardedAdNotAvailableText, TM._("No Ad Available"));
			});
			break;
		case RewardedAdResult.ErrorTimeout:
			break;
		}
	}

	private void PreviewTier()
	{
		if (GamePassesManager.TogglePreviewState.CanToggle)
		{
			MVGameControllerBase.OperationRequests.TogglePreviewTier();
			GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Combine(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
			tierProgressBar.DeactivateFreeTryBubble();
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create();
			});
			isWaitingForFreeTryTier = true;
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(TM._("Free try cannot be activated at this moment."), TM._("An error occurred"));
			});
		}
	}

	private void OnPlayerPlanetDataUpdated()
	{
		if (isWaitingForFreeTryTier)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			isWaitingForFreeTryTier = false;
		}
		GamePassTier previewGamePassTier = GamePassesManager.PlayerPlanetData.previewGamePassTier;
		if ((int)previewGamePassTier >= (int)tierToTry)
		{
			GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
			ShowTierUnlock(wasPurchased: false, wasTempUnlocked: true);
		}
	}
}
