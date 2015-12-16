using System.Collections.Generic;
using UnityEngine;

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
		int num = 45;
		MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		if (localPlayer != null)
		{
			num = localPlayer.Level;
		}
		int num2 = 0;
		if (wo.Data.ContainsKey("levelAmount"))
		{
			num2 = (int)wo.Data["levelAmount"];
			num = Mathf.Max(num, num2);
		}
		dictionary.Add("LevelSlider", new SliderData
		{
			sliderValue = num2,
			maxValue = num,
			minValue = 0f,
			setMinMaxValue = true
		});
		return dictionary;
	}
}
