using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class MaterialPurchasePopup : MonoBehaviour
{
	private Dictionary<object, object> purchaseResponseData;

	private UnityAction<bool, Dictionary<object, object>> callback;

	private byte materialID;

	[SerializeField]
	private Text price;

	[SerializeField]
	private Text productName;

	[SerializeField]
	private Text description;

	[SerializeField]
	private RawImage materialPreviewImage;

	[SerializeField]
	private MaterialPreviewer materialPreviewer;

	[SerializeField]
	private GameObject purchaseButton;

	public void Initialize(byte materialID, UnityAction<bool, Dictionary<object, object>> callback)
	{
		this.materialID = materialID;
		this.callback = callback;
		MVMaterial material = MVGameControllerBase.Game.MaterialRepository.GetMaterial(materialID);
		price.text = material.unlockPriceGold.ToString();
		if (material.isUnlocked)
		{
			purchaseButton.SetActive(value: false);
		}
		productName.text = MaterialDescription.materialDescriptions[materialID].Name;
		materialPreviewer = UnityEngine.Object.Instantiate(materialPreviewer);
		materialPreviewer.Initialize(material.mesh);
		materialPreviewImage.texture = materialPreviewer.renderTexture;
		description.text = MaterialDescription.materialDescriptions[materialID].Description;
	}

	private void OnDestroy()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		UnityEngine.Object.Destroy(materialPreviewer.gameObject);
	}

	public void OnPurchaseClick()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create();
		});
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		MVGameControllerBase.OperationRequests.UnlockMaterial(materialID);
	}

	private void ProductPurchaseResponseHandler(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		Debug.Log("Material unlocked");
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		if (returnCode == 0)
		{
			MVGameControllerBase.Game.MaterialRepository.SetMaterialUnlocked(materialID, unlocked: true);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			callback(arg0: true, purchaseResponseData);
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create((MVPurchaseReturnCode)returnCode, int.Parse(price.text), 0);
			});
		}
	}
}
