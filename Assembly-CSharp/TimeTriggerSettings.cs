using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TimeTriggerSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
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
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.TimeTrigger);
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		float value = Convert.ToSingle(data["duration"]);
		float value2 = Convert.ToSingle(data["time"]);
		durationSlider.Initialize("duration", value, 0.1f, 1000f);
		durationInputField.Initialize("duration", value);
		delaySlider.Initialize("time", value2, 0f, 1000f);
		delayInputField.Initialize("time", value2);
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, value);
	}
}
