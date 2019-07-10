using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class ObjectLinks
{
	protected readonly Dictionary<int, ObjectLink> objectLinks = new Dictionary<int, ObjectLink>();

	protected readonly Dictionary<int, ObjectLinkObjectScript> objectLinkObjects = new Dictionary<int, ObjectLinkObjectScript>();

	public ObjectLink GetObjectLink(int objectLinkID)
	{
		if (!objectLinks.ContainsKey(objectLinkID))
		{
			Debug.LogWarning("objectLinkID not found");
			return null;
		}
		return objectLinks[objectLinkID];
	}

	public void Update()
	{
		if (MVGameControllerBase.GameMode != MVGameMode.Edit || !MVGameControllerBase.MainCameraManager.IsLogicRendered)
		{
			return;
		}
		foreach (ObjectLink value in objectLinks.Values)
		{
			objectLinkObjects[value.id].UpdateLinkVisual(value);
		}
	}

	public bool Contains(int objectLinkID)
	{
		return objectLinks.ContainsKey(objectLinkID);
	}

	public bool RemoveObjectLink(ObjectLink link, MVWorldObjectClient objectConnectorWo, MVWorldObjectClient objectWo)
	{
		if (!objectLinks.ContainsKey(link.id))
		{
			Debug.LogError("Attempt to remove ObjectLink, but link is not registered");
			return false;
		}
		objectLinks.Remove(link.id);
		objectConnectorWo.RemoveObjectLink(link);
		objectWo.RemoveObjectLink(link);
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			Object.Destroy(objectLinkObjects[link.id].gameObject);
			objectLinkObjects.Remove(link.id);
		}
		return true;
	}

	public bool AddObjectLink(ObjectLink objectLink, MVWorldObjectClient objectConnectorWo, MVWorldObjectClient objectWo)
	{
		objectLinks.Add(objectLink.id, objectLink);
		objectConnectorWo.AddObjectLink(objectLink);
		objectWo.AddObjectLink(objectLink);
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			ObjectLinkObjectScript objectLinkObjectScript = Object.Instantiate(PrefabPool.Instance.ObjectLinkObject);
			objectLinkObjectScript.Initialize(objectLink);
			objectLinkObjects.Add(objectLink.id, objectLinkObjectScript);
		}
		return true;
	}
}
