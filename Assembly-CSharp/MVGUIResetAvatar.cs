using System;
using UnityEngine;

public class MVGUIResetAvatar : UXViewScript
{
	public UXIconButton resetAvatarButton;

	private bool avatarResetPending;

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
			if (avatarResetPending)
			{
				Debug.LogWarning("Avatar reset is pending");
				return;
			}
			avatarResetPending = true;
			World world = MVGameControllerBase.Game.World;
			world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedResetAvatar));
			MVGameControllerBase.Game.ResetAvatar((MVGameControllerLegacyUI.IngameController as CharacterEditorController).MarkAndReturnCurrentAvatarID());
		}
	}

	private void InitializedResetAvatar(object sender, InitializedGameQueryDataEventArgs e)
	{
		World world = MVGameControllerBase.Game.World;
		world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(InitializedResetAvatar));
		avatarResetPending = false;
		if (e.RootWO != null)
		{
			Debug.Log("Reset avatar has been added to world. WorldObjectId is: " + e.RootWO);
			MVGameControllerLegacyUI.CharacterEditorController.SubstituteAvatar(e.RootWO.Id);
		}
	}
}
