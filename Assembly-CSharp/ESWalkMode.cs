using MV.Common;
using UnityEngine;

internal class ESWalkMode : ESStateBase
{
	private LaserPointer laser;

	public override void Enter(EditorStateMachine esm)
	{
		MVCameraController cameraController = MVGameController.Instance.Game.CameraController;
		((Behaviour)((Component)esm.CameraController).GetComponent<GrayscaleEffect>()).enabled = false;
		esm.CameraController.SecondaryCameraActive = false;
		cameraController.StartTransitionCam(1f);
		cameraController.SetCamera(CameraType.ThirdPerson);
		esm.ClearStateStack();
		esm.DeSelectAll();
		esm.ExitGroupToRoot();
		MVEquipable component = MVGameController.Instance.WOCM.AvatarLocal.GameObject.GetComponent<MVEquipable>();
		if ((Object)(object)component != (Object)null)
		{
			component.Unequip();
		}
	}

	public override void Execute(EditorStateMachine e)
	{
		if (!MVGameController.Instance.EditController.PlayInEditor)
		{
			e.Event = EditorEvent.ESTerrainEdit;
		}
	}

	public override void Exit(EditorStateMachine esm)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		MVEquipable component = MVGameController.Instance.WOCM.AvatarLocal.GameObject.GetComponent<MVEquipable>();
		if ((Object)(object)component != (Object)null)
		{
			component.Equip(AvatarItemType.LaserPointer, null);
		}
		MVCameraController cameraController = MVGameController.Instance.Game.CameraController;
		MVAvatarLocal avatarLocal = MVGameController.Instance.WOCM.AvatarLocal;
		JetPackCamera jetPackCamera = Object.FindObjectOfType(typeof(JetPackCamera)) as JetPackCamera;
		avatarLocal.WorldPosition = jetPackCamera.ComputeAvatarPositionFromTransform(((Component)cameraController).transform);
		Vector3 eulerAngles = ((Component)cameraController).transform.eulerAngles;
		eulerAngles.x = 0f;
		avatarLocal.WorldEulerAngles = eulerAngles;
		cameraController.SetCamera(CameraType.JetPackCamera);
	}
}
