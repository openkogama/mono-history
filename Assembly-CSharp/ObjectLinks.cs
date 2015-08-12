using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class ObjectLinks
{
	protected readonly Dictionary<int, ObjectLink> objectLinks = new Dictionary<int, ObjectLink>();

	protected readonly Queue<ObjectLink> pendingObjectLinkQueue = new Queue<ObjectLink>();

	protected readonly Queue<ObjectLink> pendingRemoveObjectLinkQueue = new Queue<ObjectLink>();

	protected readonly Dictionary<int, GameObject> objectLinkObjects = new Dictionary<int, GameObject>();

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
		foreach (KeyValuePair<int, ObjectLink> objectLink in objectLinks)
		{
			LineDrawManager.Instance.ShowObjectLink(objectLink.Value, objectLinkObjects[objectLink.Key]);
		}
		foreach (ObjectLink item in pendingObjectLinkQueue)
		{
			LineDrawManager.Instance.DrawPendingObjectLink(item);
		}
	}

	public bool Contains(int objectLinkID)
	{
		return objectLinks.ContainsKey(objectLinkID);
	}

	public void RemovePendingObjectLink(ObjectLink link, MVWorldObjectClient objectConnectorWo)
	{
		if (!objectLinks.ContainsKey(link.id))
		{
			Debug.LogError("Attempt to RemovePending ObjectLink, but link not registered");
			return;
		}
		RemoveObjectLink(link, objectConnectorWo);
		pendingRemoveObjectLinkQueue.Enqueue(link);
	}

	public void AddPendingObjectLink(ObjectLink link)
	{
		pendingObjectLinkQueue.Enqueue(link);
	}

	public bool RemoveObjectLink(ObjectLink link, MVWorldObjectClient objectConnectorWo)
	{
		if (!objectLinks.ContainsKey(link.id))
		{
			Debug.LogError("Attempt to remove ObjectLink, but link is not registered");
			return false;
		}
		objectLinks.Remove(link.id);
		Object.Destroy(objectLinkObjects[link.id]);
		objectLinkObjects.Remove(link.id);
		objectConnectorWo.RemoveObjectLink(link);
		return true;
	}

	public bool ValidateObjectLink(ObjectLink objectLink, MVWorldObjectClient objectConnectorWo, MVWorldObjectClient objectWo)
	{
		if (objectLinks.ContainsKey(objectLink.id))
		{
			Debug.LogError("Attempt to add ObjectLink, but link with id already registered!");
			return false;
		}
		if (objectLink.objectConnectorWOID <= 0 || objectLink.objectWOID <= 0)
		{
			Debug.LogError("Attempt to add ObjectLink, but link is not connected!");
			return false;
		}
		if (objectConnectorWo == null || objectWo == null)
		{
			Debug.LogError("Attempt to add ObjectLink, but one of the ends points to unregistered WorldObjects");
			return false;
		}
		return true;
	}

	public bool AddObjectLink(ObjectLink objectLink, MVWorldObjectClient objectConnectorWo, MVWorldObjectClient objectWo)
	{
		if (!ValidateObjectLink(objectLink, objectConnectorWo, objectWo))
		{
			return false;
		}
		objectLinks.Add(objectLink.id, objectLink);
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/ObjectLinkObject"));
		gameObject.GetComponentInChildren<LinkObjectScript>().linkID = objectLink.id;
		gameObject.GetComponentInChildren<LinkObjectScript>().isObjectLink = true;
		objectLinkObjects.Add(objectLink.id, gameObject);
		objectConnectorWo.AddObjectLink(objectLink);
		return true;
	}

	public ObjectLink DequeuePendingObjectLink()
	{
		return pendingObjectLinkQueue.Dequeue();
	}

	public ObjectLink DequeuePendingRemoveObjectLink()
	{
		return pendingRemoveObjectLinkQueue.Dequeue();
	}
}
