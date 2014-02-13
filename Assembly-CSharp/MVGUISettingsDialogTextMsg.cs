using System.Collections.Generic;
using Localize;

public class MVGUISettingsDialogTextMsg : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogTextMsg()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/TextMsgSettingsDialog", TextSlotIndex.TextMessage).AddPositiveButton(TextSlotIndex.Ok).AddNegativeButton(TextSlotIndex.Cancel)
			.SetOnResultCallback(OnDialogResult)
			.SetOnIntermediateResultCallback(OnIntermediateResult)
			.SetValues(BuildDialogData())
			.Show();
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("TextField", new TextFieldData
		{
			text = (string)wo.Data["text"],
			allowedInput = TextInputType.All
		});
		if (!wo.Data.ContainsKey("textSize"))
		{
			wo.Data.Add("textSize", 0.2f);
		}
		dictionary.Add("TextSizeSlider", new SliderData
		{
			sliderValue = (float)wo.Data["textSize"]
		});
		return dictionary;
	}
}
