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
	private ToolTip toolTip;

	private InventoryItem item;

	private InventoryItemPreviewer objectPreviewer;

	private bool initialized;

	public void Initialize(Transform rootTransform, InventoryItem item, MVWorldObjectClient woPreviewObject, bool draggable)
	{
		toolTip.SetText(item.name);
		dragHandler.enabled = draggable;
		this.item = item;
		metaData.Initialize(item.slotPosition);
		objectPreviewer = Object.Instantiate(objectPreviewerPrefab);
		objectPreviewer.Initialize(previewWidth, previewHeight, CameraClearFlags.Color, woPreviewObject.PreviewLayerMask, new Vector3(4.57f, 2f, 0.15f), rootTransform, new Vector3(100f, 100f, 10f * (float)item.slotPosition), item.name, woPreviewObject, woPreviewObject.GameObject);
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
		PlayerInventoryItemManager itemManager = Object.Instantiate(itemManagerPrefab);
		itemManager.Initialize(item, previewImage);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.InventoryUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(itemManager.gameObject, UIPushOption.Blocking, null, UIGroupFlags.InventoryUISubMenu);
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
