using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GlobalSoundEmitterSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
{
	private class Keys
	{
		public enum Key
		{
			volume,
			pitch
		}

		private readonly string[] keys = new string[2] { "volume", "pitch" };

		public string this[Key key] => keys[(int)key];
	}

	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider volumeSlider;

	[SerializeField]
	private SettingsSlider pitchSlider;

	private Keys keys = new Keys();

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.GlobalSoundEmitter);
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		volumeSlider.Initialize(keys[Keys.Key.volume], (float)data[keys[Keys.Key.volume]], 0f, 1f);
		pitchSlider.Initialize(keys[Keys.Key.pitch], (float)data[keys[Keys.Key.pitch]], 0.5f, 2f);
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, value);
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
