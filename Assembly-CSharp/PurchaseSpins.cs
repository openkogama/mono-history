using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class PurchaseSpins : MonoBehaviour
{
	[SerializeField]
	private Text price;

	[SerializeField]
	private Text spinCount;

	[SerializeField]
	private int numOfSpins;

	[SerializeField]
	private NotificationPopup notificationPopupPrefab;

	private UnityAction OnConfirmed;

	public void Initialize(UnityAction OnPurchaseSpinsPop)
	{
		OnConfirmed = OnPurchaseSpinsPop;
		SetText();
	}

	private void SetText()
	{
		if (RewardManager.IsInitialized)
		{
			price.text = (RewardManager.SpinPrice * numOfSpins).ToString();
			spinCount.text = numOfSpins.ToString();
		}
		else
		{
			RewardManager.RequestInitialization(SetText);
		}
	}

	public void ConfirmAmountOfSpins()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		MVGameControllerBase.OperationRequests.PurchaseMysteryBoxSpins(numOfSpins);
	}

	private void ProductPurchaseResponseHandler(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		if (returnCode == 0)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.PopGroups(UIGroupFlags.Popup);
			});
			NotificationPopup notification = UnityEngine.Object.Instantiate(notificationPopupPrefab);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(notification.gameObject, UIPushOption.Blocking, OnConfirmed, UIGroupFlags.Popup);
			});
			notification.Initialize(string.Format(TM._("Successfully purchased {0} extra spins for {1}!"), numOfSpins, RewardManager.SpinPrice * numOfSpins), TM._("Success!"));
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.PopGroups(UIGroupFlags.Popup);
			});
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create((MVPurchaseReturnCode)returnCode, RewardManager.SpinPrice * numOfSpins, 0);
			});
		}
	}
}
