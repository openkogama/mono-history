public class MVGUIHelpButton : UXViewScript
{
	public MVGUIHelp helpWindow;

	public UXIconButton helpButton;

	public override void OnShow()
	{
		base.OnShow();
		helpButton.gameObject.SetActive(value: true);
	}

	public override void OnHide()
	{
		base.OnHide();
		helpButton.gameObject.SetActive(value: false);
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
