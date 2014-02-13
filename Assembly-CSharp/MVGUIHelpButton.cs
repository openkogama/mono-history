using UnityEngine;

public class MVGUIHelpButton : UXViewScript
{
	public MVGUIHelp helpWindow;

	public UXIconButton helpButton;

	public override void OnShow()
	{
		base.OnShow();
		((Component)helpButton).gameObject.SetActiveRecursively(true);
	}

	public override void OnHide()
	{
		base.OnHide();
		((Component)helpButton).gameObject.SetActiveRecursively(false);
		helpWindow.View.Hide();
	}

	public override void OnInitialize()
	{
		helpButton.OnClick = ShowHelp;
	}

	public void ShowHelp()
	{
		helpWindow.View.Show();
	}
}
