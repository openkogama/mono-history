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
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		if (MVGameControllerBase.Game.GameType == MVGameType.Classic)
		{
			slider.Initialize("distanceToAvatar", Convert.ToSingle(data["distanceToAvatar"]), 3f, 10f);
		}
		else if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			slider.Initialize("distanceToAvatar", Convert.ToSingle(data["distanceToAvatar"]), 15f, 45f);
		}
		inputField.Initialize("distanceToAvatar", Convert.ToSingle(data["distanceToAvatar"]));
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, Convert.ToSingle(value));
	}
}
