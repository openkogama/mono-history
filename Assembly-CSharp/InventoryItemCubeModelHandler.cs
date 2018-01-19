using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemCubeModelHandler : InventoryItemPreview
{
	[SerializeField]
	private InventoryItemPreviewSell inventoryItemPreviewSellPrefab;

	private InventoryItem previewedItem;

	public override void Initialize(InventoryItem item, RawImage image)
	{
		base.Initialize(item, image);
		previewedItem = item;
	}

	public void OnSellClicked()
	{
		InventoryItemPreviewSell itemSell = Object.Instantiate(inventoryItemPreviewSellPrefab);
		itemSell.Initialize(previewImage, item);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(itemSell.gameObject, UIPushOption.Blocking, OnSellUpdated, UIGroupFlags.InventoryUISubMenu);
		});
	}

	private void OnSellUpdated()
	{
		title.text = previewedItem.name;
		description.text = previewedItem.description;
	}

	public void OnDeleteClicked()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(TM._("This will remove the current item from your inventory and shop. Are you sure you wish to do this?"), OnDeleteConfirmation, TM._("Remove"));
		});
	}

	private void Update()
	{
		MVInputWrapper.SuppressInGameInput();
		MVInputWrapper.SuppressAllInput();
	}

	public void OnDeleteConfirmation(bool affirmative, ConfirmationPopup popup)
	{
		popup.Pop();
		if (affirmative)
		{
			MVGameControllerBase.OperationRequests.RemoveItemFromInventory(previewedItem.itemID);
			MVGameControllerBase.IEditModeUI.PlayerInventoryRepository.RemoveItem(previewedItem);
			string text = previewedItem.name;
			if (string.IsNullOrEmpty(text))
			{
				text = "item";
			}
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Pop();
			});
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(text + TM._(" was removed from inventory."), string.Empty);
			});
		}
	}
}
