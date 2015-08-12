using System.Collections.Generic;

public class MVGUISettingsDialogGameCoinChest : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogGameCoinChest()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/GameCoinChestSettingsDialog", TM._("Game Coin Chest")).AddPositiveButton(TM._("Ok")).AddNegativeButton(TM._("Cancel"))
			.SetOnResultCallback(OnDialogResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("GameCoinAmountSlider", new SliderData
		{
			sliderValue = (int)wo.Data["gameCoinAmount"]
		});
		return dictionary;
	}
}
