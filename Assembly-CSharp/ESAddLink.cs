using MV.WorldObject;
using UnityEngine;

internal class ESAddLink : ESStateBase
{
	private Link tempLink;

	private MVWorldObjectClient wo;

	public override void Enter(EditorStateMachine esm)
	{
		Debug.Log((object)"ESAddLink enter");
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
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if (!MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			return;
		}
		VoxelHit hit = default;
		if (MVGameController.Instance.WOCM.Pick(ref hit) && hit.woId != -1)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(hit.woId);
			if (worldObjectClient != null)
			{
				if (worldObjectClient.HasInputConnector && e.SingleSelectedWO.SelectedConnector == SelectedConnector.Output)
				{
					if (worldObjectClient.IsPointOverInputConnector(Input.mousePosition))
					{
						tempLink.inputWOID = hit.woId;
						DoAddLink();
					}
				}
				else if (worldObjectClient.HasOutputConnector && e.SingleSelectedWO.SelectedConnector == SelectedConnector.Input && worldObjectClient.IsPointOverOutputConnector(Input.mousePosition))
				{
					tempLink.outputWOID = hit.woId;
					DoAddLink();
				}
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

	private bool DoAddLink()
	{
		if (!MVGameController.Instance.Game.AddLink(tempLink))
		{
			return false;
		}
		return true;
	}

	public override void Exit(EditorStateMachine esm)
	{
		wo.HighlightConnector(state: false);
		LineDrawManager.Instance.SetTempLink(null);
	}
}
