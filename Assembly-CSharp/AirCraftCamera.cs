using System.Collections.Generic;
using UnityEngine;

public class AirCraftCamera : MVPlaymodeCameraBase
{
	public Transform lookAt;

	private float baseDistanceFromLookAt;

	private Vector3 initialLocalCamPosition = Vector3.zero;

	private Vector3 lookAtToCamDir = Vector3.zero;

	public AirCraftCamera()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Enter(MVCameraController camController)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		base.Enter(camController);
		lookAt.localRotation = Quaternion.identity;
		initialLocalCamPosition = ((Component)lookAt).transform.InverseTransformPoint(((Component)this).transform.position);
		Vector3 val = lookAt.position - ((Component)this).transform.position;
		baseDistanceFromLookAt = val.magnitude;
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		lookAtToCamDir = ((Component)lookAt).transform.TransformPoint(initialLocalCamPosition) - ((Component)lookAt).transform.position;
		lookAtToCamDir.Normalize();
		UpdateCameraPosition();
		CameraCollision();
		base.UpdateCamera(camController, targetTransform);
	}

	private void CameraCollision()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)lookAt).transform.position;
		Vector3 val = position - ((Component)this).transform.position;
		float magnitude = val.magnitude;
		Ray ray = new Ray(((Component)lookAt).transform.position, lookAtToCamDir);
		int num = LayerMask.NameToLayer("Default");
		int layerMask = 1 << num;
		if (CollisionDetection.MVSphereCast(ray, cameraRadius, out var voxelHit, magnitude, new HashSet<int>(), layerMask))
		{
			Vector3 intersection = default;
			float distance = 0f;
			if (!MathFunctions.DistancePointLine(voxelHit.point, position, ((Component)this).transform.position + lookAtToCamDir, ref distance, ref intersection))
			{
				Debug.Log((object)"Not within line segment");
				Debug.Log((object)voxelHit.distance);
			}
			((Component)this).transform.position = intersection;
		}
	}

	private void UpdateCameraPosition()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = lookAt.position + lookAtToCamDir * baseDistanceFromLookAt;
	}
}
