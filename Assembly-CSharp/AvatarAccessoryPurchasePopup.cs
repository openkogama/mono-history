using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class AvatarAccessoryPurchasePopup : MonoBehaviour
{
	private AccessoryDataClient accessoryDataClient;

	[SerializeField]
	private RawImage preview;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private Text originalPriceText;

	[SerializeField]
	private GameObject discountTag;

	[SerializeField]
	private Text discountTagText;

	[SerializeField]
	private Text goldSavedText;

	[SerializeField]
	private AccessoryItemBackground accessoryItemBackground;

	[SerializeField]
	private AccessoryTimeLimitDisplayer timeLimitDisplayer;

	[SerializeField]
	private RawImage levelRequirement;

	[SerializeField]
	private GameObject newAccessoryImage;

	[SerializeField]
	private AvatarAccessorySuccesPopup avatarAccessorySuccesPopup;

	[SerializeField]
	private AvatarAccessoryErrorPopup insufficientResourcesPopup;

	[SerializeField]
	private PurchasedAccessoryPreviewer successPreviewer;

	private UnityAction refreshGoldCallback;

	private int price;

	protected MVBody AvatarBody;

	private void SetCurrentBody(MVBody body)
	{
		AvatarBody = body;
	}

	public void Initialize(AccessoryDataClient accessoryDataClient, Texture previewImage, UnityAction refreshGoldCallback)
	{
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IGetCurrentBody x, BaseEventData y) =>
			{
				x.GetCurrentBody(SetCurrentBody);
			});
		}
		else
		{
			AvatarBody = MVGameControllerBase.Game.LocalPlayer.Avatar.Body;
		}
		this.refreshGoldCallback = refreshGoldCallback;
		preview.texture = previewImage;
		this.accessoryDataClient = accessoryDataClient;
		priceText.text = accessoryDataClient.priceGold.ToString();
		price = accessoryDataClient.priceGold;
		goldSavedText.gameObject.SetActive(value: false);
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
		MVGameControllerBase.OperationRequests.PurchaseAvatarAccessory(accessoryDataClient.streamingAssetID);
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
			x.Create((MVPurchaseReturnCode)returnCode, price, 0);
		});
	}

	private void HandleInsufficientResources(string header, string buttonText)
	{
		AvatarAccessoryErrorPopup confirmationPopup = UnityEngine.Object.Instantiate(insufficientResourcesPopup);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(confirmationPopup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
		confirmationPopup.Initialize(OnGoldPurchaseDialogResult, preview.mainTexture, accessoryDataClient, header, buttonText);
	}

	private void OnGoldPurchaseDialogResult(bool result)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		if (result)
		{
			BrowserComm.ToJavaScript.ExternalCall("gotoPurchaseGold");
			BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.purchaseGoldURL);
		}
	}

	private void HandleSuccessfulPurchase(Dictionary<object, object> purchaseResponseData)
	{
		int num = (int)purchaseResponseData[(byte)105];
		AccessoryDataManager.SetToOwns(num);
		SuccessfulPopupCallBack(num);
		refreshGoldCallback();
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
		int priceGold = streamingAssetInfo.priceGold;
		int discount = streamingAssetInfo.discount;
		int num = priceGold;
		originalPriceText.gameObject.SetActive(discount > 0);
		discountTag.SetActive(discount > 0);
		if (discount > 0)
		{
			discountTagText.text = ((discount < 100) ? ("-" + discount + "%") : "FREE");
			int num2 = Mathf.FloorToInt((float)priceGold * ((float)discount / 100f));
			num = priceGold - num2;
			originalPriceText.text = priceGold.ToString("N0");
			goldSavedText.gameObject.SetActive(value: true);
			goldSavedText.text = num2.ToString("N0");
		}
		priceText.text = num.ToString("N0");
	}

	private void HandleNotOwnedUI()
	{
		HandlePrices(accessoryDataClient);
		timeLimitDisplayer.Initialize(accessoryDataClient.timelimit);
		timeLimitDisplayer.gameObject.SetActive(accessoryDataClient.timelimit.IsTimeLimited);
		if (accessoryDataClient.level > 0)
		{
			BadgeManager.GetBadgeTexture(accessoryDataClient.level, OnLevelRequirementLoaded);
		}
		newAccessoryImage.SetActive(accessoryDataClient.isNew);
	}

	private void OnLevelRequirementLoaded(WWW www)
	{
		if (www == null || www.texture == null)
		{
			Debug.LogWarning("Badge not loaded for accessory level requirement");
			return;
		}
		levelRequirement.texture = www.texture;
		levelRequirement.gameObject.SetActive(value: true);
	}
}
