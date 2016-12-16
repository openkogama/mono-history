using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoundCubeSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider slider;

	[SerializeField]
	private InputField minutes;

	[SerializeField]
	private InputField seconds;

	[SerializeField]
	private SettingsDropdown dropdown;

	private string[] options = new string[3]
	{
		TM._("None"),
		TM._("Reach Lowest Altitude"),
		TM._("Reach Highest Altitude")
	};

	private List<int> WOResultsParser = new List<int> { 0, 3, 2 };

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root);
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		int num = Convert.ToInt32(data["interval"]);
		int num2 = Convert.ToInt32(data["winningCondition"]);
		slider.Initialize("interval", num, 30, 3600);
		minutes.text = GetMinutes(num / 60).ToString();
		seconds.text = GetSeconds(num).ToString();
		if (!WOResultsParser.Contains(num2))
		{
			num2 = 0;
		}
		dropdown.Initialize("winningCondition", num2, options, WOResultsParser);
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, value);
	}

	public void SliderChanged()
	{
		int num = (int)slider.slider.value;
		minutes.text = GetMinutes((num / 60).ToString()).ToString();
		seconds.text = GetSeconds(num.ToString()).ToString();
		settingsBase.OnSettingChanged("interval", num);
	}

	public void OnInputFieldChanged()
	{
		int num = Mathf.Clamp(GetSeconds(seconds.text), 0, 60);
		int num2 = Mathf.Clamp(GetMinutes(minutes.text), 0, 60);
		if (num2 == 0)
		{
			num = Mathf.Clamp(num, 30, 60);
		}
		minutes.text = num2.ToString();
		seconds.text = num.ToString();
		slider.slider.value = num2 * 60 + num;
	}

	private int GetMinutes(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return 0;
		}
		int value2 = int.Parse(value);
		return GetMinutes(value2);
	}

	private int GetMinutes(int value)
	{
		TimeSpan timeSpan = TimeSpan.FromMinutes(value);
		return timeSpan.Hours * 60 + timeSpan.Minutes;
	}

	private int GetSeconds(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return 0;
		}
		int value2 = int.Parse(value);
		return GetSeconds(value2);
	}

	private int GetSeconds(int value)
	{
		return TimeSpan.FromSeconds(value).Seconds;
	}
}
