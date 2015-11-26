using System.Collections.Generic;

public class MVGUISettingsDialogGameCoins : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogGameCoins()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/GameCoinsDialog", TM._("GameCoins")).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
			.SetOnResultCallback(OnDialogResult)
			.SetOnIntermediateResultCallback(OnIntermediateResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		int num = 0;
		if (wo.Data.ContainsKey("gameCoinAmount"))
		{
			num = (int)wo.Data["gameCoinAmount"];
		}
		dictionary.Add("GameCoinsSlider", new SliderData
		{
			sliderValue = num
		});
		return dictionary;
	}
}
