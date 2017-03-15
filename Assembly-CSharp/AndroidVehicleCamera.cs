using System.Collections.Generic;
using UnityEngine;

public class AndroidVehicleCamera : MVCameraBase, IVehicleCamera
{
	private Transform originalTransformParent;

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

	[SerializeField]
	private Vector3 lookAtOffset = Vector3.down;

	private float rotationX;

	public float RotationAroundY
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public override CameraType CameraType => CameraType.VehicleCamera;

	public override void Enter(MVCameraController camController)
	{
		originalTransformParent = transform.parent;
		transform.parent = null;
		ignoreAvatarId = new HashSet<int> { MVGameControllerBase.WOCM.AvatarLocal.Id };
		targetRotation.SetTargetRotation(initialYRotation, 0f);
		Quaternion rotation = transform.rotation;
		rotation.eulerAngles = targetRotation.EulerAngles;
		rotationX = initialYRotation;
		transform.rotation = rotation;
	}

	public override void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
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
		transform.position = lookAtTransform.position + vector + lookAtOffset;
	}

	private void UpdateTargetRotation()
	{
		float value = MVInputWrapper.GetAxis("Mouse X") * 90f;
		value = Mathf.Clamp(value, -90f, 90f);
		Quaternion quaternion = Quaternion.AngleAxis(value, Vector3.up);
		Quaternion quaternion2 = Quaternion.Euler(0f, lookAtTransform.rotation.eulerAngles.y, 0f);
		float y = (quaternion * quaternion2).eulerAngles.y;
		rotationX -= MVInputWrapper.GetAxis("Mouse Y") * 5f;
		rotationX = MathFunctions.NormalizeAngle(rotationX);
		if (rotationX > 180f)
		{
			rotationX -= 360f;
		}
		rotationX = Mathf.Clamp(rotationX, minimumY, maximumY);
		targetRotation.SetTargetRotation(rotationX, y);
		transform.rotation = targetRotation.GetLerpRotation(transform.rotation);
	}
}
