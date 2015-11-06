using System.Collections.Generic;

public class MVGUIPressurePlateSettingsBox : MVGUISettingsDialog
{
	public MVGUIPressurePlateSettingsBox()
	{
		dialogFactory.CreateDialog(TM._("Hide"), TM._("Pressure Plate"), UXDialogType.Toggle).AddPositiveButton(TM._("Yes")).AddNegativeButton(TM._("Cancel"))
			.SetOnResultCallback(OnDialogResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private void OnDialogResult(UXDialogBox dialog)
	{
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			bool flag = (bool)dialog.GetResult();
			Dictionary<object, object> dictionary = new Dictionary<object, object>(wo.Data);
			if (!dictionary.ContainsKey("hide"))
			{
				dictionary.Add("hide", false);
			}
			dictionary["hide"] = flag;
			MVGameControllerBase.Game.UpdateWorldObjectDataPartial(wo.Id, dictionary);
		}
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		bool toggleValue = false;
		if (wo.Data.ContainsKey("hide"))
		{
			toggleValue = (bool)wo.Data["hide"];
		}
		dictionary.Add("Toggle", new ToggleIconData
		{
			toggleValue = toggleValue
		});
		return dictionary;
	}
}
