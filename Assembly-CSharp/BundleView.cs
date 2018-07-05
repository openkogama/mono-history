using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.Accessories;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BundleView : MonoBehaviour
{
	[SerializeField]
	private Text originalPriceText;

	[SerializeField]
	private Text discountedPriceText;

	[SerializeField]
	private GameObject discountTag;

	[SerializeField]
	private Text discountTagText;

	[SerializeField]
	private Text goldSavedText;

	[SerializeField]
	private ConfirmationPopup bundlePopup;

	public void Initialize()
	{
		AccessoryBundleClient accessoryBundleClient = AccessoryDataManager.GetAccessoryBundleClient();
		HandlePrices(accessoryBundleClient);
	}

	public void OnBundlePurchaseClicked()
	{
		ConfirmationPopup popup = UnityEngine.Object.Instantiate(bundlePopup);
		popup.Initialize(TM._("Confirm"), OnPurchaseBundleConfirmation, TM._("Confirm Purchase"));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.InventoryUISubMenu);
		});
	}

	private void OnPurchaseBundleConfirmation(bool confirmed, ConfirmationPopup popup)
	{
		if (confirmed)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
			MVGameControllerBase.OperationRequests.PurchaseAvatarAccessoryBundle(AccessoryDataManager.GetAccessoryBundleId());
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
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
		switch ((MVPurchaseReturnCode)returnCode)
		{
		case MVPurchaseReturnCode.Success:
			Debug.Log("Display a skippable slideshow of accessories purchased.");
			break;
		case MVPurchaseReturnCode.InsufficientLevel:
		{
			ConfirmationPopup confirmationPopup2 = UnityEngine.Object.Instantiate(bundlePopup);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(confirmationPopup2.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
			});
			confirmationPopup2.Initialize(TM._("Too low level"), OnInsufficientResourceCallback, TM._("Get XP"));
			break;
		}
		case MVPurchaseReturnCode.InsufficientFunds:
		{
			ConfirmationPopup confirmationPopup = UnityEngine.Object.Instantiate(bundlePopup);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(confirmationPopup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
			});
			confirmationPopup.Initialize(TM._("Not enough gold"), OnInsufficientResourceCallback, TM._("Get gold"));
			break;
		}
		default:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create((MVPurchaseReturnCode)returnCode, 0, 0);
			});
			break;
		}
	}

	private void OnInsufficientResourceCallback(bool confirmed, ConfirmationPopup popup)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		if (confirmed)
		{
			BrowserComm.ToJavaScript.ExternalCall("gotoPurchaseGold");
			BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.purchaseGoldURL);
		}
	}

	private void HandlePrices(AccessoryBundleClient accessoryData)
	{
		int num = 0;
		int num2 = 0;
		List<AccessoryBundleItem> accessoryBundleItems = accessoryData.accessoryBundleItems;
		for (int i = 0; i < accessoryBundleItems.Count; i++)
		{
			AccessoryDataClient accessoryDataByMetaDataId = AccessoryDataManager.GetAccessoryDataByMetaDataId(accessoryBundleItems[i].accessoryMetaDataID);
			if (accessoryDataByMetaDataId != null && !accessoryDataByMetaDataId.owns)
			{
				num += accessoryDataByMetaDataId.priceGold;
				num2++;
			}
		}
		if (num2 == 0)
		{
			Debug.LogError("Bundle shown, but all items are owned");
			return;
		}
		int discount = accessoryData.discount;
		int num3 = num;
		originalPriceText.gameObject.SetActive(discount > 0);
		discountTag.SetActive(discount > 0);
		if (discount > 0)
		{
			discountTagText.text = "-" + discount + "%";
			int num4 = Mathf.FloorToInt((float)num * ((float)discount / 100f));
			num3 = num - num4;
			originalPriceText.text = num.ToString("N0");
			goldSavedText.gameObject.SetActive(value: true);
			goldSavedText.text = num4.ToString("N0");
		}
		discountedPriceText.text = num3.ToString("N0");
	}
}
