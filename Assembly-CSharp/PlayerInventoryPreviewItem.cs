using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerInventoryPreviewItem : MonoBehaviour
{
	[SerializeField]
	private RawImage previewImage;

	[SerializeField]
	private int previewWidth;

	[SerializeField]
	private int previewHeight;

	[SerializeField]
	private InventoryItemDragHandler dragHandler;

	[SerializeField]
	private PlayerInventoryItemManager itemManagerPrefab;

	[SerializeField]
	private InventoryItemMetaData metaData;

	[SerializeField]
	private InventoryItemPreviewer objectPreviewerPrefab;

	[SerializeField]
	private InventoryItemPreview itemPreviewerPrefab;

	[SerializeField]
	private InventoryItemCubeModelHandler itemPreviewerCubeModelPrefab;

	[SerializeField]
	private ToolTip toolTip;

	private InventoryItem item;

	private InventoryItemPreviewer objectPreviewer;

	private bool initialized;

	public MVWorldObjectDocumentationType DocumentationType;

	public void Initialize(Transform rootTransform, InventoryItem item, MVWorldObjectClient woPreviewObject, bool draggable)
	{
		item.ApplyLocalDescriptionOverride(woPreviewObject.DocumentationType);
		DocumentationType = woPreviewObject.DocumentationType;
		toolTip.SetText(item.name);
		dragHandler.enabled = draggable;
		this.item = item;
		metaData.Initialize(item.slotPosition);
		objectPreviewer = Object.Instantiate(objectPreviewerPrefab);
		Vector3 previewPosition = new Vector3(100f, 100f, 10f * (float)item.slotPosition);
		Vector3 cameraOffset = new Vector3(0f, 0f, 0f);
		if (InventoryItem.localItemDescriptionOverride.ContainsKey(woPreviewObject.DocumentationType))
		{
			cameraOffset = InventoryItem.localItemDescriptionOverride[woPreviewObject.DocumentationType].CameraPreviewerOffset;
		}
		objectPreviewer.Initialize(previewWidth, previewHeight, CameraClearFlags.Color, woPreviewObject.PreviewLayerMask, cameraOffset, rootTransform, previewPosition, item.name, woPreviewObject, woPreviewObject.GameObject);
		previewImage.texture = objectPreviewer.PreviewTexture;
		initialized = true;
	}

	private void OnDestroy()
	{
		Object.Destroy(objectPreviewer);
	}

	public InventoryItemPreviewer GetPreviewer()
	{
		return objectPreviewer;
	}

	public InventoryItem GetItem()
	{
		return item;
	}

	public void AdditionalItemSettingsPressed()
	{
		InventoryItemPreview itemPreviewer = null;
		if (item.resellable && !item.isDefaultInvItem)
		{
			if (item.itemCategoryID != 1)
			{
				int num = MVGameControllerBase.EditModeUI.PlayerInventoryRepository.CountItemsWithOriginalID(item);
				if (num > 1)
				{
					itemPreviewer = Object.Instantiate(itemPreviewerCubeModelPrefab);
				}
			}
			else
			{
				itemPreviewer = Object.Instantiate(itemPreviewerCubeModelPrefab);
			}
		}
		if (itemPreviewer == null)
		{
			itemPreviewer = Object.Instantiate(itemPreviewerPrefab);
		}
		itemPreviewer.Initialize(item, previewImage);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.InventoryUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(itemPreviewer.gameObject, UIPushOption.Blocking, null, UIGroupFlags.InventoryUISubMenu);
		});
	}

	public void SlotPressed()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAddItemFromInventory x, BaseEventData y) =>
		{
			x.OnAddItemFromInventory(item);
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
