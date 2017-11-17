using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoundEmitterSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
{
	private class Keys
	{
		public enum Key
		{
			volume,
			pitch,
			range
		}

		private readonly string[] keys = new string[3] { "volume", "pitch", "range" };

		public string this[Key key] => keys[(int)key];
	}

	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider volumeSlider;

	[SerializeField]
	private SettingsSlider pitchSlider;

	[SerializeField]
	private SettingsSlider rangeSlider;

	private Keys keys = new Keys();

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.SoundEmitter);
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		volumeSlider.Initialize(keys[Keys.Key.volume], (float)data[keys[Keys.Key.volume]], 0f, 1f);
		pitchSlider.Initialize(keys[Keys.Key.pitch], (float)data[keys[Keys.Key.pitch]], 0.5f, 2f);
		rangeSlider.Initialize(keys[Keys.Key.range], (int)data[keys[Keys.Key.range]], 0, 2);
	}

	public void OnSettingChanged(string key, object value)
	{
		if (key != keys[Keys.Key.range])
		{
			settingsBase.OnSettingChanged(key, (float)Convert.ToDecimal(value));
		}
		else
		{
			settingsBase.OnSettingChanged(key, Convert.ToInt32(value));
		}
	}

	public void OnSettingChanged(string key, int value)
	{
		settingsBase.OnSettingChanged(key, value);
	}

	public void OnSettingChanged(string key, float value)
	{
		settingsBase.OnSettingChanged(key, value);
	}
}
