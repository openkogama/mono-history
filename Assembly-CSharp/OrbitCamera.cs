using System;
using UnityEngine;

public class OrbitCamera : MVCameraBase
{
	public float MouseSensitivity = 6f;

	public float MouseWheelSensitivity = 5f;

	private Vector3 lookAtPos = Vector3.zero;

	private Vector3 desiredPosition;

	private float desiredDistance = 5f;

	public float RotX_Min = -80f;

	public float RotX_Max = 80f;

	public Vector3 eulerTargetRot = Vector3.zero;

	private float scrollDeadZone = 0.01f;

	public float Distance = 5f;

	public float DistanceMin = 0.5f;

	public float DistanceMax = 10f;

	public float DefaultDistance = 5f;

	public float DistanceSmoothTime = 0.05f;

	private float velDistance;

	public override CameraType CameraType => CameraType.OrbitCamera;

	public override void Enter(MVCameraController camController)
	{
		Vector3 position = camController.transform.position;
		transform.position = position;
		desiredPosition = position;
		eulerTargetRot = ComputeEulerRotationToTarget(camController.transform.position);
		transform.rotation = Quaternion.Euler(eulerTargetRot);
		lookAtPos = MVGameController.WOCM.AvatarLocal.GameObject.transform.position;
	}

	public override void HandleInput(MVCameraController cameraController)
	{
		if ((ignoreInputTypes & IgnoreInputTypes.MouseMovement) == 0 && MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelectAlt))
		{
			eulerTargetRot.y += MVInputWrapper.GetAxis("Mouse X") * MouseSensitivity;
			eulerTargetRot.x += (0f - MVInputWrapper.GetAxis("Mouse Y")) * MouseSensitivity;
		}
		eulerTargetRot.y %= 360f;
		eulerTargetRot.x %= 360f;
		eulerTargetRot.x = Mathf.Clamp(eulerTargetRot.x, RotX_Min, RotX_Max);
		if (MVInputWrapper.GetAxis("Mouse ScrollWheel") < 0f - scrollDeadZone || MVInputWrapper.GetAxis("Mouse ScrollWheel") > scrollDeadZone)
		{
			desiredDistance = Mathf.Clamp(Distance - MVInputWrapper.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity, DistanceMin, DistanceMax);
		}
		Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, DistanceSmoothTime);
		desiredPosition = lookAtPos + Quaternion.Euler(eulerTargetRot) * Vector3.back * Distance;
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		transform.position = desiredPosition;
		transform.rotation = Quaternion.Euler(eulerTargetRot);
		targetTransform.position = transform.position;
		targetTransform.rotation = transform.rotation;
		MVGameController.WOCM.AvatarLocal.GameObject.transform.position = transform.position;
	}

	public override void FocusOnObject(MVWorldObjectClient wo)
	{
		float num = wo.ComputeObjectRadius();
		float num2 = Camera.main.fieldOfView * 0.5f * ((float)Math.PI / 180f);
		float num3 = Mathf.Tan(num2 * 0.5f);
		float num4 = num / num3;
		lookAtPos = SharedCubeFunctions.GetWorldCenter(wo.GameObject.transform);
		Vector3 vector = transform.position - lookAtPos;
		transform.position = lookAtPos + vector.normalized * num4;
		eulerTargetRot = ComputeEulerRotationToTarget(transform.position);
		transform.rotation = Quaternion.Euler(eulerTargetRot);
		Distance = (desiredDistance = num4);
		MVAvatarLocal avatarLocal = MVGameController.WOCM.AvatarLocal;
		avatarLocal.GameObject.transform.position = transform.position;
		MVGameController.Game.CameraController.StartTransitionCam(2f, soft: true);
	}

	private Vector3 ComputeEulerRotationToTarget(Vector3 position)
	{
		Vector3 normalized = (lookAtPos - position).normalized;
		Vector3 zero = Vector3.zero;
		zero.y = Mathf.Acos(normalized.z) * Mathf.Sign(normalized.x) * 57.29578f;
		zero.x = Mathf.Asin(normalized.y) * Mathf.Sign(normalized.y) * 57.29578f;
		return zero;
	}
}
