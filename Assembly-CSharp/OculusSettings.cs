using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OculusSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
{
	public static class Strings
	{
		public const string Radius = "Radius";

		public const string Speed = "Speed";

		public const string Lives = "Lives";
	}

	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider rangeSlider;

	[SerializeField]
	private SettingsSlider aggresionSlider;

	[SerializeField]
	private SettingsSlider numOfLivesSlider;

	[SerializeField]
	private SettingsInputFieldSlider numOfLivesInputSlider;

	private const int maxLives = 100;

	private const string infinity = "∞";

	private MVWorldObjectClient target;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.Oculus);
		target = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
		Dictionary<object, object> data = target.Data;
		rangeSlider.Initialize("Radius", Convert.ToSingle(data["Radius"]), 5f, 40f);
		aggresionSlider.Initialize("Speed", Convert.ToSingle(data["Speed"]), 10f, 50f);
		int num = 100;
		if (data.ContainsKey("Lives"))
		{
			num = Convert.ToInt32(data["Lives"]);
		}
		numOfLivesSlider.Initialize("Lives", num, 1, 100);
		numOfLivesInputSlider.Initialize("Lives", num);
		if (num == 100)
		{
			numOfLivesInputSlider.SetText("∞");
		}
	}

	public void OnSettingChanged(string key, object value)
	{
		if (key != null && key == "Lives")
		{
			int num = Convert.ToInt32(value);
			if (num == 100)
			{
				numOfLivesInputSlider.SetText("∞");
				Dictionary<object, object> dictionary = new Dictionary<object, object>();
				dictionary.Add("Lives", num);
				MVGameControllerBase.OperationRequests.RemoveWorldObjectDataPartial(target.Id, dictionary);
				settingsBase.RemoveData(key);
			}
			else
			{
				settingsBase.OnSettingChanged(key, num);
			}
		}
		else
		{
			settingsBase.OnSettingChanged(key, Convert.ToSingle(value));
		}
	}
}
