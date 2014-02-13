using System;

public class MVGUIAvatarAccessoryButtons : UXViewScript
{
	public UXIconButton avatarAccessoryInventoryButton;

	public UXIconButton avatarAccessoryShopButton;

	public CharacterEditorController CEController => MVGameController.Instance.CharacterEditorController;

	public override void OnInitialize()
	{
		base.OnInitialize();
		UXIconButton uXIconButton = avatarAccessoryInventoryButton;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			CEController.OpenAvatarAccessoryInventory();
		}));
		UXIconButton uXIconButton2 = avatarAccessoryShopButton;
		uXIconButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			CEController.OpenAvatarAccessoryShop();
		}));
	}
}
