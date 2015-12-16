using System.Collections.Generic;

public class MVGUISettingsDialogCountingCube : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogCountingCube()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/CountingCubeSettingsDialog", TM._("Counting Cube Settings")).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
			.SetOnResultCallback(OnDialogResult)
			.SetOnIntermediateResultCallback(OnIntermediateResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("TriggerValueSlider", new SliderData
		{
			sliderValue = (int)wo.Data["startingValue"]
		});
		dictionary.Add("Toggle", new ToggleIconData
		{
			toggleValue = (bool)wo.Data["reset"]
		});
		return dictionary;
	}
}
