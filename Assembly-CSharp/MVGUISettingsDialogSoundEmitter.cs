using System;
using System.Collections.Generic;
using MV.Common;

public class MVGUISettingsDialogSoundEmitter : MVGUIDynamicSettingsDialog
{
	public MVGUISettingsDialogSoundEmitter()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Box Settings Dialogs/SoundEmitterSettingsDialog", TM._("Sound Emitter"), noButtons: true).SetOnResultCallback(OnDialogResult).SetOnIntermediateResultCallback(OnIntermediateResult)
			.SetValues(BuildDialogData())
			.Show();
		(dialogFactory.CurrentDialogBox as MVGUISoundEmitterSettingsBox).SetCurrentSound(wo.Data);
	}

	private Dictionary<string, DialogData> BuildDialogData()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		List<string> list = new List<string>(Enum.GetNames(typeof(AmbientAudioCategory)));
		list.Remove(AmbientAudioCategory.Undefined.ToString());
		dictionary.Add("CategoryComboBox", new TextComboBoxData
		{
			items = list.ToArray()
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
