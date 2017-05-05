using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OculusSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider rangeSlider;

	[SerializeField]
	private SettingsSlider aggresionSlider;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.Oculus);
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("Radius", 17);
			dictionary.Add("Speed", 20);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		rangeSlider.Initialize("Radius", Convert.ToSingle(dictionary2["Radius"]), 5f, 40f);
		aggresionSlider.Initialize("Speed", Convert.ToSingle(dictionary2["Speed"]), 10f, 50f);
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, Convert.ToSingle(value));
	}
}
