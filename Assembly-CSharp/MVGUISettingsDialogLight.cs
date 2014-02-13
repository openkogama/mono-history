using System.Collections.Generic;
using Localize;

public class MVGUISettingsDialogLight : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogLight()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/LightSettingsDialog", TextSlotIndex.Light).AddPositiveButton(TextSlotIndex.Ok).AddNegativeButton(TextSlotIndex.Cancel)
			.SetOnResultCallback(OnDialogResult)
			.SetOnIntermediateResultCallback(OnIntermediateResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		float[] array = wo.Data["color"] as float[];
		dictionary.Add("RedSlider", new SliderData
		{
			sliderValue = array[0]
		});
		dictionary.Add("GreenSlider", new SliderData
		{
			sliderValue = array[1]
		});
		dictionary.Add("BlueSlider", new SliderData
		{
			sliderValue = array[2]
		});
		dictionary.Add("RangeSlider", new SliderData
		{
			sliderValue = (float)wo.Data["range"]
		});
		dictionary.Add("IntensitySlider", new SliderData
		{
			sliderValue = (float)wo.Data["intensity"]
		});
		return dictionary;
	}
}
