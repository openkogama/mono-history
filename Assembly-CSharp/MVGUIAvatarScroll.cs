using System;

public class MVGUIAvatarScroll : UXViewScript
{
	public UXIconButton scrollRightButton;

	public UXIconButton scrollLeftButton;

	public UXText indexText;

	private CharacterEditorController CEController => MVGameController.CharacterEditorController;

	public override void OnInitialize()
	{
		UXIconButton uXIconButton = scrollRightButton;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			CEController.ShiftAvatarIndex(forward: true);
		}));
		UXIconButton uXIconButton2 = scrollLeftButton;
		uXIconButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			CEController.ShiftAvatarIndex(forward: false);
		}));
	}

	public void UpdateBodyIndex()
	{
		scrollRightButton.SetVisible(CEController.BodyCount > 1);
		scrollLeftButton.SetVisible(CEController.BodyCount > 1);
		indexText.SetVisible(CEController.BodyCount > 1);
		indexText.Text = $"{CEController.CurrentBodyIndex + 1}/{CEController.BodyCount}";
	}
}
