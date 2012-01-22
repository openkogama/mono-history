using UnityEngine;

internal class ESPlaceEndpoint : ESStateBase
{
	public override void Enter(EditorStateMachine esm)
	{
		Debug.Log((object)"ESPlaceEndpoint state ENTER");
	}

	public override void Execute(EditorStateMachine e)
	{
	}

	public override void Exit(EditorStateMachine esm)
	{
	}
}
