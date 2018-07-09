using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.Accessories;
using Newtonsoft.Json;
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
	private Text bundlePriceTextWithoutText;

	[SerializeField]
	private GameObject discountTag;

	[SerializeField]
	private Text discountTagText;

	[SerializeField]
	private Text goldSavedText;

	[SerializeField]
	private ConfirmationPopup bundlePopup;

	[SerializeField]
	private PurchasedAccessoryPreviewer previewSlideshowPrefab;

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
		{
			List<int> list = JsonConvert.DeserializeObject<List<int>>((string)purchaseResponseData[(byte)108]);
			AccessoryDataClient[] array = new AccessoryDataClient[list.Count];
			for (int num = 0; num < list.Count; num++)
			{
				AccessoryDataClient accessoryDataByStreamingAssetId = AccessoryDataManager.GetAccessoryDataByStreamingAssetId(list[num]);
				if (accessoryDataByStreamingAssetId != null)
				{
					array[num] = accessoryDataByStreamingAssetId;
				}
			}
			PurchasedAccessoryPreviewer popup = UnityEngine.Object.Instantiate(previewSlideshowPrefab);
			popup.Initialize(array);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popup.gameObject, UIPushOption.Blocking, OnPop, UIGroupFlags.Popup);
			});
			break;
		}
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

	private void OnPop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IBundleController x, BaseEventData y) =>
		{
			x.PurchasedBundle();
		});
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
			discountedPriceText.gameObject.SetActive(value: true);
			bundlePriceTextWithoutText.gameObject.SetActive(value: false);
		}
		else
		{
			discountedPriceText.gameObject.SetActive(value: false);
			bundlePriceTextWithoutText.gameObject.SetActive(value: true);
		}
		discountedPriceText.text = num3.ToString("N0");
		bundlePriceTextWithoutText.text = num3.ToString("N0");
	}
}
