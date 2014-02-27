using UnityEngine;

public class ThirdPersonCamera : PlaymodeCamera
{
	public override void HandleInput(MVCameraController cameraController)
	{
		base.HandleInput(cameraController);
		UpdateTargetRotation();
	}

	private void OnDrawGizmos()
	{
	}

	private void UpdateTargetRotation()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		Vector3 eulerAngles = targetRot.eulerAngles;
		float num = 0f - eulerAngles.x;
		float num2 = eulerAngles.y;
		autoRotate = Screen.lockCursor;
		if (autoRotate && (ignoreInputTypes & IgnoreInputTypes.MouseMovement) == 0)
		{
			num2 += MVInputWrapper.GetAxis("Mouse X") * mouseSensitivity;
			num += MVInputWrapper.GetAxis("Mouse Y") * mouseSensitivity;
			aroundYInertia = aroundYInertiaMouseControlled;
		}
		num = ((!(num < -180f)) ? Mathf.Clamp(num, 0f - maximumY, 1000f) : Mathf.Clamp(num, -1000f, -360f - minimumY));
		eulerAngles = new Vector3(0f - num, num2, 0f);
		targetRot = Quaternion.Euler(eulerAngles);
	}
}
