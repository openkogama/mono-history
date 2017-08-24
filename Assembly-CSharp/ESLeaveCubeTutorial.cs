using UnityEngine;

public class ESLeaveCubeTutorial : ESStateBase
{
	public override void Enter(EditorStateMachine e)
	{
		base.Enter(e);
		e.Event = EditorEvent.ESTerrainEdit;
		e.CameraController.BlueModeEnabled = false;
		e.CubeModelingStateMachine.Event = CubeModelingEvent.EditCubes;
		MVGameControllerBase.WOCM.AvatarLocal.SetToSpawnTransform();
		((JetPackCamera)MVGameControllerBase.CameraController.CurCamera).FocusOnPosition(MVGameControllerBase.WOCM.AvatarLocal.LookAtPos + Vector3.up + MVGameControllerBase.WOCM.AvatarLocal.Transform.rotation * Vector3.forward, 0f);
	}
}
