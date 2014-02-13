using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVGroup : MVWorldObjectClient
{
	protected Dictionary<int, MVWorldObjectClient> children = new Dictionary<int, MVWorldObjectClient>();

	public List<MVWorldObjectClient> Children
	{
		get
		{
			List<MVWorldObjectClient> list = new List<MVWorldObjectClient>();
			foreach (MVWorldObjectClient value in children.Values)
			{
				list.Add(value);
			}
			return list;
		}
	}

	public MVGroup(Hashtable data, string prefabPath, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, prefabPath, worldObjects)
	{
		CreateGroup();
	}

	public MVGroup(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		CreateGroup();
	}

	private void CreateGroup()
	{
		interactionFlags |= InteractionFlags.Selectable | InteractionFlags.CanRotateY | InteractionFlags.CanClone | InteractionFlags.CantAddChildren;
	}

	public HashSet<int> GetHierarchyWorldObjectIDs()
	{
		HashSet<int> hashSet = new HashSet<int>();
		hashSet.Add(id);
		foreach (MVWorldObjectClient child in Children)
		{
			if (child is MVGroup)
			{
				hashSet.UnionWith((child as MVGroup).GetHierarchyWorldObjectIDs());
			}
			else
			{
				hashSet.Add(child.Id);
			}
		}
		return hashSet;
	}

	private static Bounds ComputeLocalChildBounds(MVWorldObjectClient wo, BoundsContext boundsContext)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Transform val = wo.Transform;
		Matrix4x4 m = Matrix4x4.TRS(val.localPosition, val.localRotation, val.localScale);
		Bounds localBounds = wo.GetLocalBounds(boundsContext);
		return MathFunctions.FastAABBTransform(m, localBounds);
	}

	public static Bounds ComputeBoundsForWOs(List<MVWorldObjectClient> woList, BoundsContext boundsContext)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (woList.Count == 0)
		{
			return new Bounds(Vector3.zero, Vector3.zero);
		}
		Bounds result = ComputeLocalChildBounds(woList[0], boundsContext);
		for (int i = 1; i < woList.Count; i++)
		{
			result.Encapsulate(ComputeLocalChildBounds(woList[i], boundsContext));
		}
		return result;
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ComputeBoundsForWOs(Children, boundsContext);
	}

	public override void Select()
	{
		Selected = true;
		foreach (MVWorldObjectClient child in Children)
		{
			child.Select();
		}
	}

	public override void Select(Color color)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Selected = true;
		foreach (MVWorldObjectClient child in Children)
		{
			child.Select(color);
		}
	}

	public override void DeSelect()
	{
		Selected = false;
		foreach (MVWorldObjectClient child in Children)
		{
			child.DeSelect();
		}
	}

	public override void TraverseRecursiveTail(CallBackDelegate callBack)
	{
		callBack(this);
		foreach (MVWorldObjectClient value in children.Values)
		{
			value.TraverseRecursiveTail(callBack);
		}
	}

	public override MVWorldObjectClient Clone(int ownerActorNumber, int cloneGroupId, CloneBookkeeping cloneBookkeeping, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
	{
		MVGroup mVGroup = (MVGroup)base.Clone(ownerActorNumber, cloneGroupId, cloneBookkeeping, worldObjects, prototypes);
		List<MVWorldObjectClient> list = Children;
		list.Sort((MVWorldObjectClient w1, MVWorldObjectClient w2) => w1.Id.CompareTo(w2.Id));
		foreach (MVWorldObjectClient item in list)
		{
			item.Clone(ownerActorNumber, mVGroup.id, cloneBookkeeping, worldObjects, prototypes);
		}
		return mVGroup;
	}

	public override void Initialize()
	{
		base.Initialize();
		foreach (MVWorldObjectClient value in children.Values)
		{
			value.Initialize();
		}
	}

	public override void InitializeInventory()
	{
		base.InitializeInventory();
		foreach (MVWorldObjectClient value in children.Values)
		{
			value.InitializeInventory();
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		foreach (MVWorldObjectClient value in children.Values)
		{
			value.Destroy();
		}
	}

	public override void PlayModeInitialize()
	{
		base.PlayModeInitialize();
		foreach (MVWorldObjectClient value in children.Values)
		{
			value.PlayModeInitialize();
		}
	}

	public void RemoveChild(int childId)
	{
		children.Remove(childId);
	}

	public virtual void TransferChild(int id)
	{
		MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(id);
		MVGroup mVGroup = (MVGroup)MVGameController.Instance.WOCM.GetWorldObjectClient(worldObjectClient.GroupId);
		mVGroup.RemoveChild(id);
		worldObjectClient.Transform.parent = transform;
		worldObjectClient.GroupId = base.id;
		worldObjectClient.Group = this;
		children.Add(id, worldObjectClient);
	}

	public virtual void AddChild(MVWorldObjectClient child)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = child.Position;
		Quaternion val2 = child.Rotation;
		Vector3 val3 = child.Scale;
		child.Transform.parent = gameObject.transform;
		child.Position = val;
		child.Rotation = val2;
		child.Scale = val3;
		if (children.ContainsKey(child.Id))
		{
			Debug.Log((object)("Group " + Id + "already contains child " + child.Id));
		}
		else
		{
			children.Add(child.Id, child);
		}
	}

	public MVWorldObjectClient GetChild(int woID)
	{
		children.TryGetValue(woID, out var value);
		return value;
	}

	public static bool IsDescendant(int parentId, int leafId)
	{
		do
		{
			leafId = MVGameController.Instance.WOCM.GetWorldObjectClient(leafId).GroupId;
			if (leafId == -1)
			{
				return false;
			}
		}
		while (leafId != parentId);
		return true;
	}

	public static int GetGroupAbove(int currentParent, int leaf, InteractionFlags returnParentIfHasFlags = InteractionFlags.None)
	{
		int result = leaf;
		while (true)
		{
			int num = MVGameController.Instance.WOCM.GetWorldObjectClient(result).GroupId;
			if (num == -1)
			{
				return -1;
			}
			if (returnParentIfHasFlags != InteractionFlags.None)
			{
				MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(num);
				if (worldObjectClient.HasInteractionFlag(returnParentIfHasFlags))
				{
					return num;
				}
			}
			if (currentParent == num)
			{
				break;
			}
			result = num;
		}
		return result;
	}

	public static int GetParentBelow(int parentId, int childId)
	{
		MVWorldObjectClientManager wOCM = MVGameController.Instance.WOCM;
		MVWorldObjectClient worldObjectClient = wOCM.GetWorldObjectClient(childId);
		MVWorldObjectClient worldObjectClient2 = wOCM.GetWorldObjectClient(parentId);
		if (worldObjectClient == null)
		{
			Debug.LogWarning((object)("childId is not valid. Id is: " + childId));
			return -1;
		}
		return GetParentBelow(worldObjectClient2, worldObjectClient);
	}

	public static int GetParentBelow(MVWorldObjectClient parent, MVWorldObjectClient child)
	{
		if (child == null)
		{
			return -1;
		}
		if (child.GroupId == -1)
		{
			return -1;
		}
		if (parent.Id == child.GroupId)
		{
			return child.Id;
		}
		MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(child.GroupId);
		return GetParentBelow(parent, worldObjectClient);
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		MVGameController.Instance.Game.CameraController.CurCamera.FocusOnObject(this);
		Debug.Log((object)("*** Entering group: " + ToString()));
		e.EnterGroup(this);
		SharedCubeFunctions.SetLayerRecursively(e.ParentGroup.Transform, select: true);
		e.CameraController.SecondaryCameraActive = true;
		((Behaviour)((Component)e.CameraController).GetComponent<GrayscaleEffect>()).enabled = true;
		e.Event = EditorEvent.ObjectSelected;
		return true;
	}
}
