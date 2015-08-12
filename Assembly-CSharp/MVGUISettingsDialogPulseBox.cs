using System.Collections.Generic;

public class MVGUISettingsDialogPulseBox : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogPulseBox()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/PulseBoxSettingsDialog", TM._("Pulse Box")).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
			.SetOnResultCallback(OnDialogResult)
			.SetOnIntermediateResultCallback(OnIntermediateResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("EnabledTimeSlider", new SliderData
		{
			sliderValue = (float)wo.Data["intervalOn"]
		});
		dictionary.Add("DisabledTimeSlider", new SliderData
		{
			sliderValue = (float)wo.Data["intervalOff"]
		});
		return dictionary;
	}
}
