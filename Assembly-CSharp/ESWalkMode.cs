using MV.Common;
using UnityEngine;

internal class ESWalkMode : ESStateBase
{
	private LaserPointer laser;

	public override void Enter(EditorStateMachine esm)
	{
		MVCameraController cameraController = MVGameController.Game.CameraController;
		esm.CameraController.GetComponent<GrayscaleEffect>().enabled = false;
		esm.CameraController.SecondaryCameraActive = false;
		if (GameDB.GameType != MVGameType.Platformer)
		{
			cameraController.StartTransitionCam(1f);
		}
		cameraController.SetPlayModeCam();
		esm.ClearStateStack();
		esm.DeSelectAll();
		esm.ExitGroupToRoot();
		MVEquipable component = MVGameController.WOCM.AvatarLocal.GameObject.GetComponent<MVEquipable>();
		if (component != null)
		{
			component.Unequip();
		}
	}

	public override void Execute(EditorStateMachine e)
	{
		if (!MVGameController.EditorController.PlayInEditor)
		{
			e.Event = EditorEvent.ESTerrainEdit;
		}
	}

	public override void Exit(EditorStateMachine esm)
	{
		MVEquipable component = MVGameController.WOCM.AvatarLocal.GameObject.GetComponent<MVEquipable>();
		if (component != null)
		{
			component.Equip(AvatarItemType.LaserPointer, null);
		}
		MVCameraController cameraController = MVGameController.Game.CameraController;
		MVAvatarLocal avatarLocal = MVGameController.WOCM.AvatarLocal;
		JetPackCamera camera = MVGameController.Game.CameraController.GetCamera<JetPackCamera>();
		avatarLocal.WorldPosition = camera.ComputeAvatarPositionFromTransform(cameraController.transform);
		Vector3 eulerAngles = cameraController.transform.eulerAngles;
		eulerAngles.x = 0f;
		avatarLocal.WorldEulerAngles = eulerAngles;
		cameraController.SetCamera(CameraType.JetPackCamera);
	}
}
