using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollectTheItemDropoffSettings : MonoBehaviour, IEventSystemHandler, IHandleSettingChanged
{
	[SerializeField]
	private SettingsBase settingsBase;

	[SerializeField]
	private SettingsToggle toggle;

	private Dictionary<object, object> childMap;

	public void Initialize(int woID, GameObject root)
	{
		MVBlueprintBase mVBlueprintBase = (MVBlueprintBase)MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
		int id = mVBlueprintBase.Id;
		Dictionary<object, object> dictionary = (Dictionary<object, object>)mVBlueprintBase.Data["BlueprintData"];
		childMap = new Dictionary<object, object>((Dictionary<object, object>)dictionary["ChildrenMap"]);
		settingsBase.Initialize(id, root);
		toggle.Initialize("doOnce", (bool)dictionary["doOnce"]);
	}

	public void OnSettingChanged(string key, object value)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add(key, value);
		dictionary.Add("ChildrenMap", childMap);
		settingsBase.OnSettingChanged("BlueprintData", dictionary);
	}
}
