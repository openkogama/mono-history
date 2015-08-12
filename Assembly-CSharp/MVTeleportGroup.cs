using System.Collections.Generic;
using UnityEngine;

public class MVTeleportGroup : MVBlueprintBase
{
	private const string prefabPath = "Prefabs/Blueprints/TeleportGroup";

	private MVTeleporter teleporter1;

	private MVTeleporter teleporter2;

	public MVTeleporter Teleporter1 => teleporter1;

	public MVTeleporter Teleporter2 => teleporter2;

	public MVTeleportGroup(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/Blueprints/TeleportGroup", worldObjects)
	{
	}

	public override void Initialize()
	{
		base.Initialize();
		Dictionary<object, object> dictionary = (Dictionary<object, object>)Data["BlueprintData"];
		Dictionary<object, object> dictionary2 = (Dictionary<object, object>)dictionary["ChildrenMap"];
		if (dictionary2 == null)
		{
			Debug.LogWarning("TeleportGroup does not have any children. Removing it");
			MVGameController.WOCM.UnregisterWorldObject(id);
			return;
		}
		teleporter1 = RetrieveTeleporter(dictionary2, "teleporter1");
		teleporter2 = RetrieveTeleporter(dictionary2, "teleporter2");
		if (teleporter1 == null)
		{
			Debug.Log("Missing teleporter 1");
		}
		if (teleporter2 == null)
		{
			Debug.Log("Missing teleporter 2");
		}
		if (teleporter1 != null && teleporter2 != null)
		{
			teleporter1.Target = teleporter2;
			teleporter2.Target = teleporter1;
			teleporter1.InteractionFlags |= InteractionFlags.DirectlySelectable;
			teleporter2.InteractionFlags |= InteractionFlags.DirectlySelectable;
			if (HasInteractionFlag(InteractionFlags.IsPreview))
			{
				AddPreviewBoxesToTeleporters();
				teleporter1.PreviewOwnerProfileId = PreviewOwnerProfileId;
				teleporter2.PreviewOwnerProfileId = PreviewOwnerProfileId;
				teleporter1.InteractionFlags |= InteractionFlags.IsPreview;
				teleporter2.InteractionFlags |= InteractionFlags.IsPreview;
			}
			interactionFlags |= InteractionFlags.DontPushGroupToSelectionStack;
			gameObject.GetComponent<TeleportGroup>().Initialize(this);
		}
	}

	private MVTeleporter RetrieveTeleporter(Dictionary<object, object> table, string id)
	{
		if (table.ContainsKey(id))
		{
			MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient((int)table[id]);
			if (worldObjectClient == null || !(worldObjectClient is MVTeleporter))
			{
				return null;
			}
			return worldObjectClient as MVTeleporter;
		}
		return null;
	}

	public override void SetWorldObjectToPurchased()
	{
		base.SetWorldObjectToPurchased();
		teleporter1.SetWorldObjectToPurchased();
		teleporter2.SetWorldObjectToPurchased();
	}

	public override void AddPreviewBox()
	{
	}

	private void AddPreviewBoxesToTeleporters()
	{
		teleporter1.AddPreviewBox();
		teleporter2.AddPreviewBox();
	}
}
