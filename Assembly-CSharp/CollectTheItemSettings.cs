using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollectTheItemSettings : MonoBehaviour, IHandleSettingChanged, IEventSystemHandler
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
		Dictionary<object, object> dictionary = (Dictionary<object, object>)child.Data["BlueprintData"];
		childMap = new Dictionary<object, object>((Dictionary<object, object>)dictionary["ChildrenMap"]);
		settingsBase.Initialize(id, root, MVWorldObjectDocumentationType.CollectTheItem);
		toggle.Initialize("hasIndicator", (bool)dictionary["hasIndicator"]);
	}

	public void OnSettingChanged(string key, object value)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add(key, value);
		dictionary.Add("ChildrenMap", childMap);
		settingsBase.OnSettingChanged("BlueprintData", dictionary);
	}
}
