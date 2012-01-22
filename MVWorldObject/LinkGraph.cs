using System.Collections.Generic;

public class LinkGraph
{
	public delegate void OnResetNodeDelegate(int id);

	public class LinkGraphNode
	{
		private List<LinkGraphNode> toNeighbors = new List<LinkGraphNode>();

		private List<LinkGraphNode> fromNeighbors = new List<LinkGraphNode>();

		private int woID;

		internal bool dirtyFullRecursion;

		public List<LinkGraphNode> ToNeighbors => toNeighbors;

		public List<LinkGraphNode> FromNeighbors => fromNeighbors;

		public int WorldObjectID => woID;

		public LinkGraphNode(int woID)
		{
			this.woID = woID;
		}
	}

	public OnResetNodeDelegate OnResetNode;

	private List<LinkGraphNode> nodeSet = new List<LinkGraphNode>();

	public List<LinkGraphNode> Nodes => nodeSet;

	public int Count => nodeSet.Count;

	public void AddNode(LinkGraphNode node)
	{
		nodeSet.Add(node);
	}

	public LinkGraphNode AddNode(int worldObjectID)
	{
		LinkGraphNode linkGraphNode = new LinkGraphNode(worldObjectID);
		nodeSet.Add(linkGraphNode);
		return linkGraphNode;
	}

	public bool RemoveNode(int worldObjectID)
	{
		LinkGraphNode linkGraphNode = Find(worldObjectID);
		if (linkGraphNode == null)
		{
			return false;
		}
		nodeSet.Remove(linkGraphNode);
		foreach (LinkGraphNode item in nodeSet)
		{
			int num = item.ToNeighbors.IndexOf(linkGraphNode);
			if (num != -1)
			{
				item.ToNeighbors.RemoveAt(num);
			}
		}
		return true;
	}

	public bool AddLink(int fromWorldObjectID, int toWorldObjectID)
	{
		LinkGraphNode linkGraphNode = Find(fromWorldObjectID);
		LinkGraphNode linkGraphNode2 = Find(toWorldObjectID);
		if (linkGraphNode == null)
		{
			linkGraphNode = AddNode(fromWorldObjectID);
		}
		if (linkGraphNode2 == null)
		{
			linkGraphNode2 = AddNode(toWorldObjectID);
		}
		return AddLink(linkGraphNode, linkGraphNode2);
	}

	public bool AddLink(LinkGraphNode from, LinkGraphNode to)
	{
		if (!ValidateLink(from, to))
		{
			return false;
		}
		from.ToNeighbors.Add(to);
		to.FromNeighbors.Add(from);
		return true;
	}

	public bool RemoveLink(int fromWorldObjectID, int toWorldObjectID)
	{
		LinkGraphNode linkGraphNode = Find(fromWorldObjectID);
		LinkGraphNode linkGraphNode2 = Find(toWorldObjectID);
		if (linkGraphNode == null || linkGraphNode2 == null)
		{
			return false;
		}
		return RemoveLink(linkGraphNode, linkGraphNode2);
	}

	public bool RemoveLink(LinkGraphNode from, LinkGraphNode to)
	{
		to.FromNeighbors.Remove(from);
		return from.ToNeighbors.Remove(to);
	}

	public bool Contains(int worldObjectID)
	{
		foreach (LinkGraphNode item in nodeSet)
		{
			if (item.WorldObjectID == worldObjectID)
			{
				return true;
			}
		}
		return false;
	}

	public LinkGraphNode Find(int worldObjectID)
	{
		foreach (LinkGraphNode item in nodeSet)
		{
			if (item.WorldObjectID == worldObjectID)
			{
				return item;
			}
		}
		return null;
	}

	public bool ValidateLink(int fromWorldObjectID, int toWorldObjectID)
	{
		LinkGraphNode linkGraphNode = Find(fromWorldObjectID);
		LinkGraphNode linkGraphNode2 = Find(toWorldObjectID);
		if (linkGraphNode == null || linkGraphNode2 == null)
		{
			return true;
		}
		return ValidateLink(linkGraphNode, linkGraphNode2);
	}

	public bool ValidateLink(LinkGraphNode fromNode, LinkGraphNode toNode)
	{
		bool result = true;
		ValidateLinkRecursive(fromNode, toNode, ref result);
		return result;
	}

	private void ValidateLinkRecursive(LinkGraphNode fromNode, LinkGraphNode toNode, ref bool result)
	{
		foreach (LinkGraphNode toNeighbor in toNode.ToNeighbors)
		{
			if (toNeighbor == fromNode)
			{
				result = false;
			}
			else
			{
				ValidateLinkRecursive(fromNode, toNeighbor, ref result);
			}
		}
	}

	public void ResetChunk(int worldObjectID)
	{
		foreach (LinkGraphNode item in nodeSet)
		{
			item.dirtyFullRecursion = false;
		}
		LinkGraphNode linkGraphNode = Find(worldObjectID);
		if (linkGraphNode == null)
		{
			return;
		}
		if (OnResetNode != null)
		{
			OnResetNode(linkGraphNode.WorldObjectID);
		}
		linkGraphNode.dirtyFullRecursion = true;
		foreach (LinkGraphNode toNeighbor in linkGraphNode.ToNeighbors)
		{
			ResetRecursiveTo(linkGraphNode, toNeighbor);
		}
		foreach (LinkGraphNode fromNeighbor in linkGraphNode.FromNeighbors)
		{
			ResetRecursiveFrom(fromNeighbor, linkGraphNode);
		}
	}

	private void ResetRecursiveTo(LinkGraphNode fromNode, LinkGraphNode toNode)
	{
		if (toNode.dirtyFullRecursion)
		{
			return;
		}
		toNode.dirtyFullRecursion = true;
		if (OnResetNode != null)
		{
			OnResetNode(toNode.WorldObjectID);
		}
		foreach (LinkGraphNode toNeighbor in toNode.ToNeighbors)
		{
			ResetRecursiveTo(toNode, toNeighbor);
		}
		foreach (LinkGraphNode fromNeighbor in toNode.FromNeighbors)
		{
			if (fromNeighbor != fromNode)
			{
				ResetRecursiveFrom(fromNeighbor, toNode);
			}
		}
	}

	private void ResetRecursiveFrom(LinkGraphNode fromNode, LinkGraphNode toNode)
	{
		if (fromNode.dirtyFullRecursion)
		{
			return;
		}
		fromNode.dirtyFullRecursion = true;
		if (OnResetNode != null)
		{
			OnResetNode(fromNode.WorldObjectID);
		}
		foreach (LinkGraphNode toNeighbor in fromNode.ToNeighbors)
		{
			if (toNeighbor != toNode)
			{
				ResetRecursiveTo(toNode, toNeighbor);
			}
		}
		foreach (LinkGraphNode fromNeighbor in fromNode.FromNeighbors)
		{
			ResetRecursiveFrom(fromNeighbor, fromNode);
		}
	}
}
