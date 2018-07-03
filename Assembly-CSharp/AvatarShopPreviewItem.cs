using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarShopPreviewItem : MonoBehaviour
{
	[SerializeField]
	private int previewWidth;

	[SerializeField]
	private int previewHeight;

	[SerializeField]
	private RawImage previewImage;

	[SerializeField]
	private AvatarRepositoryItem item;

	[SerializeField]
	private AvatarPurchasePopup popup;

	[SerializeField]
	private AvatarPreviewer previewer;

	public void InitializeObjectPreview(AvatarRepositoryItem item, MVWorldObjectClient wo, Transform previewItemsRoot)
	{
		this.item = item;
		previewer = Object.Instantiate(previewer);
		previewer.Initialize(previewWidth, previewHeight, CameraClearFlags.Color, wo.PreviewLayerMask, new Vector3(0f, 0f, 0f), previewItemsRoot, new Vector3(100f, 100f, 10f * (float)item.slotPosition), item.name, wo, wo.GameObject, default);
		previewer.PreviewGameObject.transform.Rotate(0f, 180f, 0f);
		previewImage.texture = previewer.PreviewTexture;
	}

	public void SlotPressed()
	{
		AvatarPurchasePopup purchasePopup = Object.Instantiate(popup);
		purchasePopup.Initialize(previewImage, item);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(purchasePopup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.InventoryUISubMenu);
		});
	}

	public void Update()
	{
		if (previewer != null)
		{
			previewer.UpdateRotation(9.3f);
		}
	}
}
