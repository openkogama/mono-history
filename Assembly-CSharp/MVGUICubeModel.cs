using System;
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
			if (dropItem != null)
			{
				UXUtils.UXDialogFactory.CreateDialog(TM._("Are you sure\nwant to delete?"), TM._("Delete")).AddPositiveButton(TM._("Yes")).AddNegativeButton(TM._("No"))
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
				MVGameController.Game.RemoveItemFromInventory(itemID);
			}
		}
	}

	protected override UXCollectionViewItem InstansiateViewItem(IUXCollectionItem item)
	{
		return base.InstansiateViewItem(item);
	}
}
