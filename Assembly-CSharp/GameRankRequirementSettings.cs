using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameRankRequirementSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider slider;

	[SerializeField]
	private SettingsInputFieldSlider gameTierRequirementInputField;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, TM._("Game Tier Requirement"));
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("RequiredRank", 0);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		int num = 0;
		if (!dictionary2.ContainsKey("RequiredRank"))
		{
			dictionary2.Add("RequiredRank", 0);
		}
		num = (int)dictionary2["RequiredRank"];
		slider.Initialize("RequiredRank", num, 0, 3);
		gameTierRequirementInputField.Initialize("RequiredRank", num);
	}

	public void OnSettingChanged(string key, object value)
	{
		int num = Mathf.FloorToInt((float)value);
		gameTierRequirementInputField.SetText(num.ToString());
		settingsBase.OnSettingChanged(key, num);
	}
}
