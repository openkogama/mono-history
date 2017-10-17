using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OculusSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	public static class Strings
	{
		public const string Radius = "Radius";

		public const string Speed = "Speed";

		public const string Lives = "Lives";
	}

	private const int maxLives = 100;

	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider rangeSlider;

	[SerializeField]
	private SettingsSlider aggresionSlider;

	[SerializeField]
	private SettingsSlider numOfLivesSlider;

	[SerializeField]
	private Text numOfLivesText;

	private MVWorldObjectClient target;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.Oculus);
		target = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
		Dictionary<object, object> data = target.Data;
		rangeSlider.Initialize("Radius", Convert.ToSingle(data["Radius"]), 5f, 40f);
		aggresionSlider.Initialize("Speed", Convert.ToSingle(data["Speed"]), 10f, 50f);
		int value = 100;
		if (data.ContainsKey("Lives"))
		{
			value = Convert.ToInt32(data["Lives"]);
		}
		numOfLivesSlider.Initialize("Lives", value, 1, 100);
	}

	public void OnSettingChanged(string key, object value)
	{
		switch (key)
		{
		case "Lives":
		{
			int num = Convert.ToInt32(value);
			UpdateSettingsGUI_Lives(num);
			if (num == 100)
			{
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add("Lives", num);
				MVGameControllerBase.OperationRequests.RemoveWorldObjectDataPartial(target.Id, dictionary);
				settingsBase.RemoveData(key);
			}
			else
			{
				settingsBase.OnSettingChanged(key, num);
			}
			break;
		}
		default:
			settingsBase.OnSettingChanged(key, Convert.ToSingle(value));
			break;
		}
	}

	public void UpdateSettingsGUI_Lives(int value)
	{
		if (value == 100)
		{
			numOfLivesText.text = "∞";
		}
		else
		{
			numOfLivesText.text = value.ToString();
		}
	}
}
