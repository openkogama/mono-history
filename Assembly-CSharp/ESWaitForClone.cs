using MV.Common;
using UnityEngine;

internal class ESWaitForClone : ESStateBase
{
	private Vector3 pos = Vector3.zero;

	private Quaternion rot = Quaternion.identity;

	private bool goToInsert;

	public override void Enter(EditorStateMachine e)
	{
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
		base.Execute(e);
		if (e.SingleSelectedWO == null)
		{
			return;
		}
		e.SingleSelectedWO.WorldPosition = pos;
		e.SingleSelectedWO.WorldRotation = rot;
		if (goToInsert)
		{
			e.PushState(EditorEvent.ESInsert, EditorEvent.ObjectSelected);
			return;
		}
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			e.Data.Add("translateMode", TranslateMode.XY);
			e.Data.Add("moveWithAvatar", false);
		}
		else
		{
			e.Data.Add("translateMode", TranslateMode.XZ);
			e.Data.Add("moveWithAvatar", true);
		}
		e.PushState(EditorEvent.ESTranslate, EditorEvent.ObjectSelected);
	}

	public override void Exit(EditorStateMachine e)
	{
	}
}
