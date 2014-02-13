using System;
using Localize;

public class MVGUIItemActionManage : MVGUIItemAction
{
	public UXTextButton deleteButton;

	protected override void Initialize()
	{
		UXTextButton uXTextButton = deleteButton;
		uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, new UXBaseButton.OnClickDelegate(AskForDeleteItem));
		base.Initialize();
	}

	private void AskForDeleteItem()
	{
		UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(TextSlotIndex.DeleteConfirm, TextSlotIndex.DeleteHeadline, UXDialogType.Simple, noButtons: false, stackDialog: true).AddPositiveButton(TextSlotIndex.Confirm)
			.AddNegativeButton(TextSlotIndex.Reject)
			.SetOnResultCallback(DeleteItem)
			.Show();
	}

	private void DeleteItem(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			MVGameController.Instance.Game.RemoveItemFromInventory(item.itemID);
			FireOnActionCompleted();
		}
	}
}
