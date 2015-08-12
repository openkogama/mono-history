using System.Collections.Generic;

public class MVGUISettingsDialogTriggerBox : MVGUISettingsDialog
{
	public MVGUISettingsDialogTriggerBox()
	{
		dialogFactory.CreateDialog(TM._("Once"), TM._("Trigger Box"), UXDialogType.Toggle).AddPositiveButton(TM._("Yes")).AddNegativeButton(TM._("No"))
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
			dictionary["once"] = flag;
			MVGameController.Game.UpdateWorldObjectDataPartial(wo.Id, dictionary);
		}
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("Toggle", new ToggleIconData
		{
			toggleValue = (bool)wo.Data["once"]
		});
		return dictionary;
	}
}
