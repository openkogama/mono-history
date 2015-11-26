using System.Collections.Generic;

public class GUISettingsDialogShootableButton : MVGUIDynamicSettingsDialog
{
	public GUISettingsDialogShootableButton()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/ShootableButtonSettingsDialog", TM._("Shootable Button Settings")).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
			.SetOnResultCallback(OnDialogResult)
			.SetOnIntermediateResultCallback(OnIntermediateResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("DurationSlider", new SliderData
		{
			sliderValue = (float)wo.Data["duration"]
		});
		return dictionary;
	}
}
