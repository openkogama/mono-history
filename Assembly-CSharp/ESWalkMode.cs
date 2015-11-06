internal class ESWalkMode : ESStateBase
{
	public override void Enter(EditorStateMachine esm)
	{
		esm.ClearStateStack();
		esm.DeSelectAll();
		esm.ExitGroupToRoot();
	}

	public override void Execute(EditorStateMachine e)
	{
		if (!MVGameControllerLegacyUI.EditorController.PlayInEditor)
		{
			e.Event = EditorEvent.ESTerrainEdit;
		}
	}

	public override void Exit(EditorStateMachine esm)
	{
	}
}
