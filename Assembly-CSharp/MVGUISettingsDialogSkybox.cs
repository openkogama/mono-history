using System.Collections.Generic;
using Localize;

public class MVGUISettingsDialogSkybox : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogSkybox()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/SkyboxSettingsDialog", TextSlotIndex.Skybox).AddPositiveButton(TextSlotIndex.Ok).AddNegativeButton(TextSlotIndex.Cancel)
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
		dictionary.Add("SunAngleSlider", new SliderData
		{
			sliderValue = (float)wo.Data["sunAngle"]
		});
		dictionary.Add("FogDensitySlider", new SliderData
		{
			sliderValue = (float)wo.Data["fogDensity"]
		});
		return dictionary;
	}
}
