using MV.WorldObject;
using UnityEngine;

internal class ESAddObjectLink : ESStateBase
{
	private ObjectLink tempLink;

	private WorldObjectClientRef woRef;

	public override void Enter(EditorStateMachine esm)
	{
		MVWorldObjectClient singleSelectedWO = esm.SingleSelectedWO;
		if (singleSelectedWO == null)
		{
			Debug.LogWarning("state started with multi-selection or no selection - there can be only one connector selected when adding object link!");
			esm.PopState();
			return;
		}
		tempLink = new ObjectLink();
		if (singleSelectedWO.SelectedConnector == SelectedConnector.Object)
		{
			tempLink.objectConnectorWOID = esm.SingleSelectedWO.Id;
			MVGameControllerBase.CameraController.LineDrawManager.SetTempObjectLink(tempLink);
			woRef = MVGameControllerBase.WOCM.GetWorldObjectClientRef(singleSelectedWO.Id);
		}
		else
		{
			Debug.LogError("Should not happen - object links can only be added when starting from object-connector");
			esm.PopState();
		}
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		MVWorldObjectClient worldObjectClient = woRef.WorldObjectClient;
		if (worldObjectClient == null)
		{
			LeaveAddLink(e);
		}
		else
		{
			if (!MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
			{
				return;
			}
			VoxelHit hit = default;
			if (EditModeObjectPicker.Pick(ref hit) && hit.woId != -1)
			{
				MVWorldObjectClient worldObjectClient2 = MVGameControllerBase.WOCM.GetWorldObjectClient(hit.woId);
				if (worldObjectClient2 != null && worldObjectClient.Id != hit.woId && !worldObjectClient.ObjectLinkRefs.Exists((ObjectLink o) => o.objectWOID == hit.woId) && worldObjectClient.ValidateObjectLinkTarget(worldObjectClient2))
				{
					tempLink.objectWOID = hit.woId;
					DoAddLink();
				}
			}
			e.DeSelectAll();
			LeaveAddLink(e);
		}
	}

	private void LeaveAddLink(EditorStateMachine e)
	{
		if (e.ParentGroupID == MVGameControllerBase.WOCM.RootGroup.Id)
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
		MVGameControllerBase.CameraController.LineDrawManager.SetTempObjectLink(null);
	}

	private bool DoAddLink()
	{
		MVGameControllerBase.OperationRequests.AddObjectLink(tempLink);
		return true;
	}
}
