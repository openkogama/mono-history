using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AccessoryInventoryViewItem : MonoBehaviour
{
	private bool locked;

	private StreamingAssetInfo streamingAssetInfo;

	private ProductInventoryInfo productInventoryInfo;

	[SerializeField]
	private Image lockedImage;

	[SerializeField]
	private RawImage image;

	[SerializeField]
	private GameObject loadingWheel;

	[SerializeField]
	private Button purchasePopupButton;

	[SerializeField]
	private AvatarAccessoryPurchasePopup purchasePopupPrefab;

	private Transform rootTransform;

	private ObjectPreviewer objectPreviewer;

	public void Initialize(StreamingAssetInfo streamingAssetInfo, ProductInventoryInfo productInventoryInfo, Transform rootTransform)
	{
		this.rootTransform = rootTransform;
		locked = productInventoryInfo == null;
		this.productInventoryInfo = productInventoryInfo;
		image.enabled = false;
		lockedImage.gameObject.SetActive(locked);
		loadingWheel.SetActive(value: true);
		this.streamingAssetInfo = streamingAssetInfo;
		AvatarAccessory.Create(streamingAssetInfo, AccessoryCreatedCallback);
	}

	public void OnClicked()
	{
		if (locked)
		{
			AvatarAccessoryPurchasePopup purchasePopup = Object.Instantiate(purchasePopupPrefab);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(purchasePopup.gameObject, UIPushOption.Blocking, OnPurchasePopupPop);
			});
			purchasePopup.Initialize(streamingAssetInfo, image);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IPreviewAccessoryShopItem x, BaseEventData y) =>
			{
				x.PreviewAccessoryShopItem(streamingAssetInfo);
			});
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAttachToBody x, BaseEventData y) =>
			{
				x.AttachToBody(productInventoryInfo.InventoryID);
			});
		}
	}

	private void OnPurchasePopupPop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IPreviewAccessoryShopItem x, BaseEventData y) =>
		{
			x.PreviewAccessoryShopItem(null);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IInventoryChanged x, BaseEventData y) =>
		{
			x.InventoryChanged();
		});
	}

	private void Update()
	{
		if (objectPreviewer != null)
		{
			objectPreviewer.UpdateRotation();
		}
	}

	private void AccessoryCreatedCallback(AvatarAccessory avatarAccessory)
	{
		if (gameObject == null)
		{
			Debug.LogWarning("Subscribing game object destroyed. Currently the accessory system does not support removal of callback when view item is destroyed.");
			return;
		}
		image.enabled = true;
		purchasePopupButton.enabled = true;
		loadingWheel.SetActive(value: false);
		objectPreviewer = ObjectPreviewer.Create(256, CameraClearFlags.Color, LayerFlags.Default | LayerFlags.CamRotateTarget, rootTransform, streamingAssetInfo.Name, avatarAccessory.gameObject);
		image.texture = objectPreviewer.PreviewTexture;
	}
}
