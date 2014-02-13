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
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if (MVInputWrapper.GetKeyDown((KeyCode)324))
		{
			downPosition = Input.mousePosition;
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)324))
		{
			if (Vector3.SqrMagnitude(Input.mousePosition - downPosition) < 0.64f)
			{
				VoxelHit hit = default;
				if (MVGameController.Instance.WOCM.Pick(ref hit) && (hit.interactionFlags & InteractionFlags.IsTerrain) == 0)
				{
					if (e.SingleSelectedWO.Id != hit.woId)
					{
						MVGUIDialogBoxWrapper.Instance.ShowSettingsDialog(MVGameController.Instance.WOCM.GetWorldObjectClient(hit.woId));
						e.SelectWO(hit.woId, addToSelection: false);
					}
					else
					{
						MVGUIDialogBoxWrapper.Instance.ShowSettingsDialog(MVGameController.Instance.WOCM.GetWorldObjectClient(e.SingleSelectedWO.Id));
					}
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
