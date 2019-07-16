using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoostPurchasePopup : MonoBehaviour
{
	[Serializable]
	private struct BoosterDef
	{
		public BoostType type;

		public GameObject iconPrefab;
	}

	[SerializeField]
	private Text headerText;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private RectTransform boostImageParent;

	[SerializeField]
	private List<BoosterDef> boosterList;

	private string boostKey;

	private int price;

	private UnityAction OnPurchaseSuccessful;

	public void Initialize(BoostType boostType, string boostKey, string boostName, int price, UnityAction OnPurchaseSuccessful)
	{
		this.boostKey = boostKey;
		this.price = price;
		this.OnPurchaseSuccessful = OnPurchaseSuccessful;
		headerText.text = boostName;
		priceText.text = price.ToString("N0").Replace(",", ".");
		CreateBoostImage(boostType);
	}

	public void Purchase()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		MVGameControllerBase.OperationRequests.PurchaseGameBooster(boostKey);
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

	private void CreateBoostImage(BoostType boostType)
	{
		for (int i = 0; i < boosterList.Count; i++)
		{
			if (boosterList[i].type == boostType)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(boosterList[i].iconPrefab);
				gameObject.transform.SetParent(boostImageParent, worldPositionStays: false);
				break;
			}
		}
	}
}
