using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoundEmitterSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider volumeSlider;

	[SerializeField]
	private SettingsSlider pitchSlider;

	[SerializeField]
	private SettingsSlider rangeSlider;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root);
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("volume", 0.5f);
			dictionary.Add("pitch", 1f);
			dictionary.Add("range", 1);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		volumeSlider.Initialize("volume", (float)dictionary2["volume"], 0f, 1f);
		pitchSlider.Initialize("pitch", (float)dictionary2["pitch"], 0.5f, 2f);
		rangeSlider.Initialize("range", (int)dictionary2["range"], 0, 2);
	}

	public void OnSettingChanged(string key, object value)
	{
		if (key != "range")
		{
			float num = (float)Convert.ToDecimal(value);
			settingsBase.OnSettingChanged(key, num);
		}
		else
		{
			int num2 = Convert.ToInt32(value);
			settingsBase.OnSettingChanged(key, num2);
		}
	}
}
