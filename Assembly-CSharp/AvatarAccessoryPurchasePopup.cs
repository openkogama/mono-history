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
	private Image preview;

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
	private AvatarAccessoryEquipPopup avatarAccessoryEquipPopup;

	[SerializeField]
	private AvatarAccessorySuccesPopup avatarAccessorySuccesPopup;

	private UnityAction refreshGoldCallback;

	private int price;

	protected MVBody AvatarBody;

	private void SetCurrentBody(MVBody body)
	{
		AvatarBody = body;
	}

	public void Initialize(AccessoryDataClient accessoryDataClient, Sprite previewImage, UnityAction refreshGoldCallback)
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
		preview.sprite = previewImage;
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
		if (returnCode == 0)
		{
			HandleSuccessfulPurchase(purchaseResponseData);
			return;
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create((MVPurchaseReturnCode)returnCode, price, 0);
		});
	}

	private void HandleSuccessfulPurchase(Dictionary<object, object> purchaseResponseData)
	{
		int toOwns = (int)purchaseResponseData[(byte)105];
		AccessoryDataManager.SetToOwns(toOwns);
		Debug.LogWarning("HandleSuccessfulPurchase");
		SuccesfulPopupCallBack();
		refreshGoldCallback();
	}

	private void SuccesfulPopupCallBack()
	{
		AvatarAccessoryEquipPopup popup = UnityEngine.Object.Instantiate(avatarAccessoryEquipPopup);
		popup.Initialize(EquipPopupResultCallback, preview.sprite, accessoryDataClient);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.InventoryUISubMenu);
		});
	}

	private void EquipPopupResultCallback(bool attach)
	{
		if (attach)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAttachToBody x, BaseEventData y) =>
			{
				x.AttachToBody(accessoryDataClient.streamingAssetID, AvatarBody.GetAccessoryOffset(accessoryDataClient.accessorySlotType), AvatarBody.GetAccessoryScale(accessoryDataClient.accessorySlotType));
			});
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.InventoryUISubMenu);
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
