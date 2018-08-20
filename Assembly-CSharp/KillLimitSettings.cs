using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KillLimitSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider killLimitSlider;

	[SerializeField]
	private SettingsInputFieldSlider killLimitInputField;

	[SerializeField]
	private Text killLimitHeader;

	public void Initialize(int woID, GameObject root, string header)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.PlayerKillWinCondition);
		killLimitHeader.text = header;
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("killLimit", 5);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		killLimitSlider.Initialize("killLimit", (int)dictionary2["killLimit"], 1, 200);
		killLimitInputField.Initialize("killLimit", (int)dictionary2["killLimit"]);
	}

	public void OnSettingChanged(string key, object value)
	{
		int num = Convert.ToInt32(value);
		settingsBase.OnSettingChanged(key, num);
	}
}
