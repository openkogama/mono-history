using System.Collections.Generic;

public class MVGUISettingsDialogGravityCube : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogGravityCube()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/GravityCubeSettingsDialog", TM._("Gravity Settings")).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
			.SetOnResultCallback(OnDialogResult)
			.SetOnIntermediateResultCallback(OnIntermediateResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("GravitySlider", new SliderData
		{
			sliderValue = (float)wo.Data["gravity"]
		});
		return dictionary;
	}
}
