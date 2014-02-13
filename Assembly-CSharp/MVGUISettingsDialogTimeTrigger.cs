using System.Collections.Generic;
using Localize;

public class MVGUISettingsDialogTimeTrigger : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogTimeTrigger()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/TimeTriggerSettingsDialog", TextSlotIndex.TimeTrigger).AddPositiveButton(TextSlotIndex.Ok).AddNegativeButton(TextSlotIndex.Cancel)
			.SetOnResultCallback(OnDialogResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("DelayTimeSlider", new SliderData
		{
			sliderValue = (float)wo.Data["time"]
		});
		dictionary.Add("DurationTimeSlider", new SliderData
		{
			sliderValue = (float)wo.Data["duration"]
		});
		return dictionary;
	}
}
