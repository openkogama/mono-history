using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionController : ISelectionController
{
	private HashSet<int> selectedIDs = new HashSet<int>();

	private Stack<int> parentGroups = new Stack<int>();

	private MVWorldObjectClientManager WOCM => MVGameControllerBase.WOCM;

	public int ParentGroupID => parentGroups.Peek();

	public MVGroup ParentGroup => (MVGroup)WOCM.GetWorldObjectClient(ParentGroupID);

	public HashSet<int> SelectedIDs => selectedIDs;

	public HashSet<MVWorldObjectClient> SelectedWOs
	{
		get
		{
			HashSet<MVWorldObjectClient> hashSet = new HashSet<MVWorldObjectClient>();
			foreach (int selectedID in selectedIDs)
			{
				hashSet.Add(WOCM.GetWorldObjectClient(selectedID));
			}
			return hashSet;
		}
	}

	public MVWorldObjectClient SingleSelectedWO
	{
		get
		{
			if (SelectedIDs.Count == 1)
			{
				return WOCM.GetWorldObjectClient(selectedIDs.First());
			}
			if (0 < SelectedIDs.Count)
			{
				Debug.LogWarning("Trying to access single selected even though multiple objects are selected");
			}
			return null;
		}
	}

	public event EventHandler<WorldObjectDestroyedEventArgs> SelectedWorldObjectDeleted = delegate
	{
	};

	public SelectionController()
	{
		parentGroups.Push(WOCM.RootGroup.Id);
	}

	private void WOCM_WorldObjectDestroyedHandler(object sender, WorldObjectDestroyedEventArgs e)
	{
		bool flag = false;
		if (selectedIDs.Contains(e.WordObjectID))
		{
			flag = true;
			selectedIDs.Remove(e.WordObjectID);
		}
		if (parentGroups.Contains(e.WordObjectID))
		{
			flag = true;
			while (e.WordObjectID != parentGroups.Pop())
			{
			}
		}
		if (flag)
		{
			SelectedWorldObjectDeleted(this, e);
		}
	}

	private void PushWOParents(MVWorldObjectClient wo, bool addAsParent = false)
	{
		while (1 < parentGroups.Count)
		{
			parentGroups.Pop();
		}
		Queue<int> queue = new Queue<int>();
		MVWorldObjectClient worldObjectClient = WOCM.GetWorldObjectClient(wo.GroupId);
		while (worldObjectClient.Id != WOCM.RootGroup.Id)
		{
			if (!worldObjectClient.HasInteractionFlag(InteractionFlags.DontPushGroupToSelectionStack))
			{
				queue.Enqueue(worldObjectClient.Id);
			}
			worldObjectClient = WOCM.GetWorldObjectClient(worldObjectClient.GroupId);
		}
		while (queue.Count != 0)
		{
			parentGroups.Push(queue.Dequeue());
		}
		if (addAsParent)
		{
			parentGroups.Push(wo.Id);
		}
		foreach (int parentGroup in parentGroups)
		{
			WOCM.SubscribeWODestroyedEvent(parentGroup, WOCM_WorldObjectDestroyedHandler);
		}
	}

	public WorldObjectClientRef SelectWO(int id, bool addToSelection = false, bool showVisuals = true)
	{
		MVWorldObjectClient worldObjectClient = WOCM.GetWorldObjectClient(id);
		if (!addToSelection)
		{
			DeSelectAllExcept(id);
		}
		if (worldObjectClient.OwnerActorNr != 0 && worldObjectClient.OwnerActorNr != MVGameControllerBase.Game.LocalPlayer.ActorNr)
		{
			Debug.LogWarning("Trying to select WO " + id + " that is owned by another acotr");
			return MVWorldObjectClientManager.GetWorldObjectClientRefNullRef();
		}
		if (!MVGroup.IsDescendant(ParentGroupID, id) && !worldObjectClient.HasInteractionFlag(InteractionFlags.DirectlySelectable))
		{
			Debug.LogWarning("Trying to select WO " + id + " outside the parent group " + ParentGroupID);
			return MVWorldObjectClientManager.GetWorldObjectClientRefNullRef();
		}
		if (ParentGroupID != worldObjectClient.GroupId)
		{
			PushWOParents(worldObjectClient);
		}
		selectedIDs.Add(worldObjectClient.Id);
		WOCM.SubscribeWODestroyedEvent(worldObjectClient.Id, WOCM_WorldObjectDestroyedHandler);
		if (showVisuals)
		{
			worldObjectClient.Select(Color.blue);
		}
		else
		{
			worldObjectClient.Select();
		}
		return WOCM.GetWorldObjectClientRef(worldObjectClient.Id);
	}

	public WorldObjectClientRef Select(bool addToSelection = false, bool showVisuals = true, int layerMask = -5)
	{
		VoxelHit hit = default;
		if (!EditModeObjectPicker.Pick(ref hit, null, layerMask))
		{
			return null;
		}
		return Select(hit, addToSelection, showVisuals);
	}

	public WorldObjectClientRef Select(VoxelHit hit, bool addToSelection = false, bool showVisuals = true)
	{
		if ((hit.interactionFlags & InteractionFlags.Selectable) == 0)
		{
			return null;
		}
		MVWorldObjectClient worldObjectClient = WOCM.GetWorldObjectClient(hit.woId);
		bool flag = (hit.interactionFlags & InteractionFlags.DirectlySelectable) == InteractionFlags.DirectlySelectable;
		if ((hit.interactionFlags & InteractionFlags.SelectionRequiresEditGroup) == InteractionFlags.SelectionRequiresEditGroup || (!parentGroups.Contains(worldObjectClient.GroupId) && !flag))
		{
			int groupAbove = MVGroup.GetGroupAbove(ParentGroupID, worldObjectClient.Id, InteractionFlags.DirectlySelectable);
			if (groupAbove == -1)
			{
				Debug.Log("Could not find appropriate group Id");
				return WOCM.GetWorldObjectClientRef(-1);
			}
			return SelectWO(groupAbove, addToSelection, showVisuals);
		}
		return SelectWO(worldObjectClient.Id, addToSelection, showVisuals);
	}

	public bool SelectParent(bool showVisuals = true)
	{
		DeSelectAll();
		if (ParentGroupID != WOCM.RootGroup.Id)
		{
			return SelectWO(ParentGroupID, addToSelection: false, showVisuals) != null;
		}
		return false;
	}

	public void DeSelectAll()
	{
		foreach (int selectedID in selectedIDs)
		{
			WOCM.GetWorldObjectClient(selectedID).DeSelect();
			WOCM.UnsubscribeWODestroyedEvent(selectedID, WOCM_WorldObjectDestroyedHandler);
		}
		selectedIDs.Clear();
	}

	public void DeSelectAllExcept(int id)
	{
		if (!selectedIDs.Contains(id))
		{
			DeSelectAll();
			return;
		}
		foreach (int selectedID in selectedIDs)
		{
			if (selectedID != id)
			{
				WOCM.GetWorldObjectClient(selectedID).DeSelect();
				WOCM.UnsubscribeWODestroyedEvent(selectedID, WOCM_WorldObjectDestroyedHandler);
			}
		}
		selectedIDs.RemoveWhere((int s) => s != id);
	}

	public void DeSelectWorldObject(MVWorldObjectClient wo)
	{
		wo.DeSelect();
		WOCM.UnsubscribeWODestroyedEvent(wo.Id, WOCM_WorldObjectDestroyedHandler);
		selectedIDs.Remove(wo.Id);
	}

	public void EnterGroup(MVGroup group)
	{
		DeSelectAll();
		PushWOParents(group, addAsParent: true);
	}

	public int ExitGroup()
	{
		DeSelectAll();
		if (1 < parentGroups.Count)
		{
			int num = parentGroups.Pop();
			WOCM.UnsubscribeWODestroyedEvent(num, WOCM_WorldObjectDestroyedHandler);
			return num;
		}
		Debug.LogWarning("Trying to exit root group!");
		return parentGroups.Peek();
	}

	public int ExitGroupToRoot()
	{
		DeSelectAll();
		while (1 < parentGroups.Count)
		{
			int woID = parentGroups.Pop();
			WOCM.UnsubscribeWODestroyedEvent(woID, WOCM_WorldObjectDestroyedHandler);
		}
		return parentGroups.Peek();
	}

	public bool IsSelected(int id)
	{
		foreach (int selectedID in selectedIDs)
		{
			if (id == selectedID)
			{
				return true;
			}
			if (IsChildOf(id, selectedID))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsChildOf(int childId, int parentId)
	{
		MVWorldObjectClient worldObjectClient = WOCM.GetWorldObjectClient(childId);
		MVWorldObjectClient worldObjectClient2 = WOCM.GetWorldObjectClient(parentId);
		return IsChildOf(worldObjectClient, worldObjectClient2);
	}

	public bool IsChildOf(MVWorldObjectClient child, MVWorldObjectClient parent)
	{
		if (child.GroupId == -1)
		{
			return false;
		}
		if (child.GroupId == parent.Id)
		{
			return true;
		}
		MVWorldObjectClient worldObjectClient = WOCM.GetWorldObjectClient(child.GroupId);
		return IsChildOf(worldObjectClient, parent);
	}
}
