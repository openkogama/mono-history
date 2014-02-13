using Localize;

public class MVGUIPressM : UXViewScript
{
	public UXText pressMText;

	public void ShowStandardText()
	{
		pressMText.Text = Localization.Instance.GetText(TextSlotIndex.SHOW_MENU);
	}

	public void ShowOpenText(bool shortcut)
	{
		if (shortcut)
		{
			pressMText.Text = Localization.Instance.GetText(TextSlotIndex.HIDE_MENU_RELEASE);
		}
		else
		{
			pressMText.Text = Localization.Instance.GetText(TextSlotIndex.HIDE_MENU_PRESS);
		}
	}

	public void HideText()
	{
		pressMText.Text = string.Empty;
	}
}
