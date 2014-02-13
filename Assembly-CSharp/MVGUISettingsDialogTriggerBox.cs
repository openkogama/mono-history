using System.Collections;
using System.Collections.Generic;
using Localize;

public class MVGUISettingsDialogTriggerBox : MVGUISettingsDialog
{
	public MVGUISettingsDialogTriggerBox()
	{
		dialogFactory.CreateDialog(TextSlotIndex.Once, TextSlotIndex.TriggerBox, UXDialogType.Toggle).AddPositiveButton(TextSlotIndex.Confirm).AddNegativeButton(TextSlotIndex.Cancel)
			.SetOnResultCallback(OnDialogResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private void OnDialogResult(UXDialogBox dialog)
	{
		if (dialog.DialogResult == UXDialogResult.Positive)
		{
			bool flag = (bool)dialog.GetResult();
			Hashtable hashtable = (Hashtable)wo.Data.Clone();
			hashtable["once"] = flag;
			MVGameController.Instance.Game.UpdateWorldObjectDataPartial(wo.Id, hashtable);
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
