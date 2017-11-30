using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SmokeSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	private float[] color = new float[4];

	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider colorR;

	[SerializeField]
	private SettingsSlider colorG;

	[SerializeField]
	private SettingsSlider colorB;

	[SerializeField]
	private SettingsSlider alpha;

	[SerializeField]
	private SettingsSlider wind;

	[SerializeField]
	private SettingsSlider range;

	[SerializeField]
	private Image preview;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.Smoke);
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("color", new float[4] { 0.5f, 0.5f, 0.5f, 0.5f });
			dictionary.Add("length", 10f);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		if (!dictionary2.ContainsKey("color"))
		{
			float[] value = new float[4] { 0.5f, 0.5f, 0.5f, 0.25f };
			dictionary2.Add("color", value);
		}
		if (!dictionary2.ContainsKey("length"))
		{
			dictionary2.Add("length", 6f);
		}
		if (!dictionary2.ContainsKey("wind"))
		{
			dictionary2.Add("wind", 0.5f);
		}
		color = (float[])dictionary2["color"];
		colorR.Initialize("colorR", color[0], 0f, 1f);
		colorG.Initialize("colorG", color[1], 0f, 1f);
		colorB.Initialize("colorB", color[2], 0f, 1f);
		alpha.Initialize("alpha", color[3], 0.1f, 1f);
		range.Initialize("length", (float)dictionary2["length"], 1f, 12f);
		wind.Initialize("wind", (float)dictionary2["wind"], 0f, 1f);
		preview.color = new Color(color[0], color[1], color[2], color[3]);
	}

	public void OnSettingChanged(string key, object value)
	{
		switch (key)
		{
		case "colorR":
			color[0] = (float)value;
			settingsBase.OnSettingChanged("color", color);
			preview.color = new Color(color[0], color[1], color[2], color[3]);
			break;
		case "colorG":
			color[1] = (float)value;
			settingsBase.OnSettingChanged("color", color);
			preview.color = new Color(color[0], color[1], color[2], color[3]);
			break;
		case "colorB":
			color[2] = (float)value;
			settingsBase.OnSettingChanged("color", color);
			preview.color = new Color(color[0], color[1], color[2], color[3]);
			break;
		case "alpha":
			color[3] = (float)value;
			settingsBase.OnSettingChanged("color", color);
			preview.color = new Color(color[0], color[1], color[2], color[3]);
			break;
		default:
			settingsBase.OnSettingChanged(key, value);
			Debug.Log("Setting changed ");
			break;
		}
	}
}
