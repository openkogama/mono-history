using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarAccessoryPurchasePopup : MonoBehaviour
{
	private AccessoryDataClient accessoryDataClient;

	[SerializeField]
	private StreamedSpriteToImageManual preview;

	[SerializeField]
	private GameObject loadingWheel;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private Text originalPriceText;

	[SerializeField]
	private GameObject discountTag;

	[SerializeField]
	private GameObject freeItemTag;

	[SerializeField]
	private Text discountTagText;

	[SerializeField]
	private AccessoryItemBackground accessoryItemBackground;

	[SerializeField]
	private AccessoryTimeLimitDisplayer timeLimitDisplayer;

	[SerializeField]
	private GameObject newAccessoryImage;

	[SerializeField]
	private AvatarAccessorySuccesPopup avatarAccessorySuccesPopup;

	[SerializeField]
	private AvatarAccessoryErrorPopup insufficientResourcesPopup;

	[SerializeField]
	private PurchasedAccessoryPreviewer successPreviewer;

	[SerializeField]
	private GameObject emptyFrame;

	private int price;

	private string previewImageUrl;

	public void Initialize(AccessoryDataClient accessoryDataClient, string previewImageUrl)
	{
		loadingWheel.SetActive(value: true);
		preview.gameObject.SetActive(value: false);
		preview.Download(previewImageUrl, OnPreviewImageDownLoaded);
		this.accessoryDataClient = accessoryDataClient;
		priceText.text = accessoryDataClient.cost.ToString();
		price = accessoryDataClient.cost;
		this.previewImageUrl = previewImageUrl;
		accessoryItemBackground.Initialize(accessoryDataClient);
		HandleNotOwnedUI();
	}

	public void Purchase()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		MVGameControllerBase.OperationRequests.PurchaseAvatarAccessory(accessoryDataClient.sAID);
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
			HandleSuccessfulPurchase(purchaseResponseData);
			return;
		case MVPurchaseReturnCode.InsufficientLevel:
			HandleInsufficientResources(TM._("Too low level"), TM._("Get XP"));
			return;
		case MVPurchaseReturnCode.InsufficientFunds:
			HandleInsufficientResources(TM._("Not enough gold"), TM._("Get gold"));
			return;
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create((MVPurchaseReturnCode)returnCode, price);
		});
	}

	private void HandleInsufficientResources(string header, string buttonText)
	{
		AvatarAccessoryErrorPopup confirmationPopup = UnityEngine.Object.Instantiate(insufficientResourcesPopup);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(confirmationPopup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
		confirmationPopup.Initialize(OnGoldPurchaseDialogResult, previewImageUrl, accessoryDataClient, header, buttonText);
	}

	private void OnGoldPurchaseDialogResult(bool result)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		if (result)
		{
			BrowserCommGotoRequests.GotoPurchaseGold(newTab: false, modalPopup: true);
		}
	}

	private void HandleSuccessfulPurchase(Dictionary<object, object> purchaseResponseData)
	{
		int num = (int)purchaseResponseData[(byte)105];
		AccessoryDataManager.SetToOwns(num);
		SuccessfulPopupCallBack(num);
	}

	private void SuccessfulPopupCallBack(int streamingAssetId)
	{
		AccessoryDataClient accessoryDataByStreamingAssetId = AccessoryDataManager.GetAccessoryDataByStreamingAssetId(streamingAssetId);
		AccessoryDataClient[] previewAccessories = new AccessoryDataClient[1] { accessoryDataByStreamingAssetId };
		PurchasedAccessoryPreviewer popup = UnityEngine.Object.Instantiate(successPreviewer);
		popup.Initialize(previewAccessories);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup.gameObject, UIPushOption.Blocking, Pop, UIGroupFlags.InventoryUISubMenu);
		});
	}

	public void Pop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	private void HandlePrices(AccessoryDataClient streamingAssetInfo)
	{
		int cost = streamingAssetInfo.cost;
		int dsc = streamingAssetInfo.dsc;
		int num = cost;
		originalPriceText.gameObject.SetActive(dsc > 0);
		discountTag.SetActive(dsc > 0);
		if (dsc > 0)
		{
			discountTagText.text = ((dsc < 100) ? ("-" + dsc + "%") : "FREE");
			int num2 = Mathf.FloorToInt((float)cost * ((float)dsc / 100f));
			num = cost - num2;
			originalPriceText.text = cost.ToString("N0").Replace(",", " ");
		}
		freeItemTag.SetActive(num == 0);
		if (num == 0)
		{
			originalPriceText.gameObject.SetActive(value: false);
			discountTag.SetActive(value: false);
			priceText.gameObject.SetActive(value: false);
		}
		priceText.text = num.ToString("N0").Replace(",", " ");
	}

	private void HandleNotOwnedUI()
	{
		HandlePrices(accessoryDataClient);
		timeLimitDisplayer.Initialize(accessoryDataClient.time);
		timeLimitDisplayer.gameObject.SetActive(accessoryDataClient.time.IsTimeLimited);
		newAccessoryImage.SetActive(accessoryDataClient.iNew);
	}

	private void OnPreviewImageDownLoaded()
	{
		loadingWheel.SetActive(value: false);
		emptyFrame.SetActive(value: false);
		preview.gameObject.SetActive(value: true);
	}
}
