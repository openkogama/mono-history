using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelRequirementSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider slider;

	[SerializeField]
	private SettingsInputFieldSlider inputField;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root);
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("levelAmount", 0);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		if (!dictionary2.ContainsKey("levelAmount"))
		{
			dictionary2["levelAmount"] = 0;
		}
		int value = Convert.ToInt32(dictionary2["levelAmount"]);
		slider.Initialize("levelAmount", value, 0, MVGameControllerBase.Game.LocalPlayer.Level);
		inputField.Initialize("levelAmount", value);
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, Convert.ToInt32(value));
	}
}
