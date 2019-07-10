using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class Links
{
	private readonly Dictionary<int, Link> links = new Dictionary<int, Link>();

	private readonly Dictionary<int, LinkObjectScript> linkObjects = new Dictionary<int, LinkObjectScript>();

	public bool RemoveLink(int linkID, MVWorldObjectClient outputWo, MVWorldObjectClient inputWo)
	{
		if (!links.ContainsKey(linkID))
		{
			Debug.LogError("Attempt to remove link with id: " + linkID + ", but link not registered!");
			return false;
		}
		Link link = links[linkID];
		links.Remove(link.id);
		outputWo.RemoveOutputLink(link);
		inputWo.RemoveInputLink(link);
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			Object.Destroy(linkObjects[link.id].gameObject);
			linkObjects.Remove(link.id);
		}
		return true;
	}

	public void Update()
	{
		if (MVGameControllerBase.GameMode != MVGameMode.Edit || !MVGameControllerBase.MainCameraManager.IsLogicRendered)
		{
			return;
		}
		foreach (Link value in links.Values)
		{
			linkObjects[value.id].UpdateLinkVisual(value);
		}
	}

	public bool Contains(int linkID)
	{
		return links.ContainsKey(linkID);
	}

	public Link GetLink(int linkID)
	{
		if (!links.ContainsKey(linkID))
		{
			Debug.LogWarning("Link not found");
			return null;
		}
		return links[linkID];
	}

	public void AddLink(Link link, MVWorldObjectClient outputWo, MVWorldObjectClient inputWo)
	{
		links.Add(link.id, link);
		outputWo.AddOutputLink(link);
		inputWo.AddInputLink(link);
		if (MVGameControllerBase.GameMode == MVGameMode.Edit)
		{
			LinkObjectScript linkObjectScript = Object.Instantiate(PrefabPool.Instance.LinkObject);
			linkObjectScript.Initialize(link);
			linkObjects.Add(link.id, linkObjectScript);
		}
	}
}
