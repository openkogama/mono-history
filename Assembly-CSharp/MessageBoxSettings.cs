using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MessageBoxSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider sizeSlider;

	[SerializeField]
	private Text sizeLabel;

	[SerializeField]
	private SettingsInputField inputField;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root);
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("text", "test");
			dictionary.Add("textSize", 0.2f);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
			if (!dictionary2.ContainsKey("textSize"))
			{
				dictionary2["textSize"] = 0.2f;
			}
		}
		sizeSlider.Initialize("textSize", (float)dictionary2["textSize"], 0.1f, 0.4f);
		SetTextSize((float)dictionary2["textSize"]);
		inputField.Initialize("text", (string)dictionary2["text"]);
	}

	public void OnSettingChanged(string key, object value)
	{
		if (key == "textSize")
		{
			float num = (float)Math.Round(Convert.ToDecimal(value), 1);
			SetTextSize(num);
			settingsBase.OnSettingChanged(key, num);
		}
		else
		{
			settingsBase.OnSettingChanged(key, value);
		}
	}

	private void SetTextSize(float value)
	{
		if (value == 0.1f)
		{
			sizeLabel.text = TM._("Small");
		}
		else if (value == 0.2f)
		{
			sizeLabel.text = TM._("Medium");
		}
		else if (value == 0.3f)
		{
			sizeLabel.text = TM._("Large");
		}
		else if (value == 0.4f)
		{
			sizeLabel.text = TM._("Huge");
		}
	}
}
