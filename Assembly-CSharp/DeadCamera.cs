using UnityEngine;

public class DeadCamera : MVCameraBase
{
	[SerializeField]
	private Vector3 avatarLocalLookAtOffset;

	private MVAvatarLocal avatarLocal;

	private Vector3 lookAtPos;

	public override CameraType CameraType => CameraType.DeadCamera;

	public void Initialize(MVAvatarLocal avatarLocal)
	{
		this.avatarLocal = avatarLocal;
	}

	public override void Enter(MVCameraController camController)
	{
		base.Enter(camController);
		transform.position = MVGameControllerBase.MainCameraManager.MainCamera.transform.position;
		MVGameControllerBase.MainCameraManager.StartTransitionCam(0.5f);
	}

	public override void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
	{
		base.UpdateCamera(camController, targetTransform);
		lookAtPos = avatarLocal.Transform.position + avatarLocalLookAtOffset;
		UpdateRotation();
		targetTransform.position = transform.position;
		targetTransform.rotation = transform.rotation;
	}

	private void UpdateRotation()
	{
		Vector3 normalized = (lookAtPos - transform.position).normalized;
		transform.rotation = Quaternion.LookRotation(normalized);
	}
}
