using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraBoxSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider slider;

	[SerializeField]
	private SettingsInputFieldSlider inputField;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.CameraSettings);
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		slider.Initialize("distanceToAvatar", Convert.ToSingle(data["distanceToAvatar"]), 3f, 10f);
		inputField.Initialize("distanceToAvatar", Convert.ToSingle(data["distanceToAvatar"]));
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, Convert.ToSingle(value));
	}
}
