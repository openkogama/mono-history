using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerCubeSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider ScaleX;

	[SerializeField]
	private SettingsInputFieldSlider ScaleXInput;

	[SerializeField]
	private SettingsSlider ScaleY;

	[SerializeField]
	private SettingsInputFieldSlider ScaleYInput;

	[SerializeField]
	private SettingsSlider ScaleZ;

	[SerializeField]
	private SettingsInputFieldSlider ScaleZInput;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.TriggerCube);
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		ScaleX.Initialize("scaleX", (float)data["scaleX"], 0.5f, 20f);
		ScaleXInput.Initialize("scaleX", (float)data["scaleX"]);
		ScaleY.Initialize("scaleY", (float)data["scaleY"], 0.5f, 20f);
		ScaleYInput.Initialize("scaleY", (float)data["scaleY"]);
		ScaleZ.Initialize("scaleZ", (float)data["scaleZ"], 0.5f, 20f);
		ScaleZInput.Initialize("scaleZ", (float)data["scaleZ"]);
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, value);
	}
}
