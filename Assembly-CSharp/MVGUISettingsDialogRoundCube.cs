using System;
using System.Collections.Generic;
using Localize;
using MV.Common;

public class MVGUISettingsDialogRoundCube : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogRoundCube()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/RoundCubeSettingsDialog", TextSlotIndex.RoundCube).AddPositiveButton(TextSlotIndex.Ok).AddNegativeButton(TextSlotIndex.Cancel)
			.SetOnResultCallback(OnDialogResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("intervalSlider", new SliderData
		{
			sliderValue = (int)wo.Data["interval"]
		});
		string[] array = new string[Enum.GetNames(typeof(MVWinningCondition)).Length];
		int num = 0;
		string[] names = Enum.GetNames(typeof(MVWinningCondition));
		foreach (string key_as_text in names)
		{
			array[num++] = Localization.Instance.GetText(key_as_text);
		}
		dictionary.Add("winningConditionComboBox", new TextComboBoxData
		{
			items = array,
			currentlySelectedIndex = (int)wo.Data["winningCondition"]
		});
		return dictionary;
	}
}
