using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MVGroup : MVWorldObjectClient
{
	private Hashtable children;

	public List<MVWorldObjectClient> Children
	{
		get
		{
			List<MVWorldObjectClient> list = new List<MVWorldObjectClient>();
			foreach (int key in children.Keys)
			{
				if (!MVGameController.Instance.WOCM.WorldObjects.ContainsKey(key))
				{
					Debug.LogError((object)"Attempt to add child object to group, but child-object is unknown!");
				}
				else
				{
					list.Add(MVGameController.Instance.WOCM.GetWorldObjectClient(key));
				}
			}
			return list;
		}
	}

	public virtual void OnBuildHierarchyDone(bool isLoadingWorld)
	{
	}

	public override void Select(Color color)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		foreach (MVWorldObjectClient child in Children)
		{
			child.Select(color);
		}
	}

	public override void DeSelect()
	{
		foreach (MVWorldObjectClient child in Children)
		{
			child.DeSelect();
		}
	}

	protected override void CreateMVWOC(bool local)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected Obj, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		interactionFlags = InteractionFlags.Selectable;
		gameObject = new GameObject(GetType().ToString() + "id " + id + "group id " + groupId);
		if (!Data.Contains("children"))
		{
			Data.Add("children", new Hashtable());
		}
		children = (Hashtable)Data["children"];
		gameObject.transform.localPosition = Position;
		gameObject.transform.localRotation = Rotation;
		gameObject.transform.localScale = Scale;
		if (MVGameController.Instance.WOCM.HierarchiesHasBeenCreated)
		{
			BuildHierarchy(isLoadingWorld: false);
		}
	}

	public void BuildHierarchy(bool isLoadingWorld)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		foreach (MVWorldObjectClient child in Children)
		{
			Vector3 localPosition = child.GameObject.transform.localPosition;
			Quaternion localRotation = child.GameObject.transform.localRotation;
			child.GameObject.transform.parent = gameObject.transform;
			if (isLoadingWorld)
			{
				child.GameObject.transform.localPosition = localPosition;
				child.GameObject.transform.localRotation = localRotation;
			}
			else
			{
				((MVGroup)MVGameController.Instance.WOCM.GetWorldObjectClient(child.GroupId)).RemoveChild(child.Id);
				child.GroupId = Id;
			}
			child.Position = child.GameObject.transform.localPosition;
			child.Rotation = child.GameObject.transform.localRotation;
			child.ClearTransformQueue();
		}
		if (groupId == -1)
		{
			MVGameController.Instance.WOCM.RootGroup = this;
		}
		if (!isLoadingWorld)
		{
		}
		OnBuildHierarchyDone(isLoadingWorld);
	}

	public void RemoveChild(int childId)
	{
		children.Remove(childId);
	}

	public void SetLayerForChildren(string layer)
	{
		foreach (MVWorldObjectClient child in Children)
		{
			child.GameObject.layer = LayerMask.NameToLayer(layer);
		}
	}

	public void AddChild(MVWorldObjectClient child)
	{
		if (children.Contains(child.Id))
		{
			Debug.Log((object)"contains");
			return;
		}
		child.GameObject.transform.parent = gameObject.transform;
		SharedCubeFunctions.SetLayerRecursively(child.GameObject.transform, LayerMask.LayerToName(gameObject.layer) != "Default");
		children.Add(child.Id, (byte)0);
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

	public static int GetGroupAbove(int currentParent, int leaf)
	{
		int result = leaf;
		while (true)
		{
			int num = MVGameController.Instance.WOCM.GetWorldObjectClient(result).GroupId;
			if (num == -1)
			{
				return -1;
			}
			if (currentParent == num)
			{
				break;
			}
			result = num;
		}
		return result;
	}
}
