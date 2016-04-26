using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameCoinBoostShopDialog : MonoBehaviour
{
	[SerializeField]
	private GameObject waitOverLay;

	[SerializeField]
	private Text price;

	[SerializeField]
	private Button purchase;

	private Dictionary<object, object> purchaseResponseData;

	private UnityAction<bool, Dictionary<object, object>> callback;

	public void Initialize(UnityAction<bool, Dictionary<object, object>> callback)
	{
		this.callback = callback;
		purchase.onClick.AddListener(OnPurchaseClick);
		price.text = PricesManager.GetPrice("GameCoinBoost").gold.ToString();
	}

	private void OnPurchaseClick()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		MVGameControllerBase.Game.PurchaseGameCoinBooster();
		purchase.gameObject.SetActive(value: false);
		waitOverLay.SetActive(value: true);
	}

	private void ProductPurchaseResponseHandler(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		if (returnCode == 0)
		{
			callback(arg0: true, purchaseResponseData);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			return;
		}
		callback(arg0: false, purchaseResponseData);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create((MVPurchaseReturnCode)returnCode, int.Parse(price.text), 0);
		});
		waitOverLay.SetActive(value: false);
	}
}
