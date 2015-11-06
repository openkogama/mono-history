using UnityEngine;

public class EditorCamera2D : MVCameraBase
{
	public override CameraType CameraType => CameraType.EditorCamera2D;

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		transform.rotation = Quaternion.identity;
		transform.position = MVGameControllerBase.WOCM.AvatarLocal.LookAtPos;
		base.UpdateCamera(camController, targetTransform);
	}
}
