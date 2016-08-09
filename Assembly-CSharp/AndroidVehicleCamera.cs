using System.Collections.Generic;
using UnityEngine;

public class AndroidVehicleCamera : MVCameraBase, IVehicleCamera
{
	private Transform originalTransformParent;

	private float rotationAroundY;

	private readonly CameraLerpToDesiredDistance cameraLerpToDesiredDistance = new CameraLerpToDesiredDistance();

	private readonly CameraCollision cameraCollision = new CameraCollision();

	private HashSet<int> ignoreAvatarId;

	[SerializeField]
	private TargetRotation targetRotation;

	[SerializeField]
	private Transform lookAtTransform;

	[SerializeField]
	private float mouseSensitivity;

	[SerializeField]
	private CameraShake cameraShake;

	[SerializeField]
	private float distanceToLookAt;

	[SerializeField]
	private float minimumY;

	[SerializeField]
	private float maximumY;

	[SerializeField]
	private float initialYRotation = 20f;

	[SerializeField]
	private float localPitch = -20f;

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
		rotationAroundY = 0f;
	}

	public override void Enter(MVCameraController camController)
	{
		originalTransformParent = transform.parent;
		transform.parent = null;
		ignoreAvatarId = new HashSet<int> { MVGameControllerBase.WOCM.AvatarLocal.Id };
		targetRotation.SetTargetRotation(initialYRotation, 0f);
		Quaternion rotation = transform.rotation;
		rotation.eulerAngles = targetRotation.EulerAngles;
		transform.rotation = rotation;
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		UpdateTargetRotation();
		HandlePos();
		HandleCollision();
		targetTransform.position = cameraShake.Shake(transform.position, MVGameControllerBase.WOCM.AvatarLocal.Velocity.magnitude);
		targetTransform.rotation = transform.rotation * Quaternion.Euler(localPitch, 0f, 0f);
	}

	public override void Exit(MVCameraController camController)
	{
		transform.parent = originalTransformParent;
	}

	private void HandleCollision()
	{
		Vector3 position = lookAtTransform.position;
		if (cameraCollision.Collide(out var newPos, cameraRadius, distanceToLookAt, position, transform.position, ignoreAvatarId))
		{
			transform.position = newPos;
		}
		transform.position = cameraLerpToDesiredDistance.Update(position, transform.position);
	}

	private void HandlePos()
	{
		Vector3 vector = transform.rotation * -Vector3.forward;
		vector.Normalize();
		vector *= distanceToLookAt;
		transform.position = lookAtTransform.position + vector;
	}

	private void UpdateTargetRotation()
	{
		float num = targetRotation.EulerAngles.x;
		float y = targetRotation.EulerAngles.y;
		if (InputActive)
		{
			rotationAroundY += MVInputWrapper.GetAxis("Mouse X") * mouseSensitivity;
			rotationAroundY = Mathf.Clamp(rotationAroundY, -90f, 90f);
			Quaternion quaternion = Quaternion.Euler(0f, lookAtTransform.rotation.eulerAngles.y, 0f);
			Quaternion quaternion2 = Quaternion.AngleAxis(rotationAroundY, Vector3.up);
			y = (quaternion2 * quaternion).eulerAngles.y;
			num += MVInputWrapper.GetAxis("Mouse Y") * mouseSensitivity;
		}
		num = MathFunctions.NormalizeAngle(num);
		if (num > 180f)
		{
			num -= 360f;
		}
		num = Mathf.Clamp(num, minimumY, maximumY);
		targetRotation.SetTargetRotation(num, y);
		transform.rotation = targetRotation.GetLerpRotation(transform.rotation);
	}
}
