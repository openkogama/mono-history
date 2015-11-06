using System;

public class MVGUIAvatarAccessoryButtons : UXViewScript
{
	public UXIconButton avatarAccessoryInventoryButton;

	public UXIconButton avatarAccessoryShopButton;

	public CharacterEditorController CEController => MVGameControllerLegacyUI.CharacterEditorController;

	public override void OnInitialize()
	{
		base.OnInitialize();
		UXIconButton uXIconButton = avatarAccessoryInventoryButton;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			UXUtils.FindGUIObjectOfType<AvatarAccessoryController>().OpenAvatarAccessoryInventory();
		}));
		UXIconButton uXIconButton2 = avatarAccessoryShopButton;
		uXIconButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			UXUtils.FindGUIObjectOfType<AvatarAccessoryController>().OpenAvatarAccessoryShop();
		}));
	}
}
