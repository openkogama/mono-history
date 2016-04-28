using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class AccessoryAdCreator : MonoBehaviour
{
	[SerializeField]
	private OfferAccessoryPopup popupPrefab;

	private GameObject root;

	private StreamingAssetInfo streamingAssetInfo;

	private ObjectPreviewer previewer;

	private bool isSubscribedToPurchaseResponse;

	private UnityAction OnAccessoryPoppedCallback;

	public void Initialize(UnityAction OnAccessoryPopped, ActorOfferAccessory accessoryOffer)
	{
		OnAccessoryPoppedCallback = OnAccessoryPopped;
		root = new GameObject("Accessory Ad root");
		GenerateOfferItem(accessoryOffer);
		if (streamingAssetInfo == null)
		{
			OnPop();
		}
		else
		{
			AvatarAccessory.Create(streamingAssetInfo, AccessoryCreatedCallback);
		}
	}

	private void AccessoryCreatedCallback(AvatarAccessory avatarAccessory)
	{
		previewer = ObjectPreviewer.Create(512, CameraClearFlags.Color, LayerFlags.Default | LayerFlags.CamRotateTarget, root.transform, streamingAssetInfo.Name, avatarAccessory.gameObject);
		OfferAccessoryPopup popup = UnityEngine.Object.Instantiate(popupPrefab);
		popup.Initialize(streamingAssetInfo, previewer.PreviewTexture);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup.gameObject, UIPushOption.Blocking, OnPop, UIGroupFlags.Popup);
		});
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		isSubscribedToPurchaseResponse = true;
	}

	private void ProductPurchaseResponseHandler(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		isSubscribedToPurchaseResponse = false;
		if (returnCode == 0)
		{
			Debug.Log("Whoop! purchased previously unowned item, grats! here is some debug logs as a reward");
		}
	}

	private void OnPop()
	{
		if (root != null)
		{
			UnityEngine.Object.Destroy(root);
		}
		if (isSubscribedToPurchaseResponse)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		}
		OnAccessoryPoppedCallback();
	}

	private void Update()
	{
		if (previewer != null)
		{
			previewer.UpdateRotation();
		}
	}

	private void GenerateOfferItem(ActorOfferAccessory accessoryOffer)
	{
		streamingAssetInfo = null;
		MVGameControllerBase.Game.StreamingAssetInfoMap.TryGetValue(accessoryOffer.streamingAssetsId, out streamingAssetInfo);
	}

	private bool TryGetProductInventoryInfo(out ProductInventoryInfo productInventoryInfo, int productID, List<ProductInventoryInfo> productInventoryInfos)
	{
		foreach (ProductInventoryInfo productInventoryInfo2 in productInventoryInfos)
		{
			if (productInventoryInfo2.ProductInfo.ProductID == productID)
			{
				productInventoryInfo = productInventoryInfo2;
				return true;
			}
		}
		productInventoryInfo = null;
		return false;
	}
}
