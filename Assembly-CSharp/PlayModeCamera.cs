using System.Collections.Generic;
using UnityEngine;

public class PlayModeCamera : MVCameraBase
{
	public float moveSpeed = 5f;

	private Quaternion targetRot;

	public float sensitivityX = 15f;

	public float sensitivityY = 15f;

	public float aroundXInertia = 0.5f;

	public float aroundYInertiaMouseControlled = 0.5f;

	private float aroundYInertia;

	public float aroundYInertiaAvatarControlled = 1f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	public float targetDistanceStrength = 10f;

	public float cameraRadius = 0.1f;

	private float distance = 2f;

	public float targetDistance = 2f;

	private Vector3 lookAtPos = Vector3.zero;

	private Transform lookAtTransform;

	private Vector3 lookAtOffset;

	private bool autoRotate;

	private HashSet<int> ignoreAvatarId;

	public PlayModeCamera()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Quaternion rotation = ((Component)this).transform.rotation;
		Quaternion val = MathFunctions.InertiaX(rotation.eulerAngles, targetRot.eulerAngles, aroundXInertia);
		Quaternion rotation2 = ((Component)this).transform.rotation;
		Quaternion val2 = MathFunctions.InertiaY(rotation2.eulerAngles, targetRot.eulerAngles, aroundYInertia);
		((Component)this).transform.rotation = val2 * val;
		distance = Mathf.Lerp(distance, targetDistance, targetDistanceStrength * Time.deltaTime);
		UpdatePosition();
		CameraCollision();
		base.UpdateCamera(camController, targetTransform);
	}

	public override void HandleInput()
	{
		UpdateTargetRotation();
	}

	private void SetLookAtTransform(Transform transform, Vector3 offset)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		lookAtTransform = transform;
		lookAtOffset = offset;
	}

	public override void Enter(MVCameraController camController)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		SetLookAtTransform(MVGameController.Instance.WOCM.WoAvatar.GameObject.transform, 2f * Vector3.up);
		ignoreAvatarId = new HashSet<int> { MVGameController.Instance.WOCM.WoAvatar.Id };
	}

	public void Respawn(MVCameraController camController)
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
		((Component)this).transform.rotation = lookAtTransform.rotation;
		targetRot = lookAtTransform.rotation;
	}

	private void CameraCollision()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.position - lookAtPos;
		Vector3 normalized = val.normalized;
		Ray ray = new Ray(lookAtPos, normalized);
		int num = LayerMask.NameToLayer("Default");
		int layerMask = 1 << num;
		if (CollisionDetection.MVSphereCast(ray, cameraRadius, out var voxelHit, distance, ignoreAvatarId, layerMask))
		{
			Vector3 intersection = default;
			float num2 = 0f;
			if (!MathFunctions.DistancePointLine(voxelHit.point, lookAtPos, ((Component)this).transform.position + normalized, ref num2, ref intersection))
			{
				Debug.Log((object)"Not within line segment");
				Debug.Log((object)voxelHit.distance);
				Debug.Log((object)distance);
			}
			float num3 = Mathf.Sqrt(cameraRadius * cameraRadius - num2 * num2);
			((Component)this).transform.position = intersection - ray.direction * num3;
			Vector3 val2 = lookAtPos - ((Component)this).transform.position;
			distance = val2.magnitude;
		}
	}

	private void UpdatePosition()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		lookAtPos = lookAtTransform.position + lookAtOffset;
		((Component)this).transform.position = lookAtPos;
		((Component)this).transform.position = ((Component)this).transform.position - ((Component)this).transform.forward * distance;
	}

	private void UpdateTargetRotation()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		Vector3 eulerAngles = targetRot.eulerAngles;
		float num = 0f - eulerAngles.x;
		float num2 = eulerAngles.y;
		if (Input.GetKeyDown((KeyCode)324))
		{
			autoRotate = !autoRotate;
			Screen.lockCursor = autoRotate;
		}
		if ((autoRotate || Input.GetKey((KeyCode)120)) && (ignoreInputTypes & IgnoreInputTypes.MouseMovement) == 0)
		{
			num2 += MVInputWrapper.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
			num += MVInputWrapper.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;
			aroundYInertia = aroundYInertiaMouseControlled;
		}
		num = ((!(num < -180f)) ? Mathf.Clamp(num, 0f - maximumY, 1000f) : Mathf.Clamp(num, -1000f, -360f - minimumY));
		eulerAngles = new Vector3(0f - num, num2, 0f);
		targetRot = Quaternion.Euler(eulerAngles);
	}
}
