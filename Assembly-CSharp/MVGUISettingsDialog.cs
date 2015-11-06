public abstract class MVGUISettingsDialog
{
	protected UXDialogFactory dialogFactory;

	protected MVWorldObjectClient wo;

	public MVGUISettingsDialog()
	{
		if (dialogFactory == null)
		{
			dialogFactory = UXUtils.UXDialogFactory;
		}
		wo = MVGameControllerLegacyUI.EditorController.GetSettingsDialogSelectionWO();
	}
}
