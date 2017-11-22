using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShootablePlateSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider slider;

	[SerializeField]
	private SettingsInputFieldSlider inputField;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.ShootableButton);
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		float value = Convert.ToSingle(data["duration"]);
		slider.Initialize("duration", value, 0.5f, 30f);
		inputField.Initialize("duration", value);
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, Convert.ToSingle(value));
	}
}
