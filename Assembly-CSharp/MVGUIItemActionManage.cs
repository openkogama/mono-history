using System;

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
		UXUtils.UXDialogFactory.CreateDialog(TM._("Are you sure\nwant to delete?"), TM._("Delete"), UXDialogType.Simple, noButtons: false, stackDialog: true).AddPositiveButton(TM._("Yes")).AddNegativeButton(TM._("No"))
			.SetOnResultCallback(DeleteItem)
			.Show();
	}

	private void DeleteItem(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			MVGameController.Game.RemoveItemFromInventory(item.itemID);
			FireOnActionCompleted();
		}
	}
}
