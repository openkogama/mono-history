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

	public void Initialize(Transform rootTransform, ShopItem item, MVWorldObjectClient woPreviewObject)
	{
		this.item = item;
		objectPreviewer = Object.Instantiate(objectPreviewerPrefab);
		objectPreviewer.Initialize(previewWidth, previewHeight, CameraClearFlags.Color, woPreviewObject.PreviewLayerMask, new Vector3(4.57f, 2f, 0.15f), rootTransform, new Vector3(100f, 100f, 10f * (float)item.slotPosition), item.name, woPreviewObject, woPreviewObject.GameObject);
		previewImage.texture = objectPreviewer.PreviewTexture;
		initialized = true;
	}

	public void SlotPressed()
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
}
