using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TimeTriggerSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider durationSlider;

	[SerializeField]
	private SettingsInputFieldSlider durationInputField;

	[SerializeField]
	private SettingsSlider delaySlider;

	[SerializeField]
	private SettingsInputFieldSlider delayInputField;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root);
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("duration", 0.5f);
			dictionary.Add("time", 0.5f);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		float value = Convert.ToSingle(dictionary2["duration"]);
		float value2 = Convert.ToSingle(dictionary2["time"]);
		durationSlider.Initialize("duration", value, 0f, 20f);
		durationInputField.Initialize("duration", value);
		delaySlider.Initialize("time", value2, 0f, 20f);
		delayInputField.Initialize("time", value2);
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, value);
	}
}
