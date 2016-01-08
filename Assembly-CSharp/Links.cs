using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class Links
{
	private readonly Dictionary<int, Link> links = new Dictionary<int, Link>();

	private readonly Queue<Link> pendingLinkQueue = new Queue<Link>();

	private readonly Queue<Link> pendingRemoveLinkQueue = new Queue<Link>();

	private readonly Dictionary<int, GameObject> linkObjects = new Dictionary<int, GameObject>();

	private readonly LinkGraph linkGraph = new LinkGraph();

	public Links(LinkGraph.OnResetNodeDelegate onResetNode)
	{
		LinkGraph linkGraph = this.linkGraph;
		linkGraph.OnResetNode = (LinkGraph.OnResetNodeDelegate)Delegate.Combine(linkGraph.OnResetNode, onResetNode);
	}

	public bool RemoveLink(int linkID, MVWorldObjectClient outputWo, MVWorldObjectClient inputWo)
	{
		if (!links.ContainsKey(linkID))
		{
			Debug.LogError("Attempt to remove link with id: " + linkID + ", but link not registered!");
			return false;
		}
		Link link = links[linkID];
		links.Remove(link.id);
		linkGraph.RemoveLink(link.outputWOID, link.inputWOID);
		UnityEngine.Object.Destroy(linkObjects[link.id]);
		linkObjects.Remove(link.id);
		outputWo.RemoveOutputLink(link);
		inputWo.RemoveInputLink(link);
		return true;
	}

	public void Update()
	{
		foreach (KeyValuePair<int, Link> link in links)
		{
			LineDrawManager.Instance.ShowLink(link.Value, linkObjects[link.Key]);
		}
		foreach (Link item in pendingLinkQueue)
		{
			LineDrawManager.Instance.DrawPendingLink(item);
		}
	}

	public void ResetChunk(int worldObjectID)
	{
		linkGraph.ResetChunk(worldObjectID);
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

	public bool AddLink(Link link, MVWorldObjectClient outputWo, MVWorldObjectClient inputWo)
	{
		if (!ValidateLink(link, outputWo, inputWo))
		{
			return false;
		}
		links.Add(link.id, link);
		linkGraph.AddLink(link.outputWOID, link.inputWOID);
		GameObject gameObject = UnityEngine.Object.Instantiate(PrefabPool.Instance.LinkObject);
		gameObject.GetComponentInChildren<LinkObjectScript>().linkID = link.id;
		linkObjects.Add(link.id, gameObject);
		outputWo.AddOutputLink(link);
		inputWo.AddInputLink(link);
		return true;
	}

	public bool ValidateLink(Link link, MVWorldObjectClient outputWo, MVWorldObjectClient inputWo)
	{
		if (link.outputWOID <= 0 || link.inputWOID <= 0)
		{
			return false;
		}
		if (link.inputWOID == link.outputWOID)
		{
			return false;
		}
		if (outputWo == null)
		{
			return false;
		}
		if (inputWo == null)
		{
			return false;
		}
		if (!outputWo.ValidateLink(link))
		{
			return false;
		}
		if (!linkGraph.ValidateLink(link.outputWOID, link.inputWOID))
		{
			return false;
		}
		if (link.inputWOID <= 0 || link.outputWOID <= 0)
		{
			Debug.LogError("Attempt to add link, but link not added to input/output WO's");
			return false;
		}
		return true;
	}

	public void RemovePendingLink(int linkID, MVWorldObjectClient outputWo, MVWorldObjectClient inputWo)
	{
		if (!links.ContainsKey(linkID))
		{
			Debug.LogError("Attempt to RemovePending link, but link not registered");
			return;
		}
		Link item = links[linkID];
		RemoveLink(linkID, outputWo, inputWo);
		pendingRemoveLinkQueue.Enqueue(item);
	}

	public Link DequeuePendingLink()
	{
		return pendingLinkQueue.Dequeue();
	}

	public Link DequeuePendingRemoveLink()
	{
		return pendingRemoveLinkQueue.Dequeue();
	}

	public void AddPendingLink(Link link)
	{
		pendingLinkQueue.Enqueue(link);
	}
}
