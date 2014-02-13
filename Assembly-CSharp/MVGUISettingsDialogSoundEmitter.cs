using System;
using System.Collections.Generic;
using Localize;
using MV.Common;

public class MVGUISettingsDialogSoundEmitter : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogSoundEmitter()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/SoundEmitterSettingsDialog", TextSlotIndex.SoundEmitterBox, noButtons: true).SetOnResultCallback(OnDialogResult).SetOnIntermediateResultCallback(OnIntermediateResult)
			.SetValues(BuildDialogData())
			.Show();
		(dialogFactory.CurrentDialogBox as MVGUISoundEmitterSettingsBox).SetCurrentSound(wo.Data);
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("CategoryComboBox", new TextComboBoxData
		{
			items = Enum.GetNames(typeof(AmbientAudioCategory))
		});
		dictionary.Add("PitchSlider", new SliderData
		{
			sliderValue = (float)wo.Data["pitch"]
		});
		dictionary.Add("VolumeSlider", new SliderData
		{
			sliderValue = (float)wo.Data["volume"]
		});
		dictionary.Add("RangeSlider", new SliderData
		{
			sliderValue = (int)wo.Data["range"]
		});
		dictionary.Add("MuteToggle", new ToggleIconData
		{
			toggleValue = (bool)wo.Data["mute"]
		});
		return dictionary;
	}
}
