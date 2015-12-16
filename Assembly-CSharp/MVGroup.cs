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

	public MVGroup(Dictionary<object, object> data, GameObject prefabObject, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, prefabObject, worldObjects)
	{
		CreateGroup();
	}

	public MVGroup(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
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
		Transform transform = wo.Transform;
		Matrix4x4 m = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
		Bounds localBounds = wo.GetLocalBounds(boundsContext);
		return MathFunctions.FastAABBTransform(m, localBounds);
	}

	public static Bounds ComputeBoundsForWOs(List<MVWorldObjectClient> woList, BoundsContext boundsContext)
	{
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
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(id);
		MVGroup mVGroup = (MVGroup)MVGameControllerBase.WOCM.GetWorldObjectClient(worldObjectClient.GroupId);
		mVGroup.RemoveChild(id);
		worldObjectClient.Transform.parent = transform;
		worldObjectClient.GroupId = base.id;
		worldObjectClient.Group = this;
		children.Add(id, worldObjectClient);
	}

	public virtual void AddChild(MVWorldObjectClient child)
	{
		Vector3 vector = child.Position;
		Quaternion quaternion = child.Rotation;
		Vector3 vector2 = child.Scale;
		child.Transform.parent = gameObject.transform;
		child.Position = vector;
		child.Rotation = quaternion;
		child.Scale = vector2;
		if (children.ContainsKey(child.Id))
		{
			Debug.Log("Group " + Id + "already contains child " + child.Id);
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
			leafId = MVGameControllerBase.WOCM.GetWorldObjectClient(leafId).GroupId;
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
			int num = MVGameControllerBase.WOCM.GetWorldObjectClient(result).GroupId;
			if (num == -1)
			{
				return -1;
			}
			if (returnParentIfHasFlags != InteractionFlags.None)
			{
				MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(num);
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
		MVWorldObjectClientManager wOCM = MVGameControllerBase.WOCM;
		MVWorldObjectClient worldObjectClient = wOCM.GetWorldObjectClient(childId);
		MVWorldObjectClient worldObjectClient2 = wOCM.GetWorldObjectClient(parentId);
		if (worldObjectClient == null)
		{
			Debug.LogWarning("childId is not valid. Id is: " + childId);
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
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(child.GroupId);
		return GetParentBelow(parent, worldObjectClient);
	}

	public override bool OnEnterObject(EditorStateMachine e)
	{
		MVGameControllerBase.CameraController.CurCamera.FocusOnObject(this);
		Debug.Log("*** Entering group: " + ToString());
		e.EnterGroup(this);
		SharedCubeFunctions.SetLayerRecursively(e.ParentGroup.Transform, select: true);
		e.CameraController.BlueModeEnabled = true;
		e.Event = EditorEvent.ObjectSelected;
		return true;
	}
}
