using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WaterBoxSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider colorR;

	[SerializeField]
	private SettingsSlider colorG;

	[SerializeField]
	private SettingsSlider colorB;

	[SerializeField]
	private Image preview;

	private float[] color = new float[3];

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root);
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("waterColor", new float[3] { 0.5f, 0.5f, 0.5f });
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		color = (float[])dictionary2["waterColor"];
		colorR.Initialize("colorR", color[0], 0f, 1f);
		colorG.Initialize("colorG", color[1], 0f, 1f);
		colorB.Initialize("colorB", color[2], 0f, 1f);
		preview.color = new Color(color[0], color[1], color[2]);
	}

	public void OnSettingChanged(string key, object value)
	{
		switch (key)
		{
		case "colorR":
			color[0] = (float)value;
			UpdateWaterColor();
			break;
		case "colorG":
			color[1] = (float)value;
			UpdateWaterColor();
			break;
		case "colorB":
			color[2] = (float)value;
			UpdateWaterColor();
			break;
		default:
			settingsBase.OnSettingChanged(key, value);
			break;
		}
	}

	private void UpdateWaterColor()
	{
		settingsBase.OnSettingChanged("waterColor", color);
		preview.color = new Color(color[0], color[1], color[2]);
	}
}
