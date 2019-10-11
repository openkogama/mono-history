using MV.WorldObject.Subscription;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditModeClientShopItem : MonoBehaviour
{
	[SerializeField]
	private RawImage previewImage;

	[SerializeField]
	private int previewWidth;

	[SerializeField]
	private int previewHeight;

	[SerializeField]
	private ItemPurchasePopup popup;

	[SerializeField]
	private InventoryItemPreviewer objectPreviewerPrefab;

	private ShopItem item;

	private InventoryItemPreviewer objectPreviewer;

	private bool initialized;

	private MVWorldObjectDocumentationType documentationType;

	public MVWorldObjectDocumentationType DocumentationType => documentationType;

	public void Initialize(Transform rootTransform, ShopItem item, MVWorldObjectClient woPreviewObject)
	{
		this.item = item;
		item.ApplyLocalDescriptionOverride(woPreviewObject.DocumentationType);
		objectPreviewer = Object.Instantiate(objectPreviewerPrefab);
		Vector3 previewPosition = new Vector3(100f, 100f, 10f * (float)item.slotPosition);
		Vector3 cameraOffset = new Vector3(0f, 0f, 0f);
		if (InventoryItem.localItemDescriptionOverride.ContainsKey(woPreviewObject.DocumentationType))
		{
			cameraOffset = InventoryItem.localItemDescriptionOverride[woPreviewObject.DocumentationType].CameraPreviewerOffset;
		}
		objectPreviewer.Initialize(previewWidth, previewHeight, CameraClearFlags.Color, woPreviewObject.PreviewLayerMask, cameraOffset, rootTransform, previewPosition, item.name, woPreviewObject, woPreviewObject.GameObject);
		previewImage.texture = objectPreviewer.PreviewTexture;
		documentationType = woPreviewObject.DocumentationType;
		initialized = true;
	}

	public void SlotPressed()
	{
		if (MVGameControllerBase.Game.LocalPlayer.SubscriptionRules.HasBenefit(SubscriptionBenefit.FreeBuildingGameObjects))
		{
			InventoryItem inventoryItem = new InventoryItem(item);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
			{
				handler.PopGroups(UIGroupFlags.InventoryUI);
			});
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAddItemFromInventory x, BaseEventData y) =>
			{
				x.OnAddItemFromInventory(inventoryItem);
			});
		}
		else
		{
			ShowPurchasePopUp();
		}
	}

	public void ShowPurchasePopUp()
	{
		ItemPurchasePopup purchasePopup = Object.Instantiate(popup);
		purchasePopup.Initialize(previewImage, item);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.InventoryUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(purchasePopup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.InventoryUISubMenu);
		});
	}

	public void Update()
	{
		if (initialized)
		{
			objectPreviewer.UpdateRotation();
		}
	}

	public InventoryItem GetItem()
	{
		return new InventoryItem(item);
	}
}
