using MV.WorldObject;
using UnityEngine;

internal class ESAddObjectLink : ESStateBase
{
	private ObjectLink tempLink;

	private MVWorldObjectClient wo;

	public override void Enter(EditorStateMachine esm)
	{
		wo = esm.SingleSelectedWO;
		if (wo == null)
		{
			Debug.LogWarning((object)"state started with multi-selection or no selection - there can be only one connector selected when adding object link!");
			esm.PopState();
			return;
		}
		tempLink = new ObjectLink();
		if (wo.SelectedConnector == SelectedConnector.Object)
		{
			tempLink.objectConnectorWOID = esm.SingleSelectedWO.Id;
			LineDrawManager.Instance.SetTempObjectLink(tempLink);
		}
		else
		{
			Debug.LogError((object)"Should not happen - object links can only be added when starting from object-connector");
			esm.PopState();
		}
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (!MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			return;
		}
		VoxelHit hit = default;
		if (MVGameController.Instance.WOCM.Pick(ref hit) && hit.woId != -1)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(hit.woId);
			if (worldObjectClient != null && wo.Id != hit.woId && !wo.ObjectLinkRefs.Exists((ObjectLink o) => o.objectWOID == hit.woId) && wo.ValidateObjectLinkTarget(worldObjectClient))
			{
				tempLink.objectWOID = hit.woId;
				DoAddLink();
			}
		}
		e.DeSelectAll();
		if (e.ParentGroupID == MVGameController.Instance.WOCM.RootGroup.Id)
		{
			e.Event = EditorEvent.ESTerrainEdit;
		}
		else
		{
			e.PopState();
		}
	}

	public override void Exit(EditorStateMachine esm)
	{
		LineDrawManager.Instance.SetTempObjectLink(null);
	}

	private bool DoAddLink()
	{
		MVGameController.Instance.Game.AddObjectLink(tempLink);
		return true;
	}
}
