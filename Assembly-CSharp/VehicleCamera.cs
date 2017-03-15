using UnityEngine;

public class VehicleCamera : PlaymodeCamera, IVehicleCamera
{
	private float rotationAroundY;

	private Transform originalTransformParent;

	[SerializeField]
	private Transform LookAtTransform;

	public float RotationAroundY
	{
		get
		{
			return rotationAroundY;
		}
		set
		{
			rotationAroundY = value;
		}
	}

	public override CameraType CameraType => CameraType.VehicleCamera;

	public override void Reset()
	{
		base.Reset();
		rotationAroundY = 0f;
	}

	public override void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
	{
		UpdateTargetRotation();
		HandleGunMode();
		base.UpdateCamera(camController, targetTransform);
	}

	public override void Enter(MVCameraController cameraController)
	{
		originalTransformParent = transform.parent;
		transform.parent = null;
		base.Enter(cameraController);
		lookAtTransform = LookAtTransform;
	}

	public override void Exit(MVCameraController camController)
	{
		base.Exit(camController);
		Debug.Log("VehicleCamera exit");
		transform.parent = originalTransformParent;
	}

	private void UpdateTargetRotation()
	{
		Vector3 eulerAngles = targetRot.EulerAngles;
		float num = 0f - eulerAngles.x;
		float y = eulerAngles.y;
		autoRotate = !MVGameControllerBase.IPlayModeUI.InLobbyState;
		if (autoRotate && (ignoreInputTypes & IgnoreInputTypes.MouseMovement) == 0)
		{
			rotationAroundY += MVInputWrapper.GetAxis("Mouse X") * mouseSensitivity;
			rotationAroundY = Mathf.Clamp(rotationAroundY, -90f, 90f);
			Quaternion quaternion = Quaternion.Euler(0f, lookAtTransform.rotation.eulerAngles.y, 0f);
			Quaternion quaternion2 = Quaternion.AngleAxis(rotationAroundY, Vector3.up);
			y = (quaternion2 * quaternion).eulerAngles.y;
			num += MVInputWrapper.GetAxis("Mouse Y") * mouseSensitivity;
			aroundYInertia = aroundYInertiaMouseControlled;
		}
		num = MathFunctions.NormalizeAngle(num);
		if (num > 180f)
		{
			num -= 360f;
		}
		num = Mathf.Clamp(num, minimumY, maximumY);
		eulerAngles = new Vector3(0f - num, y, 0f);
		targetRot.SetTargetRotation(eulerAngles.x, eulerAngles.y);
	}
}
