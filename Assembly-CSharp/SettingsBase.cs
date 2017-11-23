using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsBase : MonoBehaviour
{
	[SerializeField]
	private Text headerText;

	private int woID;

	private Dictionary<object, object> result = new Dictionary<object, object>();

	public void Initialize(int woID, GameObject root, MVWorldObjectDocumentationType documentationType)
	{
		headerText.text = InventoryItem.localItemDescriptionOverride[documentationType].Name;
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

	public void Initialize(int woID, GameObject root, string header)
	{
		headerText.text = header;
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
				MVGameControllerBase.OperationRequests.UpdateWorldObjectDataPartial(woID, result);
			}
		}
	}

	public void RemoveData(string key)
	{
		result.Remove(key);
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
