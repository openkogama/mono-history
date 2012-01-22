using UnityEngine;

internal class ESContextMenu : ESStateBase
{
	private Vector3 downPosition;

	public override void Enter(EditorStateMachine esm)
	{
		base.Enter(esm);
	}

	public override void Execute(EditorStateMachine e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		if (MVInputWrapper.GetKeyDown((KeyCode)324))
		{
			downPosition = Input.mousePosition;
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)324))
		{
			if (Vector3.SqrMagnitude(Input.mousePosition - downPosition) < 0.64f)
			{
				VoxelHit hit = default;
				if (!MVGameController.Instance.WOCM.Pick(ref hit) || (hit.interactionFlags & InteractionFlags.IsTerrain) != 0)
				{
					return;
				}
				if (e.SingleSelectedWO.Id != hit.woId)
				{
					if (MVGameController.Instance.guiManager.ShowContextMenu(MVGameController.Instance.WOCM.WorldObjects[hit.woId].WorldObjectType, Input.mousePosition))
					{
						e.SelectWo(hit.woId, addToSelection: false);
					}
					else
					{
						e.Event = EditorEvent.ObjectSelected;
					}
				}
				else if (!MVGameController.Instance.guiManager.ShowContextMenu(MVGameController.Instance.WOCM.WorldObjects[e.SingleSelectedWO.Id].WorldObjectType, Input.mousePosition))
				{
				}
			}
			else
			{
				e.Event = EditorEvent.ObjectSelected;
			}
		}
		else if (MVInputWrapper.GetKey((KeyCode)324) && Vector3.SqrMagnitude(Input.mousePosition - downPosition) > 0.64f)
		{
			e.Event = EditorEvent.ObjectSelected;
		}
	}

	public override void Exit(EditorStateMachine esm)
	{
	}
}
