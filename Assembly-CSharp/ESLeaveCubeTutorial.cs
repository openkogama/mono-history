public class ESLeaveCubeTutorial : ESStateBase
{
	public override void Enter(EditorStateMachine e)
	{
		base.Enter(e);
		e.Event = EditorEvent.ESTerrainEdit;
		e.MainCameraManager.BlueModeEnabled = false;
		e.CubeModelingStateMachine.Event = CubeModelingEvent.EditCubes;
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.EnterBuildStateEvent(stateType, null);
	}
}
