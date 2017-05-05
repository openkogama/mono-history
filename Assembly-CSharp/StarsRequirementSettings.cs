using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StarsRequirementSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsSlider slider;

	[SerializeField]
	private SettingsInputFieldSlider inputField;

	[SerializeField]
	private GameObject message;

	private AllCollectiblesCollectedClient collectible;

	private int maxValue;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, TM._("Star Requirement"));
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("starAmount", 0);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		if (!dictionary2.ContainsKey("starAmount"))
		{
			dictionary2["starAmount"] = 0;
		}
		int num = Convert.ToInt32(dictionary2["starAmount"]);
		UpdateMaxValue();
		UpdateMessage(num);
		slider.Initialize("starAmount", num, 0, maxValue);
		inputField.Initialize("starAmount", num);
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, Convert.ToInt32(value));
	}

	private void UpdateMaxValue()
	{
		collectible = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>();
		if (collectible != null)
		{
			maxValue = collectible.Limit - 1;
		}
	}

	private void UpdateMessage(int starAmount)
	{
		if (maxValue < 1)
		{
			if (starAmount > 0)
			{
				OnSettingChanged("starAmount", 0);
			}
			message.gameObject.SetActive(value: true);
		}
		else
		{
			message.gameObject.SetActive(value: false);
		}
	}
}
