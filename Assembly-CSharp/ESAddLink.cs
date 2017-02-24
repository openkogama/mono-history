using MV.WorldObject;
using UnityEngine;

internal class ESAddLink : ESStateBase
{
	private Link tempLink;

	private MVWorldObjectClient wo;

	public override void Enter(EditorStateMachine esm)
	{
		Debug.Log("ESAddLink enter");
		wo = esm.SingleSelectedWO;
		if (wo == null)
		{
			Debug.LogWarning("state started with multi-selection or no selection - there can be only one connector selected when adding link!");
			esm.PopState();
			return;
		}
		tempLink = new Link();
		if (wo.SelectedConnector == SelectedConnector.Input)
		{
			tempLink.inputWOID = esm.SingleSelectedWO.Id;
		}
		else
		{
			if (wo.SelectedConnector != SelectedConnector.Output)
			{
				Debug.LogError("Should not happen - links can only be added when starting from either an input- or output-connector");
				esm.PopState();
				return;
			}
			tempLink.outputWOID = esm.SingleSelectedWO.Id;
		}
		MVGameControllerBase.CameraController.LineDrawManager.SetTempLink(tempLink);
		wo.HighlightConnector(state: true);
	}

	public override void Execute(EditorStateMachine e)
	{
		if (!MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
		{
			return;
		}
		VoxelHit hit = default;
		if (EditModeObjectPicker.Pick(ref hit) && hit.woId != -1)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(hit.woId);
			if (worldObjectClient != null)
			{
				if (worldObjectClient.HasInputConnector && e.SingleSelectedWO.SelectedConnector == SelectedConnector.Output)
				{
					if (worldObjectClient.IsPointOverInputConnector(MVInputWrapper.GetPointerPosition()))
					{
						tempLink.inputWOID = hit.woId;
						DoAddLink();
					}
				}
				else if (worldObjectClient.HasOutputConnector && e.SingleSelectedWO.SelectedConnector == SelectedConnector.Input && worldObjectClient.IsPointOverOutputConnector(MVInputWrapper.GetPointerPosition()))
				{
					tempLink.outputWOID = hit.woId;
					DoAddLink();
				}
			}
		}
		e.DeSelectAll();
		if (e.ParentGroupID == MVGameControllerBase.WOCM.RootGroup.Id)
		{
			e.Event = EditorEvent.ESTerrainEdit;
		}
		else
		{
			e.PopState();
		}
	}

	private bool DoAddLink()
	{
		if (!MVGameControllerBase.OperationRequests.AddLink(tempLink))
		{
			return false;
		}
		return true;
	}

	public override void Exit(EditorStateMachine esm)
	{
		wo.HighlightConnector(state: false);
		MVGameControllerBase.CameraController.LineDrawManager.SetTempLink(null);
	}
}
