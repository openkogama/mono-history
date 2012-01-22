using MV.WorldObject;
using UnityEngine;

internal class ESAddLink : ESStateBase
{
	private Link tempLink;

	private MVWorldObjectClient wo;

	public override void Enter(EditorStateMachine esm)
	{
		wo = esm.SingleSelectedWO;
		if (wo == null)
		{
			Debug.LogWarning((object)"state started with multi-selection or no selection - there can be only one connector selected when adding link!");
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
				Debug.LogError((object)"Should not happen - links can only be added when starting from either an input- or output-connector");
				esm.PopState();
				return;
			}
			tempLink.outputWOID = esm.SingleSelectedWO.Id;
		}
		LineDrawManager.Instance.SetTempLink(tempLink);
		wo.HighlightConnector(state: true);
	}

	public override void Execute(EditorStateMachine e)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		base.Execute(e);
		if (!MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			return;
		}
		VoxelHit hit = default;
		if (MVGameController.Instance.WOCM.Pick(ref hit) && hit.woId != -1 && MVGameController.Instance.WOCM.WorldObjects.ContainsKey(hit.woId))
		{
			if (MVGameController.Instance.WOCM.WorldObjects[hit.woId].HasInputConnector && e.SingleSelectedWO.SelectedConnector == SelectedConnector.Output)
			{
				if (MVGameController.Instance.WOCM.WorldObjects[hit.woId].IsPointOverInputConnector(Input.mousePosition))
				{
					tempLink.inputWOID = hit.woId;
					AddTempLink();
				}
			}
			else if (MVGameController.Instance.WOCM.WorldObjects[hit.woId].HasOutputConnector && e.SingleSelectedWO.SelectedConnector == SelectedConnector.Input && MVGameController.Instance.WOCM.WorldObjects[hit.woId].IsPointOverOutputConnector(Input.mousePosition))
			{
				tempLink.outputWOID = hit.woId;
				AddTempLink();
			}
		}
		e.PopState();
	}

	private bool AddTempLink()
	{
		if (tempLink.outputWOID <= 0 || tempLink.inputWOID <= 0)
		{
			return false;
		}
		if (tempLink.inputWOID == tempLink.outputWOID)
		{
			return false;
		}
		if (!MVGameController.Instance.WOCM.WorldObjects[tempLink.outputWOID].ValidateLink(tempLink))
		{
			return false;
		}
		if (!MVGameController.Instance.WOCM.LinkGraph.ValidateLink(tempLink.outputWOID, tempLink.inputWOID))
		{
			return false;
		}
		MVGameController.Instance.Game.AddLink(tempLink);
		return true;
	}

	public override void Exit(EditorStateMachine esm)
	{
		wo.HighlightConnector(state: false);
		LineDrawManager.Instance.SetTempLink(null);
	}
}
