using UnityEngine;

internal class ESSettingsDialog : ESStateBase
{
	private Vector3 downPosition;

	public override void Enter(EditorStateMachine esm)
	{
		base.Enter(esm);
	}

	public override void Execute(EditorStateMachine e)
	{
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelectAlt))
		{
			downPosition = MVInputWrapper.GetPointerPosition();
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelectAlt))
		{
			if (Vector3.SqrMagnitude(MVInputWrapper.GetPointerPosition() - downPosition) < 0.64f)
			{
				VoxelHit hit = default;
				if (MVGameController.WOCM.Pick(ref hit) && (hit.interactionFlags & InteractionFlags.IsTerrain) == 0)
				{
					if (e.SingleSelectedWO.Id != hit.woId)
					{
						MVGUIDialogBoxWrapper.Instance.ShowSettingsDialog(MVGameController.WOCM.GetWorldObjectClient(hit.woId));
						e.SelectWO(hit.woId, addToSelection: false);
					}
					else
					{
						MVGUIDialogBoxWrapper.Instance.ShowSettingsDialog(MVGameController.WOCM.GetWorldObjectClient(e.SingleSelectedWO.Id));
					}
				}
			}
			else
			{
				e.Event = EditorEvent.ObjectSelected;
			}
		}
		else if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelectAlt) && Vector3.SqrMagnitude(MVInputWrapper.GetPointerPosition() - downPosition) > 0.64f)
		{
			e.Event = EditorEvent.ObjectSelected;
		}
	}

	public override void Exit(EditorStateMachine esm)
	{
	}
}
