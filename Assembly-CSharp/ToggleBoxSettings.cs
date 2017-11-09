using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleBoxSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsToggle toggle;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.ToggleBox);
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("once", false);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		toggle.Initialize("once", (bool)dictionary2["once"]);
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, value);
	}
}
