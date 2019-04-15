using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class TierPurchasePopup : MonoBehaviour
{
	[SerializeField]
	private Text tierText;

	[SerializeField]
	private Text priceText;

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
	}

	public void Purchase()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		MVGameControllerBase.OperationRequests.PurchaseTier(tierToPurchase);
	}

	public void Cancel()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
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

	private void HandleSuccessfulPurchase()
	{
		OnPurchaseSuccessful();
	}
}
