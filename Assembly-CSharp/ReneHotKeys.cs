using UnityEngine;

public static class ReneHotKeys
{
	public static void Handle()
	{
		if (MVInputWrapper.DebugGetKeyDown(KeyCode.D))
		{
			UXDialogFactory uXDialogFactory = UXUtils.UXDialogFactory;
			uXDialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/SelectDevToolDialog", "Dev Tools", noButtons: true, stackDialog: true).Show();
		}
	}
}
