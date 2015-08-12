public class MVGUIPressM : UXViewScript
{
	public UXText pressMText;

	public void ShowStandardText()
	{
		pressMText.Text = TM._("Press M to show Menu");
	}

	public void ShowOpenText(bool shortcut)
	{
		if (shortcut)
		{
			pressMText.Text = TM._("Release TAB to hide Menu");
		}
		else
		{
			pressMText.Text = TM._("Press M to hide Menu");
		}
	}

	public void HideText()
	{
		pressMText.Text = string.Empty;
	}
}
