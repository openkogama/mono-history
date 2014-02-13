using UnityEngine;

internal class ESWaitForClone : ESStateBase
{
	private Vector3 pos = Vector3.zero;

	private Quaternion rot = Quaternion.identity;

	private bool goToInsert;

	public ESWaitForClone()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Enter(EditorStateMachine e)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (e.SingleSelectedWO != null)
		{
			pos = e.SingleSelectedWO.WorldPosition;
			rot = e.SingleSelectedWO.WorldRotation;
		}
		e.DeSelectAll();
		if (e.Data.ContainsKey("goToInsert"))
		{
			goToInsert = true;
		}
	}

	public override void Execute(EditorStateMachine e)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		base.Execute(e);
		if (e.SingleSelectedWO != null)
		{
			e.SingleSelectedWO.WorldPosition = pos;
			e.SingleSelectedWO.WorldRotation = rot;
			if (goToInsert)
			{
				e.PushState(EditorEvent.ESInsert, EditorEvent.ObjectSelected);
			}
			else
			{
				e.PushState(EditorEvent.ESTranslate, EditorEvent.ObjectSelected);
			}
		}
	}

	public override void Exit(EditorStateMachine e)
	{
	}
}
