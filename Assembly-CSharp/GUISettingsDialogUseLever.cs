using System.Collections.Generic;

public class GUISettingsDialogUseLever : MVGUIDynamicSettingsDialog
{
	public GUISettingsDialogUseLever()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/UseLeverSettingsDialog", TM._("Use Lever Settings")).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
			.SetOnResultCallback(OnDialogResult)
			.SetOnIntermediateResultCallback(OnIntermediateResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("BeginActivatedButton", new ToggleIconData
		{
			toggleValue = (bool)wo.Data["beginActivated"]
		});
		return dictionary;
	}
}
