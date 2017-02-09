using UnityEngine;

public class DesktopFirstPersonCamera : FirstPersonCamera
{
	protected override void UpdateCameraRotation()
	{
		Vector3 vector = new Vector3(0f - MVInputWrapper.GetAxisRaw("Mouse Y"), MVInputWrapper.GetAxisRaw("Mouse X"));
		targetRotation.x += vector.x * pitchSensitivity;
		targetRotation.y += vector.y * yawSensitivity;
		targetRotation.x = MathFunctions.NormalizeAngle(targetRotation.x);
		targetRotation.y = MathFunctions.NormalizeAngle(targetRotation.y);
		if (targetRotation.x > 180f)
		{
			targetRotation.x -= 360f;
		}
		targetRotation.x = Mathf.Clamp(targetRotation.x, 0f - maxLookAngleDownward, maxLookAngleUpward);
		smoothRotation.SetTargetRotation(targetRotation.x, targetRotation.y);
		transform.rotation = smoothRotation.GetLerpRotation(transform.rotation);
	}
}
