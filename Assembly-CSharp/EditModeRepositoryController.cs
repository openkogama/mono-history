using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class EditModeRepositoryController : MonoBehaviour
{
	private ShopItem currentlyBuyingItem;

	public void PurchaseClientShopItem(ShopItem item, UnityAction UpdateContent)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopToGroup(UIGroupFlags.MainUI);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		currentlyBuyingItem = item;
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		MVGameControllerBase.OperationRequests.UnlockClientShopInventoryItem(item.itemID);
	}

	public void ProductPurchaseResponseHandler(int returnCode, Dictionary<object, object> data)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		if (returnCode == 0)
		{
			MVGameControllerBase.EditModeUI.PlayerInventoryRepository.AddPurchasedItem(currentlyBuyingItem);
			MVGameControllerBase.EditModeUI.ClientShopRepository.RemoveItem(currentlyBuyingItem);
			MVGameControllerBase.EditModeUI.ClientShopRepository.ReorganizeBySlotPositions();
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create((MVPurchaseReturnCode)returnCode, int.Parse(currentlyBuyingItem.priceGold.ToString()));
			});
		}
	}
}
