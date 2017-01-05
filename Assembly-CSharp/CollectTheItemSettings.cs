using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollectTheItemSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsToggle toggle;

	private Dictionary<object, object> childMap;

	public void Initialize(int woID, GameObject root)
	{
		MVBlueprintBase mVBlueprintBase = (MVBlueprintBase)MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
		MVWorldObjectClient child = mVBlueprintBase.GetChild("CollectableInstance");
		int id = child.Id;
		Dictionary<object, object> dictionary2;
		if (woID == -1)
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			dictionary.Add("hasIndicator", false);
			dictionary2 = dictionary;
		}
		else
		{
			dictionary2 = (Dictionary<object, object>)child.Data["BlueprintData"];
			childMap = new Dictionary<object, object>((Dictionary<object, object>)dictionary2["ChildrenMap"]);
		}
		settingsBase.Initialize(id, root);
		toggle.Initialize("hasIndicator", (bool)dictionary2["hasIndicator"]);
	}

	public void OnSettingChanged(string key, object value)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add(key, value);
		dictionary.Add("ChildrenMap", childMap);
		settingsBase.OnSettingChanged("BlueprintData", dictionary);
	}
}
