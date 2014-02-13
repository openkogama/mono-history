using System.Collections;
using System.Collections.Generic;
using Localize;

public class MVGUIObjectEnablerSettingsBox : MVGUISettingsDialog
{
	public MVGUIObjectEnablerSettingsBox()
	{
		dialogFactory.CreateDialog(TextSlotIndex.ShowOutline, TextSlotIndex.ObjectEnabler, UXDialogType.Toggle).AddPositiveButton(TextSlotIndex.Confirm).AddNegativeButton(TextSlotIndex.Reject)
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
			hashtable["showOutline"] = flag;
			MVGameController.Instance.Game.UpdateWorldObjectDataPartial(wo.Id, hashtable);
		}
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("Toggle", new ToggleIconData
		{
			toggleValue = (wo as MVObjectEnabler).ShowingOutline
		});
		return dictionary;
	}
}
