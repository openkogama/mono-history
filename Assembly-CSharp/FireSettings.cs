using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FireSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
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
	private SettingsSlider intensity;

	[SerializeField]
	private Image preview;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.Fire);
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		if (!data.ContainsKey("I"))
		{
			data["I"] = 5f;
		}
		if (!data.ContainsKey("C"))
		{
			float[] value = new float[3] { 1f, 0.7843137f, 0.509804f };
			data["C"] = value;
		}
		color = (float[])data["C"];
		colorR.Initialize("R", color[0], 0.3f, 1f);
		colorG.Initialize("G", color[1], 0.3f, 1f);
		colorB.Initialize("B", color[2], 0.3f, 1f);
		intensity.Initialize("I", (float)data["I"], 1f, 20f);
		preview.color = new Color(color[0], color[1], color[2]);
	}

	public void OnSettingChanged(string key, object value)
	{
		switch (key)
		{
		case "R":
			color[0] = (float)value;
			settingsBase.OnSettingChanged("C", color);
			preview.color = new Color(color[0], color[1], color[2]);
			break;
		case "G":
			color[1] = (float)value;
			settingsBase.OnSettingChanged("C", color);
			preview.color = new Color(color[0], color[1], color[2]);
			break;
		case "B":
			color[2] = (float)value;
			settingsBase.OnSettingChanged("C", color);
			preview.color = new Color(color[0], color[1], color[2]);
			break;
		default:
			settingsBase.OnSettingChanged(key, value);
			break;
		}
	}
}
