using System.Collections.Generic;
using UnityEngine;

public class GUISettingsDialogStars : MVGUIDynamicSettingsDialog
{
	public GUISettingsDialogStars()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/StarTriggerSettingsDialog", TM._("Stars Required Settings")).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
			.SetOnResultCallback(OnDialogResult)
			.SetOnIntermediateResultCallback(OnIntermediateResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		int num = 0;
		if (wo.Data.ContainsKey("starAmount"))
		{
			num = (int)wo.Data["starAmount"];
		}
		AllCollectiblesCollectedClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>();
		int num2 = 0;
		if (singletonWinnerConditionByType != null)
		{
			num2 = singletonWinnerConditionByType.Limit;
		}
		if (num2 == 0)
		{
			num2 = 1;
			dictionary.Add("NoStarsText", new TextData
			{
				text = "There are no stars in play, add stars to level to increase slider limit."
			});
		}
		int num3 = num;
		dictionary.Add("StarsNeededSlider", new SliderData
		{
			sliderValue = num3,
			maxValue = Mathf.Max(num2, num3),
			minValue = 0f,
			setMinMaxValue = true
		});
		return dictionary;
	}
}
