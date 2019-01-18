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
	private Text bundlePriceWithoutDiscount;

	[SerializeField]
	private GameObject discountTag;

	[SerializeField]
	private Text discountTagText;

	[SerializeField]
	private Text goldSavedText;

	[SerializeField]
	private PurchasedAccessoryPreviewer previewSlideshowPrefab;

	[SerializeField]
	private Image levelLocked;

	[SerializeField]
	private BundlePurchasePopUp bundlePurchasePopup;

	[SerializeField]
	private BundleErrorPopUp bundleErrorPopUp;

	[SerializeField]
	private LevelErrorPopup levelErrorPopUp;

	[SerializeField]
	private GameObject claimText;

	[SerializeField]
	private Button purchaseButton;

	[SerializeField]
	private AccessoryShinyButton shineEffect;

	private AccessoryBundleClient bundleData;

	private int price;

	private int originalPrice;

	public void Initialize()
	{
		bundleData = AccessoryDataManager.AccessoryBundleClient;
		if (MVGameControllerBase.Game.LocalPlayer.Level >= bundleData.level)
		{
			HandlePrices(bundleData);
		}
		else
		{
			HandleLevel(bundleData);
		}
	}

	public void OnBundlePurchaseClicked()
	{
		if (MVGameControllerBase.Game.LocalPlayer.Level < bundleData.level)
		{
			LevelErrorPopup errorPopup = UnityEngine.Object.Instantiate(levelErrorPopUp);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(errorPopup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
			});
			errorPopup.Initialize(OnInsufficientLevelCallback, bundleData.level);
		}
		else if (MVGameControllerBase.Game.LocalPlayer.UserProfileData.Gold < price)
		{
			BundleErrorPopUp errorPopup2 = UnityEngine.Object.Instantiate(bundleErrorPopUp);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(errorPopup2.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
			});
			errorPopup2.Initialize(OnInsufficientResourceCallback, TM._("NOT ENOUGH GOLD"), TM._("Get Gold"));
		}
		else
		{
			BundlePurchasePopUp popup = UnityEngine.Object.Instantiate(bundlePurchasePopup);
			popup.Initialize(bundleData, price, originalPrice, OnPurchaseBundleConfirmation);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.InventoryUISubMenu);
			});
		}
	}

	private void OnPurchaseBundleConfirmation(bool confirmed)
	{
		if (confirmed)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
			MVGameControllerBase.OperationRequests.PurchaseAvatarAccessoryBundle(AccessoryDataManager.AccessoryBundleId);
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
			LevelErrorPopup errorPopup2 = UnityEngine.Object.Instantiate(levelErrorPopUp);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(errorPopup2.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
			});
			errorPopup2.Initialize(OnInsufficientLevelCallback, bundleData.level);
			break;
		}
		case MVPurchaseReturnCode.InsufficientFunds:
		{
			BundleErrorPopUp errorPopup = UnityEngine.Object.Instantiate(bundleErrorPopUp);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(errorPopup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
			});
			errorPopup.Initialize(OnInsufficientResourceCallback, TM._("NOT ENOUGH GOLD"), TM._("Get Gold"));
			break;
		}
		default:
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create((MVPurchaseReturnCode)returnCode, 0);
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

	private void OnInsufficientResourceCallback(bool confirmed)
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

	private void OnInsufficientLevelCallback()
	{
	}

	private void HandlePrices(AccessoryBundleClient accessoryData)
	{
		originalPrice = 0;
		int num = 0;
		levelLocked.gameObject.SetActive(value: false);
		purchaseButton.image.color = Styles.GetColor(ColorStyle.ButtonSuccess);
		shineEffect.gameObject.SetActive(value: true);
		List<AccessoryBundleItem> accessoryBundleItems = accessoryData.accessoryBundleItems;
		for (int i = 0; i < accessoryBundleItems.Count; i++)
		{
			AccessoryDataClient accessoryDataByMetaDataId = AccessoryDataManager.GetAccessoryDataByMetaDataId(accessoryBundleItems[i].accessoryMetaDataID);
			if (accessoryDataByMetaDataId != null && !accessoryDataByMetaDataId.owns)
			{
				originalPrice += accessoryDataByMetaDataId.priceGold;
				num++;
			}
		}
		if (num == 0)
		{
			Debug.LogError("Bundle shown, but all items are owned");
			return;
		}
		int discount = accessoryData.discount;
		price = originalPrice;
		originalPriceText.gameObject.SetActive(discount > 0);
		discountTag.SetActive(discount > 0);
		goldSavedText.gameObject.SetActive(discount > 0);
		claimText.SetActive(value: false);
		if (discount > 0)
		{
			discountTagText.text = "-" + discount + "%";
			int num2 = Mathf.FloorToInt((float)originalPrice * ((float)discount / 100f));
			price = originalPrice - num2;
			originalPriceText.text = originalPrice.ToString("N0").Replace(",", " ");
			goldSavedText.text = num2.ToString("N0").Replace(",", " ");
			discountedPriceText.gameObject.SetActive(value: true);
			bundlePriceWithoutDiscount.gameObject.SetActive(value: false);
		}
		else
		{
			discountedPriceText.gameObject.SetActive(value: false);
			bundlePriceWithoutDiscount.gameObject.SetActive(value: true);
		}
		if (price == 0)
		{
			discountTag.SetActive(value: false);
			discountedPriceText.gameObject.SetActive(value: false);
			bundlePriceWithoutDiscount.gameObject.SetActive(value: false);
			originalPriceText.gameObject.SetActive(value: false);
			goldSavedText.gameObject.SetActive(value: false);
			claimText.SetActive(value: true);
		}
		discountedPriceText.text = price.ToString("N0").Replace(",", " ");
		bundlePriceWithoutDiscount.text = price.ToString("N0").Replace(",", " ");
	}

	private void HandleLevel(AccessoryBundleClient accessoryData)
	{
		purchaseButton.image.color = Styles.GetColor(ColorStyle.DisabledButton);
		shineEffect.gameObject.SetActive(value: false);
		levelLocked.gameObject.SetActive(value: true);
		originalPriceText.gameObject.SetActive(value: false);
		discountedPriceText.gameObject.SetActive(value: false);
		bundlePriceWithoutDiscount.gameObject.SetActive(value: false);
		discountTag.SetActive(value: false);
		goldSavedText.gameObject.SetActive(value: false);
		claimText.SetActive(value: false);
	}
}
