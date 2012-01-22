using System.Collections.Generic;
using UnityEngine;

public class PlayModeWithInertiaCamera : MVCameraBase
{
	public float mouseSensitivity = 15f;

	public float aroundXInertia = 0.5f;

	public float aroundYInertiaMouseControlled = 0.5f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	public float targetDistanceStrength = 10f;

	public float targetDistance = 2f;

	public float cameraRadius = 0.1f;

	private float distance = 2f;

	private Vector3 lookAtPos = Vector3.zero;

	private Quaternion targetRot;

	private float aroundYInertia;

	private Transform lookAtTransform;

	public Vector3 lookAtOffset = new Vector3(0f, 2.5f, 0f);

	private Vector3 velocity = Vector3.zero;

	private Vector3 currentLookAt = Vector3.zero;

	private Vector3 actualLookAt = Vector3.zero;

	public Vector3 followSpeed = new Vector3(0.05f, 0.2f, 0.05f);

	public float followRotationSpeed = 0.6f;

	public float maximumFollowDistance = 10f;

	public float shakeAmplitudeFactor = 0.12f;

	public float shakeStartingDistanceFactor = 0.04f;

	public float shakeTimeFactor = 6.3f;

	public float shakeStrengthFadeSpeed = 1f;

	private float shakeStrength;

	private float shakeDuration;

	public Vector3 avatarHeadOffset = new Vector3(0f, 1.5f, 0f);

	private bool autoRotate;

	private HashSet<int> ignoreAvatarId;

	public Vector3 FireDirection
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)this).transform.forward;
		}
	}

	public Vector3 FireOrigin
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return actualLookAt;
		}
	}

	public PlayModeWithInertiaCamera()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Enter(MVCameraController camController)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		lookAtTransform = MVGameController.Instance.WOCM.WoAvatar.GameObject.transform;
		currentLookAt = ((Component)camController).transform.position;
		((Component)this).transform.position = ((Component)camController).transform.position;
		((Component)this).transform.rotation = ((Component)camController).transform.rotation;
		targetRot = ((Component)camController).transform.rotation;
		distance = targetDistance;
		ignoreAvatarId = new HashSet<int> { MVGameController.Instance.WOCM.WoAvatar.Id };
	}

	public override void Respawn()
	{
		ResetRotationToTargetTransform();
	}

	public override void Init(MVCameraController camController)
	{
		distance = targetDistance;
		cameraType = CameraType.ThirdPerson;
		aroundYInertia = aroundYInertiaMouseControlled;
		base.Init(camController);
	}

	private void ResetRotationToTargetTransform()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.rotation = lookAtTransform.rotation;
		targetRot = lookAtTransform.rotation;
		currentLookAt = lookAtTransform.position + avatarHeadOffset;
	}

	public override void Shake(float duration, float strength)
	{
		shakeStrength = Mathf.Max(strength, shakeStrength);
		shakeDuration = Mathf.Max(duration, shakeDuration);
	}

	public override void HandleInput()
	{
		UpdateTargetRotation();
	}

	private void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.blue;
		Gizmos.DrawCube(lookAtPos, Vector3.one * 0.2f);
		Gizmos.color = Color.green;
		Gizmos.DrawCube(actualLookAt, Vector3.one * 0.25f);
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		Quaternion rotation = ((Component)this).transform.rotation;
		Quaternion val = MathFunctions.InertiaX(rotation.eulerAngles, targetRot.eulerAngles, aroundXInertia);
		Quaternion rotation2 = ((Component)this).transform.rotation;
		Quaternion val2 = MathFunctions.InertiaY(rotation2.eulerAngles, targetRot.eulerAngles, aroundYInertia);
		((Component)this).transform.rotation = val2 * val;
		distance = Mathf.Lerp(distance, targetDistance, targetDistanceStrength * Time.deltaTime);
		UpdatePosition();
		CameraCollision();
		float num = Mathf.Abs(actualLookAt.y - lookAtPos.y);
		num *= num;
		Vector3 val3 = currentLookAt - ((Component)this).transform.position;
		Vector3 val4 = lookAtPos - ((Component)this).transform.position;
		Quaternion val5 = Quaternion.FromToRotation(val3, val4);
		val5 = Quaternion.Slerp(Quaternion.identity, val5, Time.deltaTime * followRotationSpeed * num);
		targetTransform.position = ((Component)this).transform.position;
		targetTransform.rotation = val5 * ((Component)this).transform.rotation;
	}

	private void CameraCollision()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = lookAtTransform.position + avatarHeadOffset;
		Vector3 val2 = ((Component)this).transform.position - val;
		float magnitude = val2.magnitude;
		val2.Normalize();
		Ray ray = new Ray(val, val2);
		int num = LayerMask.NameToLayer("Default");
		int layerMask = 1 << num;
		if (CollisionDetection.MVSphereCast(ray, cameraRadius, out var voxelHit, magnitude, ignoreAvatarId, layerMask))
		{
			Vector3 intersection = default;
			float num2 = 0f;
			if (!MathFunctions.DistancePointLine(voxelHit.point, val, ((Component)this).transform.position + val2, ref num2, ref intersection))
			{
				Debug.Log((object)"Not within line segment");
				Debug.Log((object)voxelHit.distance);
				Debug.Log((object)distance);
			}
			float num3 = Mathf.Sqrt(cameraRadius * cameraRadius - num2 * num2);
			((Component)this).transform.position = intersection - ray.direction * num3;
			Vector3 val3 = val - ((Component)this).transform.position;
			distance = val3.magnitude;
		}
	}

	private void UpdatePosition()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		Quaternion rotation = ((Component)MVGameController.Instance.WOCM.WoAvatar.Avatar).transform.rotation;
		float num = distance / targetDistance;
		Quaternion identity = Quaternion.identity;
		Quaternion rotation2 = ((Component)this).transform.rotation;
		identity.eulerAngles = new Vector3(0f, rotation2.eulerAngles.y, 0f);
		lookAtPos = lookAtTransform.position + identity * (avatarHeadOffset + lookAtOffset * num);
		float num2 = Mathf.SmoothDamp(currentLookAt.x, lookAtPos.x, ref velocity.x, followSpeed.x);
		float num3 = Mathf.SmoothDamp(currentLookAt.y, lookAtPos.y, ref velocity.y, followSpeed.y);
		float num4 = Mathf.SmoothDamp(currentLookAt.z, lookAtPos.z, ref velocity.z, followSpeed.z);
		currentLookAt = new Vector3(num2, num3, num4);
		Vector3 val = currentLookAt - lookAtPos;
		if (val.sqrMagnitude > maximumFollowDistance * maximumFollowDistance)
		{
			currentLookAt = lookAtPos + val.normalized * maximumFollowDistance;
		}
		Vector3 val2 = lookAtTransform.position + avatarHeadOffset;
		Vector3 val3 = (currentLookAt - val2) * num;
		Vector3 val4 = ((Component)this).transform.position - val2;
		float num5 = distance;
		float magnitude = val3.magnitude;
		float num6 = Vector3.Dot(val3.normalized, val4.normalized);
		float num7 = Mathf.Sqrt(num5 * num5 + magnitude * magnitude - 2f * num5 * magnitude * num6);
		actualLookAt = val2 + val3;
		((Component)this).transform.position = val2 + val3 - ((Component)this).transform.forward * num7;
		float num8 = ComputeShakeFactor(val.magnitude);
		if (num8 > 0f)
		{
			float num9 = MathFunctions.PerlinSimplexNoise.noise(Time.time * shakeTimeFactor, ((Component)this).transform.position.x);
			float num10 = MathFunctions.PerlinSimplexNoise.noise(Time.time * shakeTimeFactor, ((Component)this).transform.position.y);
			Transform transform = ((Component)this).transform;
			transform.position += new Vector3(2f * num9 - 1f, 2f * num10 - 1f, 0f) * num8 * shakeAmplitudeFactor;
		}
		shakeStrength = Mathf.Lerp(shakeStrength, 0f, Time.deltaTime * Time.deltaTime + shakeStrengthFadeSpeed * Time.deltaTime);
		if (shakeDuration > 0f)
		{
			float num11 = MathFunctions.PerlinSimplexNoise.noise(Time.time * shakeTimeFactor, ((Component)this).transform.position.x);
			float num12 = MathFunctions.PerlinSimplexNoise.noise(Time.time * shakeTimeFactor, ((Component)this).transform.position.y);
			Transform transform2 = ((Component)this).transform;
			transform2.position += ((2f * num11 - 1f) * ((Component)this).transform.right + (2f * num12 - 1f) * ((Component)this).transform.up) * shakeStrength * shakeAmplitudeFactor;
			shakeDuration -= Time.deltaTime;
		}
	}

	private float ComputeShakeFactor(float currentDistance)
	{
		float num = maximumFollowDistance - distance;
		float num2 = distance + num * shakeStartingDistanceFactor;
		if (currentDistance < distance)
		{
			return 0f;
		}
		return (currentDistance - num2) / num;
	}

	private void UpdateTargetRotation()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		Vector3 eulerAngles = targetRot.eulerAngles;
		float num = 0f - eulerAngles.x;
		float num2 = eulerAngles.y;
		if (Input.GetKeyDown((KeyCode)324))
		{
			autoRotate = !autoRotate;
		}
		if (Screen.lockCursor != autoRotate)
		{
			Screen.lockCursor = autoRotate;
		}
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
