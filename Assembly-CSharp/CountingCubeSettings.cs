using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CountingCubeSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider slider;

	[SerializeField]
	private SettingsInputFieldSlider inputField;

	[SerializeField]
	private SettingsToggle toggle;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root);
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("startingValue", 5);
			dictionary.Add("reset", true);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		slider.Initialize("startingValue", (int)dictionary2["startingValue"], 1, 99);
		inputField.Initialize("startingValue", (int)dictionary2["startingValue"]);
		toggle.Initialize("reset", (bool)dictionary2["reset"]);
	}

	public void OnSettingChanged(string key, object value)
	{
		if (key == "startingValue")
		{
			settingsBase.OnSettingChanged(key, Convert.ToInt32(value));
		}
		else
		{
			settingsBase.OnSettingChanged(key, value);
		}
	}
}
