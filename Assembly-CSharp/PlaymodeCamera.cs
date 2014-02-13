using System.Collections.Generic;
using UnityEngine;

public class PlaymodeCamera : MVPlaymodeCameraBase
{
	public float mouseSensitivity = 5f;

	public float aroundYInertiaMouseControlled = 0.5f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	public Vector3 shoulderOffset = new Vector3(1.5f, 0f, -0.2f);

	public Vector3 followSpeed = new Vector3(0.02f, 0.15f, 0.02f);

	public Vector3 avatarHeadOffset = new Vector3(0f, 1.5f, 0f);

	public float aroundXInertia = 7f;

	public float targetDistanceStrength = 2f;

	public float targetDistance = 5f;

	public float followRotationSpeed = 2f;

	public Vector3 lookAtOffset = new Vector3(0f, 2.5f, 0f);

	protected bool autoRotate;

	protected Vector3 currentLookAt = Vector3.zero;

	protected Vector3 actualLookAt = Vector3.zero;

	protected float distance = 2f;

	protected HashSet<int> ignoreAvatarId;

	protected Transform lookAtTransform;

	protected Quaternion targetRot;

	protected float aroundYInertia;

	protected Vector3 lookAtPos = Vector3.zero;

	public float maximumFollowDistance = 7f;

	public PlaymodeCamera()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Init(MVCameraController camController)
	{
		base.Init(camController);
		distance = targetDistance;
		aroundYInertia = aroundYInertiaMouseControlled;
	}

	public override void Enter(MVCameraController cameraController)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		base.Enter(cameraController);
		lookAtTransform = MVGameController.Instance.WOCM.AvatarLocal.GameObject.transform;
		currentLookAt = ((Component)cameraController).transform.position;
		((Component)this).transform.position = ((Component)cameraController).transform.position;
		((Component)this).transform.rotation = ((Component)cameraController).transform.rotation;
		targetRot = ((Component)cameraController).transform.rotation;
		distance = targetDistance;
		ignoreAvatarId = new HashSet<int> { MVGameController.Instance.WOCM.AvatarLocal.Id };
		AvatarCameraFade component = ((Component)cameraController).gameObject.GetComponent<AvatarCameraFade>();
		if ((Object)(object)component != (Object)null)
		{
			((Behaviour)component).enabled = true;
		}
	}

	public override void Respawn()
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
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
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
		targetTransform.position = ((Component)this).transform.position + shakeOffset;
		targetTransform.rotation = val5 * ((Component)this).transform.rotation;
	}

	private void UpdatePosition()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		float num = distance / targetDistance;
		Quaternion identity = Quaternion.identity;
		Quaternion rotation = ((Component)this).transform.rotation;
		identity.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
		lookAtPos = lookAtTransform.position + identity * (avatarHeadOffset + lookAtOffset * num);
		float num2 = Vector3.Distance(currentLookAt, lookAtPos);
		float num3 = Mathf.Max(15f, num2 * 4f);
		num3 = 15f;
		currentLookAt = Vector3.Lerp(currentLookAt, lookAtPos, num3 * Time.deltaTime);
		Vector3 val = currentLookAt - lookAtPos;
		if (val.sqrMagnitude > maximumFollowDistance * maximumFollowDistance)
		{
			currentLookAt = lookAtPos + val.normalized * maximumFollowDistance;
		}
		Vector3 val2 = lookAtTransform.position + avatarHeadOffset;
		Vector3 val3 = (currentLookAt - val2) * num;
		Vector3 val4 = ((Component)this).transform.position - val2;
		float num4 = distance;
		float magnitude = val3.magnitude;
		float num5 = Vector3.Dot(val3.normalized, val4.normalized);
		float num6 = Mathf.Sqrt(num4 * num4 + magnitude * magnitude - 2f * num4 * magnitude * num5);
		actualLookAt = val2 + val3;
		currentLookAt = actualLookAt;
		((Component)this).transform.position = actualLookAt - ((Component)this).transform.forward * num6;
		Vector3 velocity = MVGameController.Instance.WOCM.AvatarLocal.RigidBody.Velocity;
		float magnitude2 = velocity.magnitude;
		Shake(magnitude2);
	}

	protected void CameraCollision()
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
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
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
			float num4 = distance;
			((Component)this).transform.position = intersection - ray.direction * num3;
			Vector3 val3 = val - ((Component)this).transform.position;
			distance = val3.magnitude;
			float num5 = distance / num4;
			Vector3 val4 = currentLookAt - val;
			currentLookAt = val + num5 * val4;
			actualLookAt = currentLookAt;
		}
	}
}
