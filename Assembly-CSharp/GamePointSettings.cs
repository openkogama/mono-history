using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePointSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider gamePointsAwardedSlider;

	[SerializeField]
	private SettingsInputFieldSlider gamePointsAwardedInputField;

	private int woID;

	private const string gamePointAwardedString = "gamePointAmount";

	public void Initialize(int woID, GameObject root)
	{
		this.woID = woID;
		settingsBase.Initialize(woID, root, "Crystal Reward");
		Dictionary<object, object> data = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		int num = 0;
		if (!data.ContainsKey("gamePointAmount"))
		{
			data.Add("gamePointAmount", 0);
		}
		num = (int)data["gamePointAmount"];
		gamePointsAwardedSlider.Initialize("gamePointAmount", num, 0, 1000);
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

	private void UpdateData()
	{
		if (Mathf.FloorToInt(gamePointsAwardedSlider.Value) == 0)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("gamePointAmount", 0);
			MVGameControllerBase.OperationRequests.RemoveWorldObjectDataPartial(woID, dictionary);
			settingsBase.RemoveData("gamePointAmount");
		}
	}

	private void OnDestroy()
	{
		UpdateData();
	}
}
