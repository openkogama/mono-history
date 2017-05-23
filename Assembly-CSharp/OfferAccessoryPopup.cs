using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OfferAccessoryPopup : MonoBehaviour
{
	private StreamingAssetInfo streamingAssetInfo;

	[SerializeField]
	private RawImage preview;

	[SerializeField]
	private Text accessoryName;

	[SerializeField]
	private Text goldPrice;

	[SerializeField]
	private Text extraSpinText;

	private AccessoryAttacher accessoryAttacher = new AccessoryAttacher();

	private int purchasedInventoryID;

	public void Initialize(StreamingAssetInfo streamingAssetInfo, Texture previewImage)
	{
		preview.texture = previewImage;
		this.streamingAssetInfo = streamingAssetInfo;
		accessoryName.text = streamingAssetInfo.Name;
		extraSpinText.text = string.Format(extraSpinText.text, (OffersManager.CurrentOffer as ActorOfferAccessory).extraSpins);
		goldPrice.text = streamingAssetInfo.ShopInfo.PriceGold.ToString();
	}

	public void Purchase()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		OffersManager.ClaimOffer();
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
			x.Create((MVPurchaseReturnCode)returnCode, int.Parse(goldPrice.text), 0);
		});
	}

	private void HandleSuccessfulPurchase(Dictionary<object, object> purchaseResponseData)
	{
		purchasedInventoryID = (int)purchaseResponseData[(byte)73];
		long ticks = (long)purchaseResponseData[(byte)83];
		DateTime purchaseTime = new DateTime(ticks);
		if (!MVGameControllerBase.Game.StreamingAssetInventory.Contains(purchasedInventoryID))
		{
			AddToInventory(purchasedInventoryID, purchaseTime, purchaseResponseData);
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(TM._("Successfully purchased accessory! Attach now?"), OnAttachConfirmation, string.Empty);
		});
	}

	private void OnAttachConfirmation(bool confirmed, ConfirmationPopup popup)
	{
		if (confirmed)
		{
			accessoryAttacher.AttachAccessory(purchasedInventoryID, MVGameControllerBase.WOCM.AvatarLocal.Body, CloseAd);
		}
		else
		{
			CloseAd();
		}
	}

	private void CloseAd()
	{
		Pop();
		Pop();
	}

	private void AddToInventory(int invID, DateTime purchaseTime, Dictionary<object, object> purchaseResponse)
	{
		int productID = streamingAssetInfo.ProductID;
		StreamingAssetInfo value = null;
		MVGameControllerBase.Game.StreamingAssetInfoMap.TryGetValue(productID, out value);
		if (value != null)
		{
			ProductInventoryInfo invInfo = new ProductInventoryInfo(invID, value, purchaseTime);
			MVGameControllerBase.Game.StreamingAssetInventory.Add(invInfo);
			MVGameControllerBase.Game.StreamingAssetInventory.NotifyProductInventoryChange();
		}
		else
		{
			Debug.LogError("Trying to add non-existing avatar accessory to inventory");
		}
	}

	public void Pop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	private void OnDestroy()
	{
		accessoryAttacher.Destroy();
	}
}
