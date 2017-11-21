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
		settingsBase.Initialize(woID, root, TM._("Level Requirement"));
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		int value = 0;
		if (data.ContainsKey("levelAmount"))
		{
			value = (int)data["levelAmount"];
		}
		slider.Initialize("levelAmount", value, 0, MVGameControllerBase.Game.LocalPlayer.Level);
		inputField.Initialize("levelAmount", value);
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, Convert.ToInt32(value));
	}
}
