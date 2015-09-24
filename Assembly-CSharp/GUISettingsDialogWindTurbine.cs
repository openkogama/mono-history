using System.Collections.Generic;

public class GUISettingsDialogWindTurbine : MVGUIDynamicSettingsDialog
{
	public GUISettingsDialogWindTurbine()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/WindTurbineSettingsDialog", TM._("Wind Turbine Settings")).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
			.SetOnResultCallback(OnDialogResult)
			.SetOnIntermediateResultCallback(OnIntermediateResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("PitchSlider", new SliderData
		{
			sliderValue = (float)wo.Data["windPitch"]
		});
		dictionary.Add("SizeSlider", new SliderData
		{
			sliderValue = (float)wo.Data["windSize"]
		});
		return dictionary;
	}
}
