using UnityEngine;

public class AvatarEditModeCamera : JetPackCamera
{
	public override CameraType CameraType => CameraType.AvatarEditModeCamera;

	public void FocusOnPosition(Vector3 lookAtPosition)
	{
		Transform transform = MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform;
		base.transform.position = transform.position + lookAtOffset;
		base.transform.LookAt(lookAtPosition);
		xAxis = (xAxisTarget = JetPackCamera.NormalizeAngle(base.transform.eulerAngles.x));
		yAxis = (yAxisTarget = base.transform.eulerAngles.y);
		xAxisVelocity = (yAxisVelocity = 0f);
		MVGameControllerBase.CameraController.StartTransitionCam(2f, soft: true);
	}
}
