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

	public OrbitCamera()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Enter(MVCameraController camController)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)camController).transform.position;
		((Component)this).transform.position = position;
		desiredPosition = position;
		eulerTargetRot = ComputeEulerRotationToTarget(((Component)camController).transform.position);
		((Component)this).transform.rotation = Quaternion.Euler(eulerTargetRot);
		lookAtPos = MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position;
		UXUtils.FindGUIObjectOfType<MVGUIAskForFocus>().RegainFocus();
	}

	public override void Init(MVCameraController camController)
	{
		base.Init(camController);
	}

	public override void HandleInput(MVCameraController cameraController)
	{
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		if ((ignoreInputTypes & IgnoreInputTypes.MouseMovement) == 0 && Input.GetMouseButton(1))
		{
			ref Vector3 reference = ref eulerTargetRot;
			reference.y += MVInputWrapper.GetAxis("Mouse X") * MouseSensitivity;
			ref Vector3 reference2 = ref eulerTargetRot;
			reference2.x += (0f - MVInputWrapper.GetAxis("Mouse Y")) * MouseSensitivity;
		}
		eulerTargetRot.y %= 360f;
		eulerTargetRot.x %= 360f;
		eulerTargetRot.x = Mathf.Clamp(eulerTargetRot.x, RotX_Min, RotX_Max);
		if (Input.GetAxis("Mouse ScrollWheel") < 0f - scrollDeadZone || Input.GetAxis("Mouse ScrollWheel") > scrollDeadZone)
		{
			desiredDistance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity, DistanceMin, DistanceMax);
		}
		Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, DistanceSmoothTime);
		desiredPosition = lookAtPos + Quaternion.Euler(eulerTargetRot) * Vector3.back * Distance;
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = desiredPosition;
		((Component)this).transform.rotation = Quaternion.Euler(eulerTargetRot);
		targetTransform.position = ((Component)this).transform.position;
		targetTransform.rotation = ((Component)this).transform.rotation;
		MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform.position = ((Component)this).transform.position;
	}

	public override void FocusOnObject(MVWorldObjectClient wo)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		float num = wo.ComputeObjectRadius();
		float num2 = Camera.main.fieldOfView * 0.5f * ((float)Math.PI / 180f);
		float num3 = Mathf.Tan(num2 * 0.5f);
		float num4 = num / num3;
		lookAtPos = SharedCubeFunctions.GetWorldCenter(wo.GameObject.transform);
		Vector3 val = ((Component)this).transform.position - lookAtPos;
		((Component)this).transform.position = lookAtPos + val.normalized * num4;
		eulerTargetRot = ComputeEulerRotationToTarget(((Component)this).transform.position);
		((Component)this).transform.rotation = Quaternion.Euler(eulerTargetRot);
		Distance = (desiredDistance = num4);
		MVAvatarLocal avatarLocal = MVGameController.Instance.WOCM.AvatarLocal;
		avatarLocal.GameObject.transform.position = ((Component)this).transform.position;
		MVGameController.Instance.Game.CameraController.StartTransitionCam(2f, soft: true);
	}

	private Vector3 ComputeEulerRotationToTarget(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = lookAtPos - position;
		Vector3 normalized = val.normalized;
		Vector3 zero = Vector3.zero;
		zero.y = Mathf.Acos(normalized.z) * Mathf.Sign(normalized.x) * 57.29578f;
		zero.x = Mathf.Asin(normalized.y) * Mathf.Sign(normalized.y) * 57.29578f;
		return zero;
	}
}
