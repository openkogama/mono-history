using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointLightSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	private float[] color = new float[3];

	private int intType;

	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider colorR;

	[SerializeField]
	private SettingsSlider colorG;

	[SerializeField]
	private SettingsSlider colorB;

	[SerializeField]
	private SettingsSlider range;

	[SerializeField]
	private SettingsSlider intensity;

	[SerializeField]
	private SettingsSlider HaloTextures;

	[SerializeField]
	private SettingsToggle hide;

	[SerializeField]
	private Image preview;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.PointLight);
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		if (!data.ContainsKey("hide"))
		{
			data.Add("hide", true);
			settingsBase.OnSettingChanged("hide", true);
		}
		if (!data.ContainsKey("halo"))
		{
			data.Add("halo", 1);
			settingsBase.OnSettingChanged("halo", 1);
		}
		color = (float[])data["color"];
		colorR.Initialize("colorR", color[0], 0f, 1f);
		colorG.Initialize("colorG", color[1], 0f, 1f);
		colorB.Initialize("colorB", color[2], 0f, 1f);
		range.Initialize("range", (float)data["range"], 1f, 10f);
		intensity.Initialize("intensity", (float)data["intensity"], 1f, 20f);
		HaloTextures.Initialize("halo", (int)data["halo"], 1, 3);
		hide.Initialize("hide", (bool)data["hide"]);
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
		case "halo":
			intType = Convert.ToInt32(value);
			settingsBase.OnSettingChanged("halo", intType);
			break;
		default:
			settingsBase.OnSettingChanged(key, value);
			Debug.Log("Setting changed ");
			break;
		}
	}
}
