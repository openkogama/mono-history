using System.Collections.Generic;

public class MVGUISettingsDialogKillLimit : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogKillLimit()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/KillLimitSettingsDialog", TM._("Kill limit")).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
			.SetOnResultCallback(OnDialogResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("KillLimit", new SliderData
		{
			sliderValue = (int)wo.Data["killLimit"]
		});
		return dictionary;
	}
}
