using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInventoryDeleteTab : ManageItemPage
{
	[SerializeField]
	private Text itemName;

	[SerializeField]
	private RawImage preview;

	private InventoryItem previewedItem;

	public override void Initialize(RawImage image, InventoryItem item)
	{
		previewedItem = item;
		preview.texture = image.texture;
		itemName.text = item.name;
	}

	public void OnRemoveFromInventory()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(TM._("This will remove the current item from your inventory and marketplace. Are you sure you wish to do this?"), OnConfirmation, TM._("Remove"));
		});
	}

	public void OnConfirmation(bool affirmative, ConfirmationPopup popup)
	{
		popup.Pop();
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		if (affirmative)
		{
			MVGameControllerBase.Game.RemoveItemFromInventory(previewedItem.itemID);
			MVGameControllerBase.IEditModeUI.PlayerInventoryRepository.RemoveItem(previewedItem);
			string text = itemName.text;
			if (itemName.text == string.Empty)
			{
				text = "item";
			}
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create(text + TM._(" was removed from inventory."), string.Empty);
			});
		}
	}
}
