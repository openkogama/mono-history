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
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		if (!data.ContainsKey("levelAmount"))
		{
			data["levelAmount"] = 0;
		}
		int value = Convert.ToInt32(data["levelAmount"]);
		slider.Initialize("levelAmount", value, 0, MVGameControllerBase.Game.LocalPlayer.Level);
		inputField.Initialize("levelAmount", value);
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, Convert.ToInt32(value));
	}
}
