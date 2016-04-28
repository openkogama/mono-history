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
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(string.Format(TM._("Are you sure you wish to purchase {0} spins for {1} gold?"), numOfSpins, RewardManager.SpinPrice * numOfSpins), OnConfirmedPurchase, TM._("Confirm Purchase"));
		});
	}

	private void OnConfirmedPurchase(bool confirmed, ConfirmationPopup popup)
	{
		if (confirmed)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
			MVGameControllerBase.OperationRequests.PurchaseMysteryBoxSpins(numOfSpins);
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.PopGroups(UIGroupFlags.Popup);
			});
		}
	}

	private void ProductPurchaseResponseHandler(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		if (returnCode == 0)
		{
			OnConfirmed();
			OnConfirmed = null;
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.PopGroups(UIGroupFlags.Popup);
			});
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
