using UnityEngine;

internal class ESWaitForClone : ESStateBase
{
	private Vector3 pos = Vector3.zero;

	private Quaternion rot = Quaternion.identity;

	private bool goToInsert;

	public override void Enter(EditorStateMachine e)
	{
		if (e.Data.ContainsKey("goToInsert"))
		{
			goToInsert = true;
		}
		else
		{
			goToInsert = false;
		}
		if (e.SingleSelectedWO != null && !goToInsert)
		{
			pos = e.SingleSelectedWO.WorldPosition;
			rot = e.SingleSelectedWO.WorldRotation;
		}
		e.DeSelectAll();
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (e.SingleSelectedWO != null)
		{
			e.SingleSelectedWO.WorldPosition = pos;
			e.SingleSelectedWO.WorldRotation = rot;
			if (goToInsert)
			{
				e.PushState(EditorEvent.ESInsert, EditorEvent.ObjectSelected);
				return;
			}
			e.Data.Add("translateMode", TranslateMode.XZ);
			e.Data.Add("moveWithAvatar", true);
			e.PushState(EditorEvent.ESTranslate, EditorEvent.ObjectSelected);
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		pos = Vector3.zero;
		rot = Quaternion.identity;
	}
}
