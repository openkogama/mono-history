using System.Collections.Generic;
using Localize;

public class MVGUISettingsDialogAdvancedGhost : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogAdvancedGhost()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/AdvancedGhostSettingsDialog", TextSlotIndex.Light).AddPositiveButton(TextSlotIndex.Ok).AddNegativeButton(TextSlotIndex.Cancel)
			.SetOnResultCallback(OnDialogResult)
			.SetOnIntermediateResultCallback(OnIntermediateResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("Radius", new SliderData
		{
			sliderValue = (float)wo.Data["Radius"]
		});
		dictionary.Add("Speed", new SliderData
		{
			sliderValue = (float)wo.Data["Speed"]
		});
		return dictionary;
	}
}
