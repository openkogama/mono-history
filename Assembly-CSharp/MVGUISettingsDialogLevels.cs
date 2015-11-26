using System.Collections.Generic;

public class MVGUISettingsDialogLevels : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogLevels()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/LevelSettingsDialog", TM._("Levels")).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
			.SetOnResultCallback(OnDialogResult)
			.SetOnIntermediateResultCallback(OnIntermediateResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		int num = 0;
		if (wo.Data.ContainsKey("levelAmount"))
		{
			num = (int)wo.Data["levelAmount"];
		}
		dictionary.Add("LevelSlider", new SliderData
		{
			sliderValue = num
		});
		return dictionary;
	}
}
