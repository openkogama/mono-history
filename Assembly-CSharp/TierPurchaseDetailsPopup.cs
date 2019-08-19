using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class TierPurchaseDetailsPopup : MonoBehaviour
{
	[SerializeField]
	private Text tierText;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private Text buyText;

	[SerializeField]
	private GameObject priceStrikeOut;

	[SerializeField]
	private GamePassesShop gamePassesShopPrefab;

	[SerializeField]
	private TierPurchaseNotEnoughGoldErrorPopup tierPurchaseGoldErrorPopupPrefab;

	[SerializeField]
	private TierUnlockedPopupController TierUnlockedPopupControllerPrefab;

	private GamePassTier tierToPurchase;

	private int price;

	private UnityAction OnPurchaseSuccessful;

	public void Initialize(GamePassTier tierToPurchase, int price, UnityAction OnPurchaseSuccessful)
	{
		this.tierToPurchase = tierToPurchase;
		this.price = price;
		this.OnPurchaseSuccessful = OnPurchaseSuccessful;
		Text text = tierText;
		int num = (int)tierToPurchase;
		text.text = num.ToString();
		priceText.text = price.ToString();
		if (MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit)
		{
			buyText.text = "Test";
			priceStrikeOut.SetActive(value: true);
		}
	}

	public void ShowTier()
	{
		GamePassesShop gamePassesShop = UnityEngine.Object.Instantiate(gamePassesShopPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(gamePassesShop.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		gamePassesShop.Initialize(tierToPurchase);
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

	private void OnEnable()
	{
		if (MVGameControllerBase.LocalPlayer.PlayerPlanetData != null && (int)MVGameControllerBase.LocalPlayer.PlayerPlanetData.gamePassTier >= (int)tierToPurchase)
		{
			OnPurchaseSuccessful();
		}
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

	private void ShowTierUnlock(bool wasPurchased)
	{
		TierUnlockedPopupController tierUnlockedPopupController = UnityEngine.Object.Instantiate(TierUnlockedPopupControllerPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(tierUnlockedPopupController.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		tierUnlockedPopupController.Initialize(tierToPurchase, wasPurchased);
	}

	private void HandleSuccessfulPurchase()
	{
		ShowTierUnlock(wasPurchased: true);
		OnPurchaseSuccessful();
	}
}
