using System;

public class MVGUIAvatarShopButton : UXViewScript
{
	public UXIconButton AvatarShopButton;

	public override void OnInitialize()
	{
		UXIconButton avatarShopButton = AvatarShopButton;
		avatarShopButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(avatarShopButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			MVGameController.Instance.CharacterEditorController.ShowAvatarShopWindow();
		}));
	}
}
