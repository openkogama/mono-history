using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePointChestSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider gamePointsAwardedSlider;

	[SerializeField]
	private SettingsInputFieldSlider gamePointsAwardedInputField;

	private const string gamePointAwardedString = "gamePointAmount";

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, "Crystal Reward");
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		int num = 30;
		if (!data.ContainsKey("gamePointAmount"))
		{
			data.Add("gamePointAmount", num);
		}
		num = (int)data["gamePointAmount"];
		gamePointsAwardedSlider.Initialize("gamePointAmount", num, 2, 100);
		gamePointsAwardedInputField.Initialize("gamePointAmount", num);
	}

	public void OnSettingChanged(string key, object value)
	{
		if (key == "gamePointAmount")
		{
			int num = Mathf.FloorToInt((float)value);
			gamePointsAwardedInputField.SetText(num.ToString());
			settingsBase.OnSettingChanged("gamePointAmount", num);
		}
	}
}
