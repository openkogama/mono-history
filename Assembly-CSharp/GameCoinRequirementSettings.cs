using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameCoinRequirementSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider slider;

	[SerializeField]
	private SettingsInputFieldSlider inputField;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, TM._("Game Coin Requirement"));
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		if (!data.ContainsKey("gameCoinAmount"))
		{
			data["gameCoinAmount"] = 0;
		}
		int value = Convert.ToInt32(data["gameCoinAmount"]);
		slider.Initialize("gameCoinAmount", value, 0, 10000);
		inputField.Initialize("gameCoinAmount", value);
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, Convert.ToInt32(value));
	}
}
