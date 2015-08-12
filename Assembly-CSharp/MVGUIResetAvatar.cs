using System;

public class MVGUIResetAvatar : UXViewScript
{
	public UXIconButton resetAvatarButton;

	public override void OnInitialize()
	{
		UXIconButton uXIconButton = resetAvatarButton;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			ShowResetDialog();
		}));
	}

	private void ShowResetDialog()
	{
		if (!(UXUtils.UXDialogFactory.CurrentDialogBox != null))
		{
			UXUtils.UXDialogFactory.CreateDialog(TM._("This will remove any changes you've made.\nAre you sure?"), TM._("Reset Avatar")).AddPositiveButton(TM._("Yes")).AddNegativeButton(TM._("No"))
				.SetOnResultCallback(ResetAvatarCallBack)
				.Show();
		}
	}

	private void ResetAvatarCallBack(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			MVGameController.Game.ResetAvatar((MVGameController.IngameController as CharacterEditorController).MarkAndReturnCurrentAvatarID());
		}
	}
}
