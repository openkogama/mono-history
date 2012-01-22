using System.Collections.Generic;
using UnityEngine;

public class SelectionController
{
	private HashSet<int> selected = new HashSet<int>();

	private Stack<int> parentGroups = new Stack<int>();

	private SelectionGizmo selectionGizmo;

	public int ParentGroup
	{
		get
		{
			UpdateParentGroupStack();
			return parentGroups.Peek();
		}
	}

	public HashSet<int> Selected
	{
		get
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (int item in selected)
			{
				if (!MVGameController.Instance.WOCM.WorldObjects.ContainsKey(item))
				{
					hashSet.Add(item);
				}
			}
			selected.ExceptWith(hashSet);
			return selected;
		}
	}

	public HashSet<MVWorldObjectClient> SelectedWOs
	{
		get
		{
			HashSet<MVWorldObjectClient> hashSet = new HashSet<MVWorldObjectClient>();
			foreach (int item in Selected)
			{
				hashSet.Add(MVGameController.Instance.WOCM.GetWorldObjectClient(item));
			}
			return hashSet;
		}
	}

	public MVWorldObjectClient SingleSelectedWO
	{
		get
		{
			if (Selected.Count == 1)
			{
				return MVGameController.Instance.WOCM.GetWorldObjectClient(new List<int>(selected)[0]);
			}
			if (Selected.Count != 0)
			{
				Debug.LogWarning((object)"Trying to access single selected even though multiple objects are selected");
			}
			return null;
		}
	}

	public SelectionController()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected Obj, but got Unknown
		PushParent(MVGameController.Instance.WOCM.RootGroup.Id);
		GameObject val = new GameObject("Selection Controller Bounds Gizmo");
		selectionGizmo = val.AddComponent<SelectionGizmo>();
	}

	public void PushParent(int id)
	{
		if (!parentGroups.Contains(id))
		{
			parentGroups.Push(id);
		}
		else
		{
			Debug.LogError((object)"parent allready in stack");
		}
	}

	public int PopParent()
	{
		if (parentGroups.Count > 1)
		{
			return parentGroups.Pop();
		}
		Debug.LogError((object)"Trying to pop root of parent stack");
		return -1;
	}

	private void UpdateParentGroupStack()
	{
		if (parentGroups.Count == 0)
		{
			Debug.LogError((object)"parentGroups is 0. Is should always contain at least 1, the root");
		}
		else if (!MVGameController.Instance.WOCM.WorldObjects.ContainsKey(parentGroups.Peek()))
		{
			parentGroups.Pop();
			UpdateParentGroupStack();
		}
	}

	private bool DeSelectAll(int exceptionId)
	{
		bool result = false;
		HashSet<int> hashSet = new HashSet<int>();
		foreach (MVWorldObjectClient selectedWO in SelectedWOs)
		{
			if (selectedWO.Id != exceptionId)
			{
				hashSet.Add(selectedWO.Id);
			}
			else
			{
				result = true;
			}
		}
		foreach (int item in hashSet)
		{
			MVGameController.Instance.WOCM.GetWorldObjectClient(item).DeSelect();
		}
		selected.ExceptWith(hashSet);
		return result;
	}

	public bool SelectWo(int id, bool addToSelection)
	{
		MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(id);
		if (!addToSelection)
		{
			if (DeSelectAll(id))
			{
				return true;
			}
			if (worldObjectClient.OwnerActorNr == 0)
			{
				selected.Add(worldObjectClient.Id);
				SetSelectVisualization(worldObjectClient, select: true);
				return true;
			}
		}
		else
		{
			if (selected.Contains(id))
			{
				return true;
			}
			if (worldObjectClient.OwnerActorNr == 0 || worldObjectClient.OwnerActorNr == MVGameController.Instance.WOCM.LocalPlayer.ActorNr)
			{
				selected.Add(worldObjectClient.Id);
				SetSelectVisualization(worldObjectClient, select: true);
				return true;
			}
		}
		return false;
	}

	public void SelectNewRegisteredObject(MVWorldObjectClient wo)
	{
		foreach (int item in selected)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(item);
			if (worldObjectClient != null)
			{
				SetSelectVisualization(worldObjectClient, select: false);
			}
		}
		selected.Clear();
		selected.Add(wo.Id);
		SetSelectVisualization(wo, select: true);
	}

	private static void SetSelectVisualization(MVWorldObjectClient wo, bool select)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (wo != null)
		{
			if (select)
			{
				Color color = new Color(1f, 0.5f, 0.5f, 0.5f);
				wo.Select(color);
			}
			else
			{
				wo.DeSelect();
			}
		}
	}

	public bool IsWorldObjectSelected(int id)
	{
		if (selected.Contains(id))
		{
			return true;
		}
		if (SingleSelectedWO != null && (object)MVGameController.Instance.WOCM.GetWorldObjectClient(SingleSelectedWO.Id).GetType() == typeof(MVGroup))
		{
			return MVGroup.IsDescendant(SingleSelectedWO.Id, id);
		}
		return false;
	}

	public bool Select(bool addToSelection)
	{
		VoxelHit hit = default;
		if (MVGameController.Instance.WOCM.Pick(ref hit) && (hit.interactionFlags & InteractionFlags.Selectable) != 0)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(hit.woId);
			if (worldObjectClient.GroupId != ParentGroup)
			{
				Debug.Log((object)"not parent group");
				Debug.Log((object)("ParentGroup " + ParentGroup));
				Debug.Log((object)("wo.Id " + worldObjectClient.Id));
				Debug.Log((object)("wo.GroupId " + worldObjectClient.GroupId));
				int groupAbove = MVGroup.GetGroupAbove(ParentGroup, worldObjectClient.Id);
				if (groupAbove == -1)
				{
					Debug.Log((object)"could not find appropriate group Id");
					return false;
				}
				MVGroup mVGroup = (MVGroup)MVGameController.Instance.WOCM.GetWorldObjectClient(groupAbove);
				if (SelectWo(mVGroup.Id, addToSelection))
				{
					foreach (MVWorldObjectClient child in mVGroup.Children)
					{
						SetSelectVisualization(child, select: true);
					}
					return true;
				}
				PushParent(worldObjectClient.GroupId);
			}
			else if (SelectWo(worldObjectClient.Id, addToSelection))
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public void DeSelect()
	{
		foreach (int item in selected)
		{
			MVGameController.Instance.WOCM.GetWorldObjectClient(item).DeSelect();
		}
		Debug.Log((object)"Deselect");
		selected.Clear();
	}

	public int GetParentBelow(int parentId, int childId)
	{
		if (MVGameController.Instance.WOCM.GetWorldObjectClient(childId) == null)
		{
			Debug.LogWarning((object)("childId is not valid. Id is: " + childId));
			return -1;
		}
		if (MVGameController.Instance.WOCM.GetWorldObjectClient(childId).GroupId == -1)
		{
			return -1;
		}
		if (MVGameController.Instance.WOCM.GetWorldObjectClient(parentId).Id == MVGameController.Instance.WOCM.GetWorldObjectClient(childId).GroupId)
		{
			return childId;
		}
		return GetParentBelow(parentId, MVGameController.Instance.WOCM.GetWorldObjectClient(childId).GroupId);
	}

	public bool IsSelected(int id)
	{
		foreach (int item in selected)
		{
			if (id == item)
			{
				return true;
			}
			if (IsChildOf(item, id))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsChildOf(int parentId, int childId)
	{
		if (MVGameController.Instance.WOCM.GetWorldObjectClient(childId).GroupId == -1)
		{
			return false;
		}
		if (MVGameController.Instance.WOCM.GetWorldObjectClient(parentId).Id == MVGameController.Instance.WOCM.GetWorldObjectClient(childId).GroupId)
		{
			return true;
		}
		return IsChildOf(parentId, MVGameController.Instance.WOCM.GetWorldObjectClient(childId).GroupId);
	}

	public void UpdateSelectionGizmos()
	{
		selectionGizmo.wos = SelectedWOs;
	}
}
