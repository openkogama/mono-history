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
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.Skybox);
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		color = (float[])data["color"];
		colorR.Initialize("colorR", color[0], 0f, 1f);
		colorG.Initialize("colorG", color[1], 0f, 1f);
		colorB.Initialize("colorB", color[2], 0f, 1f);
		angle.Initialize("sunAngle", (float)data["sunAngle"], 0f, 360f);
		fog.Initialize("fogDensity", (float)data["fogDensity"], 0.005f, 0.05f);
		preview.color = new Color(color[0], color[1], color[2]);
	}

	public void OnSettingChanged(string key, object value)
	{
		switch (key)
		{
		case "colorR":
			color[0] = (float)value;
			OnColorChange();
			break;
		case "colorG":
			color[1] = (float)value;
			OnColorChange();
			break;
		case "colorB":
			color[2] = (float)value;
			OnColorChange();
			break;
		case "sunAngle":
			settingsBase.OnSettingChanged(key, value);
			OnColorChange();
			break;
		case "fogDensity":
			settingsBase.OnSettingChanged(key, value);
			break;
		default:
			Debug.LogError("Unknown key: " + key);
			break;
		}
	}

	private void OnColorChange()
	{
		settingsBase.OnSettingChanged("color", color);
		preview.color = new Color(color[0], color[1], color[2]);
	}
}
