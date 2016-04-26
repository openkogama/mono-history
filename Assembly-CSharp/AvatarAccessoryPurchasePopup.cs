using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarAccessoryPurchasePopup : MonoBehaviour
{
	private StreamingAssetInfo streamingAssetInfo;

	[SerializeField]
	private RawImage preview;

	[SerializeField]
	private Text accessoryName;

	[SerializeField]
	private Text goldPrice;

	[SerializeField]
	private GameObject waitOverLay;

	public void Initialize(StreamingAssetInfo streamingAssetInfo, RawImage previewImage)
	{
		preview.texture = previewImage.texture;
		this.streamingAssetInfo = streamingAssetInfo;
		accessoryName.text = streamingAssetInfo.Name;
		goldPrice.text = streamingAssetInfo.ShopInfo.PriceGold.ToString();
	}

	public void Purchase()
	{
		waitOverLay.SetActive(value: true);
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		MVGameControllerBase.Game.PurchaseAvatarAccessory(streamingAssetInfo.ProductID);
	}

	private void ProductPurchaseResponseHandler(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		waitOverLay.SetActive(value: false);
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
		int num = (int)purchaseResponseData[(byte)73];
		long ticks = (long)purchaseResponseData[(byte)83];
		DateTime purchaseTime = new DateTime(ticks);
		if (!MVGameControllerBase.Game.StreamingAssetInventory.Contains(num))
		{
			AddToInventory(num, purchaseTime, purchaseResponseData);
		}
		Pop();
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create("Successfully purchased accessory!", string.Empty);
		});
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
		waitOverLay.SetActive(value: false);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}
}
