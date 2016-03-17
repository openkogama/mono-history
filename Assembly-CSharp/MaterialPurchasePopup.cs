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
	private GameObject waitOverLay;

	[SerializeField]
	private Text price;

	[SerializeField]
	private Text productName;

	[SerializeField]
	private Button purchase;

	[SerializeField]
	private RawImage materialPreviewImage;

	[SerializeField]
	private MaterialPreviewer materialPreviewer;

	public void Initialize(byte materialID, UnityAction<bool, Dictionary<object, object>> callback)
	{
		this.materialID = materialID;
		this.callback = callback;
		purchase.onClick.AddListener(OnPurchaseClick);
		MVMaterial material = MVGameControllerBase.Game.MaterialRepository.GetMaterial(materialID);
		price.text = material.unlockPriceGold.ToString();
		productName.text = material.name;
		materialPreviewer = UnityEngine.Object.Instantiate(materialPreviewer);
		materialPreviewer.Initialize(MVGameControllerBase.Game.MaterialRepository.GetMaterial(materialID).mesh);
		materialPreviewImage.texture = materialPreviewer.renderTexture;
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(materialPreviewer.gameObject);
	}

	private void OnPurchaseClick()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		MVGameControllerBase.Game.UnlockMaterial(materialID);
		purchase.gameObject.SetActive(value: false);
		waitOverLay.SetActive(value: true);
	}

	private void ProductPurchaseResponseHandler(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		Debug.Log("Material unlocked");
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Remove(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
		if (returnCode == 0)
		{
			callback(arg0: true, purchaseResponseData);
			MVGameControllerBase.Game.MaterialRepository.SetMaterialUnlocked(materialID, unlocked: true);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
		}
		else
		{
			callback(arg0: false, purchaseResponseData);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create((MVPurchaseReturnCode)returnCode, int.Parse(price.text), 0);
			});
			waitOverLay.SetActive(value: false);
		}
	}
}
