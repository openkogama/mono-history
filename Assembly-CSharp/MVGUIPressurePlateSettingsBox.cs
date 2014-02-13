using System.Collections;
using System.Collections.Generic;
using Localize;

public class MVGUIPressurePlateSettingsBox : MVGUISettingsDialog
{
	public MVGUIPressurePlateSettingsBox()
	{
		dialogFactory.CreateDialog(TextSlotIndex.Hide, TextSlotIndex.PressurePlate, UXDialogType.Toggle).AddPositiveButton(TextSlotIndex.Confirm).AddNegativeButton(TextSlotIndex.Cancel)
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
			if (!hashtable.ContainsKey("hide"))
			{
				hashtable.Add("hide", false);
			}
			hashtable["hide"] = flag;
			MVGameController.Instance.Game.UpdateWorldObjectDataPartial(wo.Id, hashtable);
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
