public class MVGUIPublishDialog : UXCustomDialogBox
{
	public UXToggleIconButton publishToFacebookToggle;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		publishToFacebookToggle.SetToggleState(toggle: true);
	}

	public override object GetResult()
	{
		return publishToFacebookToggle.ToggleState;
	}
}
