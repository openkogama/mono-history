using MV.WorldObject;
using UnityEngine;

internal class ESAddLink : ESStateBase
{
	private Link tempLink;

	private WorldObjectClientRef woRef;

	private Color originalRedConnectorColor;

	private Color originalBlueConnectorColor;

	public float FadeDuration = 0.7f;

	private Color startColor;

	private Color endColor;

	private float lastColorChangeTime;

	private Material materialToPulse;

	public override void Enter(EditorStateMachine esm)
	{
		Debug.Log("ESAddLink enter");
		MVWorldObjectClient singleSelectedWO = esm.SingleSelectedWO;
		originalRedConnectorColor = PrefabPool.Instance.LogicCubeConnectorRedMaterial.color;
		originalBlueConnectorColor = PrefabPool.Instance.LogicCubeConnectorBlueMaterial.color;
		if (singleSelectedWO == null)
		{
			Debug.LogWarning("state started with multi-selection or no selection - there can be only one connector selected when adding link!");
			esm.PopState();
			return;
		}
		tempLink = new Link();
		if (singleSelectedWO.SelectedConnector == SelectedConnector.Input)
		{
			tempLink.inputWOID = esm.SingleSelectedWO.Id;
			materialToPulse = PrefabPool.Instance.LogicCubeConnectorBlueMaterial;
			endColor = PrefabPool.Instance.LogicCubeConnectorBlueMaterial.color;
			startColor = new Color(endColor.r, 0.6f, endColor.b);
		}
		else
		{
			if (singleSelectedWO.SelectedConnector != SelectedConnector.Output)
			{
				Debug.LogError("Should not happen - links can only be added when starting from either an input- or output-connector");
				esm.PopState();
				return;
			}
			tempLink.outputWOID = esm.SingleSelectedWO.Id;
			materialToPulse = PrefabPool.Instance.LogicCubeConnectorRedMaterial;
			endColor = PrefabPool.Instance.LogicCubeConnectorRedMaterial.color;
			startColor = new Color(endColor.r, 0.6f, endColor.b);
		}
		MVGameControllerBase.CameraController.LineDrawManager.SetTempLink(tempLink);
		singleSelectedWO.HighlightConnector(state: true);
		woRef = MVGameControllerBase.WOCM.GetWorldObjectClientRef(singleSelectedWO.Id);
	}

	public override void Execute(EditorStateMachine e)
	{
		if (woRef.WorldObjectClient == null)
		{
			LeaveAddLink(e);
			return;
		}
		PulseColor();
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
		LeaveAddLink(e);
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
		if (woRef.WorldObjectClient != null)
		{
			woRef.WorldObjectClient.HighlightConnector(state: false);
		}
		MVGameControllerBase.CameraController.LineDrawManager.SetTempLink(null);
		PrefabPool.Instance.LogicCubeConnectorRedMaterial.color = originalRedConnectorColor;
		PrefabPool.Instance.LogicCubeConnectorBlueMaterial.color = originalBlueConnectorColor;
	}

	private void PulseColor()
	{
		float value = (Time.time - lastColorChangeTime) / FadeDuration;
		value = Mathf.Clamp01(value);
		materialToPulse.color = Color.Lerp(startColor, endColor, value);
		if (value == 1f)
		{
			lastColorChangeTime = Time.time;
			Color color = startColor;
			startColor = endColor;
			endColor = color;
		}
	}
}
