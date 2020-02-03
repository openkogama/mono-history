using System;
using System.Collections.Generic;
using Assets.Scripts.AdIntegration;
using MV.Common;
using MV.WorldObject.GamePassSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class TierUnlockDetailsPopup : MonoBehaviour
{
	[SerializeField]
	private Text tierText;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private ProgressBar tierProgressBar;

	[SerializeField]
	private Text progressText;

	[SerializeField]
	private GameObject progressBarDivider;

	[SerializeField]
	private GameObject freeTryButton;

	[SerializeField]
	private GamePassesShop gamePassesShopPrefab;

	[SerializeField]
	private TierPurchaseNotEnoughGoldErrorPopup tierPurchaseGoldErrorPopupPrefab;

	[SerializeField]
	private TierUnlockedPopupController TierUnlockedPopupControllerPrefab;

	[SerializeField]
	private Image buttonAdImage;

	private GamePassTier tierToPurchase;

	private int price;

	private UnityAction OnPurchaseSuccessful;

	private bool isWaitingForFreeTryTier;

	public void Initialize(GamePassTier tierToPurchase, int price, UnityAction OnPurchaseSuccessful)
	{
		this.tierToPurchase = tierToPurchase;
		this.price = price;
		this.OnPurchaseSuccessful = OnPurchaseSuccessful;
		Text text = tierText;
		int num = (int)tierToPurchase;
		text.text = num.ToString();
		priceText.text = price.ToString();
		freeTryButton.SetActive(MVClientSettings.RewardedAdsEnabled);
		if (GamePassesManager.GamePassesActive)
		{
			UpdateTierProgressBar();
		}
		buttonAdImage.enabled = !GamePassesManager.TogglePreviewState.FreeTryWithoutAdAvailable;
	}

	public void ShowTier()
	{
		GamePassesShop gamePassesShop = UnityEngine.Object.Instantiate(gamePassesShopPrefab);
		gamePassesShop.Initialize(tierToPurchase);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(gamePassesShop.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	public void Purchase()
	{
		if (MVGameControllerBase.GameSessionData.gameMode != MVGameMode.Edit)
		{
			int gold = MVGameControllerBase.Game.LocalPlayer.UserProfileData.Gold;
			if (price <= gold)
			{
				ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
				{
					x.Create();
				});
				MVNetworkGame game = MVGameControllerBase.Game;
				game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
				MVGameControllerBase.OperationRequests.PurchaseTier(tierToPurchase);
			}
			else
			{
				TierPurchaseNotEnoughGoldErrorPopup tierErrorPopup = UnityEngine.Object.Instantiate(tierPurchaseGoldErrorPopupPrefab);
				ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
				{
					x.Push(tierErrorPopup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
				});
				tierErrorPopup.Initialize(tierToPurchase);
			}
			return;
		}
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		if (gamePassTier != tierToPurchase)
		{
			MVGameControllerBase.OperationRequests.SetTier(tierToPurchase);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
		}
	}

	public void FreeTryTier()
	{
		ShowAd();
	}

	private void OnEnable()
	{
		if (MVGameControllerBase.LocalPlayer.PlayerPlanetData != null && (int)MVGameControllerBase.LocalPlayer.PlayerPlanetData.gamePassTier >= (int)tierToPurchase && OnPurchaseSuccessful != null)
		{
			OnPurchaseSuccessful();
		}
	}

	private void OnDestroy()
	{
		GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
	}

	private void ProductPurchaseResponseHandler(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		if (returnCode == 0)
		{
			HandleSuccessfulPurchase();
			return;
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create((MVPurchaseReturnCode)returnCode, price);
		});
	}

	private void ShowTierUnlock(bool wasPurchased, bool wasTempUnlocked)
	{
		TierUnlockedPopupController tierUnlockedPopupController = UnityEngine.Object.Instantiate(TierUnlockedPopupControllerPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(tierUnlockedPopupController.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		tierUnlockedPopupController.Initialize(tierToPurchase, wasPurchased, wasTempUnlocked);
	}

	private void HandleSuccessfulPurchase()
	{
		StatHatWrapper.Count("Purchase.Tier", 1);
		StatHatWrapper.Count("Purchase.Tier." + tierToPurchase, 1);
		StatHatWrapper.Count("Purchase.Tier.GoldSpent", price);
		ShowTierUnlock(wasPurchased: true, wasTempUnlocked: false);
		OnPurchaseSuccessful();
	}

	private void UpdateTierProgressBar()
	{
		int progressionGamePoints = GamePassesManager.PlayerPlanetData.progressionGamePoints;
		GamePassTier gamePassTier = GamePassesManager.PlayerPlanetData.gamePassTier;
		Dictionary<GamePassTier, PlayerTierState> tierPricingState = GamePassesManager.playerTierStateCalculator.GetTierPricingState(progressionGamePoints, gamePassTier);
		int num = ReduceGamePointsWithPreviousTierRequirements(tierToPurchase, progressionGamePoints, tierPricingState);
		if ((float)num < 0f)
		{
			num = 0;
		}
		int gamePointRequirementBase = tierPricingState[tierToPurchase].gamePointRequirementBase;
		if (num > gamePointRequirementBase)
		{
			num = gamePointRequirementBase;
		}
		float progress = (float)num / (float)gamePointRequirementBase;
		tierProgressBar.Progress = progress;
		string text = num.ToString() + " / " + gamePointRequirementBase;
		progressText.text = text;
		if (progressBarDivider.activeSelf && tierProgressBar.Progress <= 0f)
		{
			progressBarDivider.SetActive(value: false);
		}
	}

	private int ReduceGamePointsWithPreviousTierRequirements(GamePassTier gamePassTierToDisplay, int gamePoints, Dictionary<GamePassTier, PlayerTierState> gameTierShopStatus)
	{
		for (int num = (int)(gamePassTierToDisplay - 1); num > 0; num--)
		{
			GamePassTier key = (GamePassTier)num;
			gamePoints -= gameTierShopStatus[key].gamePointRequirementBase;
		}
		return gamePoints;
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
				x.Create(TM._("Free try cannot be activated at this moment."), TM._("An error occurred"));
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
		if ((int)previewGamePassTier >= (int)tierToPurchase)
		{
			GamePassesManager.OnPlayerPlanetDataUpdated = (Action)Delegate.Remove(GamePassesManager.OnPlayerPlanetDataUpdated, new Action(OnPlayerPlanetDataUpdated));
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			ShowTierUnlock(wasPurchased: false, wasTempUnlocked: true);
		}
	}
}
