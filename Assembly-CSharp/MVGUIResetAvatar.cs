using System;
using Localize;
using UnityEngine;

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
		if (!((Object)(object)UXUtils.FindGUIObjectOfType<UXDialogFactory>().CurrentDialogBox != (Object)null))
		{
			UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(TextSlotIndex.ResestAvatarConfirm, TextSlotIndex.ResetAvatarHeadline).AddPositiveButton(TextSlotIndex.Confirm)
				.AddNegativeButton(TextSlotIndex.Reject)
				.SetOnResultCallback(ResetAvatarCallBack)
				.Show();
		}
	}

	private void ResetAvatarCallBack(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			MVGameController.Instance.Game.ResetAvatar((MVGameController.Instance.IngameController as CharacterEditorController).MarkAndReturnCurrentAvatarID());
		}
	}
}
