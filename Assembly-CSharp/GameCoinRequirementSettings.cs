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
		settingsBase.Initialize(woID, root);
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("gameCoinAmount", 0);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		if (!dictionary2.ContainsKey("gameCoinAmount"))
		{
			dictionary2["gameCoinAmount"] = 0;
		}
		int value = Convert.ToInt32(dictionary2["gameCoinAmount"]);
		slider.Initialize("gameCoinAmount", value, 0, 10000);
		inputField.Initialize("gameCoinAmount", value);
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, Convert.ToInt32(value));
	}
}
