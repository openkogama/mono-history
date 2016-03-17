using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkyboxSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	private float[] color = new float[3];

	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider colorR;

	[SerializeField]
	private SettingsSlider colorG;

	[SerializeField]
	private SettingsSlider colorB;

	[SerializeField]
	private SettingsSlider angle;

	[SerializeField]
	private SettingsSlider fog;

	[SerializeField]
	private Image preview;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root);
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("color", new float[3] { 0.5f, 0.5f, 0.5f });
			dictionary.Add("sunAngle", 10f);
			dictionary.Add("fogDensity", 10f);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		color = (float[])dictionary2["color"];
		colorR.Initialize("colorR", color[0], 0f, 1f);
		colorG.Initialize("colorG", color[1], 0f, 1f);
		colorB.Initialize("colorB", color[2], 0f, 1f);
		angle.Initialize("sunAngle", (float)dictionary2["sunAngle"], 0f, 360f);
		fog.Initialize("fogDensity", (float)dictionary2["fogDensity"], 0.005f, 0.05f);
		preview.color = new Color(color[0], color[1], color[2]);
	}

	public void OnSettingChanged(string key, object value)
	{
		switch (key)
		{
		case "colorR":
			color[0] = (float)value;
			settingsBase.OnSettingChanged("color", color);
			preview.color = new Color(color[0], color[1], color[2]);
			break;
		case "colorG":
			color[1] = (float)value;
			settingsBase.OnSettingChanged("color", color);
			preview.color = new Color(color[0], color[1], color[2]);
			break;
		case "colorB":
			color[2] = (float)value;
			settingsBase.OnSettingChanged("color", color);
			preview.color = new Color(color[0], color[1], color[2]);
			break;
		default:
			Debug.Log(value);
			settingsBase.OnSettingChanged(key, value);
			Debug.Log("Setting changed ");
			break;
		}
	}
}
