using System.Collections;
using System.Collections.Generic;
using Localize;
using UnityEngine;

public class MVMovingPlatformNode : MVWorldObjectClient
{
	private const string prefabPath = "Prefabs/Blueprints/MovingPlatformNode";

	public MVMovingPlatformNode Previous { get; set; }

	public MVMovingPlatformNode Next { get; set; }

	public MVMovingPlatformNode(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/Blueprints/MovingPlatformNode", worldObjects)
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
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Bounds bounds = GameObject.GetComponent<Collider>().bounds;
		bounds.center = Vector3.zero;
		return bounds;
	}

	public override bool Delete(MVWorldObjectClientManager WOCM, ref TextSlotIndex errorTextIndex)
	{
		MVWorldObjectClient worldObjectClient = WOCM.GetWorldObjectClient(groupId);
		if (worldObjectClient != null && worldObjectClient is MVMovingPlatformGroup)
		{
			return worldObjectClient.Delete(WOCM, ref errorTextIndex);
		}
		WOCM.UnregisterWorldObject(id);
		return true;
	}
}
