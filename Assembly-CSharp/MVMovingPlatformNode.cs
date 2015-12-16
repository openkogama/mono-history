using System.Collections.Generic;
using UnityEngine;

public class MVMovingPlatformNode : MVWorldObjectClient
{
	public MVMovingPlatformNode Previous { get; set; }

	public MVMovingPlatformNode Next { get; set; }

	public MVMovingPlatformNode(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVMovingPlatformNodePrefab, worldObjects)
	{
		interactionFlags &= ~(InteractionFlags.CanRotateX | InteractionFlags.CanRotateY | InteractionFlags.CanRotateZ);
		interactionFlags |= InteractionFlags.Selectable | InteractionFlags.DirectlySelectable;
	}

	public override void Select(Color color)
	{
		AddSelectionBox();
		Selected = true;
	}

	public override Bounds GetLocalBounds(BoundsContext boundsContext)
	{
		Bounds bounds = GameObject.GetComponent<Collider>().bounds;
		bounds.center = Vector3.zero;
		return bounds;
	}

	public override bool Delete(MVWorldObjectClientManager WOCM, ref string errorText)
	{
		MVWorldObjectClient worldObjectClient = WOCM.GetWorldObjectClient(groupId);
		if (worldObjectClient != null && worldObjectClient is MVMovingPlatformGroup)
		{
			return worldObjectClient.Delete(WOCM, ref errorText);
		}
		WOCM.UnregisterWorldObject(id);
		return true;
	}
}
