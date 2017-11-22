using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectEnablerSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsToggle toggle;

	public void Initialize(int woID, GameObject root)
	{
		settingsBase.Initialize(woID, root, MVWorldObjectDocumentationType.ModelToggle);
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("showOutline", false);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = MVGameControllerBase.WOCM.GetWorldObjectClient(woID).Data;
		}
		if (!dictionary2.ContainsKey("showOutline"))
		{
			dictionary2["showOutline"] = true;
		}
		toggle.Initialize("showOutline", (bool)dictionary2["showOutline"]);
	}

	public void OnSettingChanged(string key, object value)
	{
		settingsBase.OnSettingChanged(key, value);
	}
}
