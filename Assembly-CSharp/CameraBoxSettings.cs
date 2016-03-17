using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraBoxSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
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
			dictionary.Add("distanceToAvatar", 5);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		if (MVGameControllerBase.Game.GameType == MVGameType.Classic)
		{
			slider.Initialize("distanceToAvatar", Convert.ToSingle(dictionary2["distanceToAvatar"]), 3f, 10f);
		}
		else if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			slider.Initialize("distanceToAvatar", Convert.ToSingle(dictionary2["distanceToAvatar"]), 15f, 45f);
		}
		inputField.Initialize("distanceToAvatar", Convert.ToSingle(dictionary2["distanceToAvatar"]));
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, Convert.ToSingle(value));
	}
}
