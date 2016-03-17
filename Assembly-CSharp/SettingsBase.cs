using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsBase : MonoBehaviour
{
	private int woID;

	private Dictionary<object, object> result = new Dictionary<object, object>();

	public void Initialize(int woID, GameObject root)
	{
		this.woID = woID;
		ExecuteEvents.ExecuteHierarchy(root, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(root, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.Push(gameObject, UIPushOption.None, OnPop, UIGroupFlags.GameObjectUISubMenu);
		});
	}

	private void OnPop()
	{
		if (woID != -1)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
			if (worldObjectClient != null && result.Count != 0)
			{
				MVGameControllerBase.Game.UpdateWorldObjectDataPartial(woID, result);
			}
		}
	}

	public void OnSettingChanged(string key, object value)
	{
		if (woID == -1 || string.IsNullOrEmpty(key))
		{
			return;
		}
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
		if (worldObjectClient != null)
		{
			WorldObjectDataValidator.Validate(worldObjectClient, key, value);
			if (!result.ContainsKey(key))
			{
				result.Add(key, value);
			}
			else
			{
				result[key] = value;
			}
			worldObjectClient.PartialUpdateWOData(new Dictionary<object, object> { { key, value } });
		}
	}

	private void Update()
	{
		if (woID != -1 && MVGameControllerBase.WOCM.GetWorldObjectClient(woID) == null)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
			{
				handler.Pop();
			});
		}
	}
}
