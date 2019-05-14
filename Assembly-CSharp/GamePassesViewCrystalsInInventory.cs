using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.MetaData;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePassesViewCrystalsInInventory : MonoBehaviour
{
	private List<ShopItem> buyingItems = new List<ShopItem>();

	private int pending;

	public void OnClickGetCrystals()
	{
		HighlightObject(WorldObjectType.GamePoint);
	}

	private void HighlightObject(WorldObjectType worldObjectType)
	{
		int itemSlot = GetItemSlot(worldObjectType);
		if (itemSlot == -1)
		{
			TryPurchaseCrystalItems();
			return;
		}
		OpenInventoryAtPosition(7, itemSlot);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.Popup);
		});
	}

	private int GetItemSlot(WorldObjectType worldObjectType)
	{
		int result = -1;
		if (MVGameControllerBase.EditModeUI.PlayerInventoryRepository.GetItemByWorldObjectTypeInCategory(InventoryCategoryType.Pickups, worldObjectType, out var item))
		{
			result = item.slotPosition;
		}
		return result;
	}

	private void OnInventoryItemAdded(int category, int slot)
	{
		OpenInventoryAtPosition(category, slot);
	}

	private void TryPurchaseCrystalItems()
	{
		PurchaseItem(WorldObjectType.GamePoint);
		PurchaseItem(WorldObjectType.GamePointChest);
	}

	private void PurchaseItem(WorldObjectType woType)
	{
		if (MVGameControllerBase.EditModeUI.ClientShopRepository.GetItemByWorldObjectTypeInCategory(InventoryCategoryType.Pickups, woType, out var item))
		{
			PurchaseClientShopItem(item);
		}
		else
		{
			Debug.LogWarning("Tried to buy crystal item but it couldn't be found in shop inventory.");
		}
	}

	private void OpenInventoryAtPosition(int categoryId, int itemSlot)
	{
		if (!FirstTimeEventManager.HasFirstTimeEventOccured(FirstTimeEvent.BM_PlaceBazooka))
		{
			FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent.BM_PlaceBazooka);
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IPlayerInventory x, BaseEventData y) =>
		{
			x.ActivateAtCategoryWithSlot(UIPushOption.Blocking, categoryId, itemSlot);
		});
	}

	private void PurchaseClientShopItem(ShopItem item)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		if (pending == 0)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		}
		pending++;
		buyingItems.Add(item);
		MVGameControllerBase.OperationRequests.UnlockClientShopInventoryItem(item.itemID);
	}

	private void ProductPurchaseResponseHandler(int returnCode, Dictionary<object, object> data)
	{
		pending--;
		if (pending != 0)
		{
			return;
		}
		for (int i = 0; i < buyingItems.Count; i++)
		{
			ShopItem currentlyBuyingItem = buyingItems[i];
			if (returnCode == 0)
			{
				MVGameControllerBase.EditModeUI.PlayerInventoryRepository.AddPurchasedItem(currentlyBuyingItem);
				MVGameControllerBase.EditModeUI.ClientShopRepository.RemoveItem(currentlyBuyingItem);
				MVGameControllerBase.EditModeUI.ClientShopRepository.ReorganizeBySlotPositions();
			}
			else
			{
				Debug.LogError("Trying to pay for crystals through Add Crystals popup. Contact dev.");
				ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
				{
					x.Create((MVPurchaseReturnCode)returnCode, int.Parse(currentlyBuyingItem.priceGold.ToString()));
				});
			}
		}
		buyingItems.Clear();
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopToGroup(UIGroupFlags.MainUI);
		});
		OpenInventoryAtPosition(7, GetItemSlot(WorldObjectType.GamePoint));
	}
}
