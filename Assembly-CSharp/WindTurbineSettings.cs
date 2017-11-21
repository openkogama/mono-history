using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindTurbineSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider pitchSlider;

	[SerializeField]
	private SettingsInputFieldSlider pitchInputField;

	[SerializeField]
	private SettingsSlider powerSlider;

	[SerializeField]
	private SettingsInputFieldSlider powerInputField;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.WindTurbine);
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("windPitch", 0);
			dictionary.Add("windSize", 10);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		pitchSlider.Initialize("windPitch", Convert.ToInt32(dictionary2["windPitch"]), 0, 180);
		pitchInputField.Initialize("windPitch", Convert.ToInt32(dictionary2["windPitch"]));
		powerSlider.Initialize("windSize", Convert.ToInt32(dictionary2["windSize"]), 1, 20);
		powerInputField.Initialize("windSize", Convert.ToInt32(dictionary2["windSize"]));
	}

	public void OnSettingChanged(string key, object value)
	{
		Debug.Log("Setting changed " + key + " " + value);
		settingsBase.OnSettingChanged(key, Convert.ToSingle(value));
	}
}
