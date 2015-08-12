using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : PlaymodeCamera
{
	public override CameraType CameraType => CameraType.ThirdPerson;

	public override void HandleInput(MVCameraController cameraController)
	{
		base.HandleInput(cameraController);
		UpdateTargetRotation();
	}

	public override void SetDefaultSettings()
	{
		distanceToAvatar = 5f;
	}

	public override void UpdateFromCameraSettings(Dictionary<object, object> data)
	{
		distanceToAvatar = (float)data["distanceToAvatar"];
	}

	private void UpdateTargetRotation()
	{
		Vector3 eulerAngles = targetRot.EulerAngles;
		float num = 0f - eulerAngles.x;
		float num2 = eulerAngles.y;
		autoRotate = Cursor.lockState == CursorLockMode.Locked;
		if (autoRotate && (ignoreInputTypes & IgnoreInputTypes.MouseMovement) == 0)
		{
			num2 += MVInputWrapper.GetAxis("Mouse X") * mouseSensitivity;
			num += MVInputWrapper.GetAxis("Mouse Y") * mouseSensitivity;
			aroundYInertia = aroundYInertiaMouseControlled;
		}
		num = MathFunctions.NormalizeAngle(num);
		if (num > 180f)
		{
			num -= 360f;
		}
		num = Mathf.Clamp(num, minimumY, maximumY);
		eulerAngles = new Vector3(0f - num, num2, 0f);
		targetRot.SetTargetRotation(eulerAngles.x, eulerAngles.y);
	}
}
