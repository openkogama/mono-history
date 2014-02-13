using System;
using Localize;
using MV.WorldObject;
using UnityEngine;

public class MVGUICubeModel : MVGUIInventoryGroup
{
	public UXBucket deleteBucket;

	private UXCollectionViewItem dropItem;

	public override void Initialize()
	{
		base.Initialize();
		UXBucket uXBucket = deleteBucket;
		uXBucket.OnDropInto = (UXBucket.OnDropDelegate)Delegate.Combine(uXBucket.OnDropInto, (UXBucket.OnDropDelegate)((GameObject drop) =>
		{
			dropItem = collectionView.GetDraggedViewItem();
			if ((Object)(object)dropItem != (Object)null)
			{
				UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(TextSlotIndex.DeleteConfirm, TextSlotIndex.DeleteHeadline).AddPositiveButton(TextSlotIndex.Confirm)
					.AddNegativeButton(TextSlotIndex.Reject)
					.SetOnResultCallback(HandleRemoveItem)
					.Show();
			}
		}));
	}

	private void HandleRemoveItem(UXDialogBox dialog)
	{
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			IUXCollectionItem item = dropItem.Item;
			if (item != null)
			{
				int itemID = (item.Object as MVItem).itemID;
				MVGameController.Instance.Game.RemoveItemFromInventory(itemID);
			}
		}
	}

	protected override UXCollectionViewItem InstansiateViewItem(IUXCollectionItem item)
	{
		return base.InstansiateViewItem(item);
	}
}
