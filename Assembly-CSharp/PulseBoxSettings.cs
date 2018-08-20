using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PulseBoxSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider enabledSlider;

	[SerializeField]
	private SettingsInputFieldSlider enabledInputField;

	[SerializeField]
	private SettingsSlider disabledSlider;

	[SerializeField]
	private SettingsInputFieldSlider disabledInputField;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.PulseBox);
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("intervalOn", 0.5f);
			dictionary.Add("intervalOff", 0.5f);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		float value = Convert.ToSingle(dictionary2["intervalOn"]);
		float value2 = Convert.ToSingle(dictionary2["intervalOff"]);
		enabledSlider.Initialize("intervalOn", value, 0.1f, 1000f);
		enabledInputField.Initialize("intervalOn", value);
		disabledSlider.Initialize("intervalOff", value2, 0.1f, 1000f);
		disabledInputField.Initialize("intervalOff", value2);
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, value);
	}
}
