using System.Collections.Generic;
using Localize;

public class MVGUISettingsDialogPulseBox : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogPulseBox()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/PulseBoxSettingsDialog", TextSlotIndex.PulseBox).AddPositiveButton(TextSlotIndex.Ok).AddNegativeButton(TextSlotIndex.Cancel)
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
