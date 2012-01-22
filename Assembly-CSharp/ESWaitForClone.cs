using UnityEngine;

internal class ESWaitForClone : ESStateBase
{
	private Vector3 pos = Vector3.zero;

	private Quaternion rot = Quaternion.identity;

	public ESWaitForClone()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Enter(EditorStateMachine e)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"ESWaitForClone");
		pos = e.SingleSelectedWO.GameObject.transform.position;
		rot = e.SingleSelectedWO.GameObject.transform.rotation;
		e.DeSelect();
	}

	public override void Execute(EditorStateMachine e)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.Execute(e);
		if (e.SingleSelectedWO != null)
		{
			e.SingleSelectedWO.GameObject.transform.position = pos;
			e.SingleSelectedWO.GameObject.transform.rotation = rot;
			e.PushState(EditorEvent.ESTranslate, EditorEvent.ObjectSelected);
		}
	}

	public override void Exit(EditorStateMachine e)
	{
	}
}
