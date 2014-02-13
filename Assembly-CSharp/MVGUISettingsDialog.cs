using UnityEngine;

public abstract class MVGUISettingsDialog
{
	protected UXDialogFactory dialogFactory;

	protected MVWorldObjectClient wo;

	public MVGUISettingsDialog()
	{
		if ((Object)(object)dialogFactory == (Object)null)
		{
			dialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
		}
		wo = MVGameController.Instance.EditorController.GetSettingsDialogSelectionWO();
	}
}
