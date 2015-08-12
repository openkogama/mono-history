using System.Collections.Generic;
using UnityEngine;

public class AirCraftCamera : MVPlaymodeCameraBase
{
	public Transform lookAt;

	private float baseDistanceFromLookAt;

	private Vector3 initialLocalCamPosition = Vector3.zero;

	private Vector3 lookAtToCamDir = Vector3.zero;

	public override void Enter(MVCameraController camController)
	{
		base.Enter(camController);
		lookAt.localRotation = Quaternion.identity;
		initialLocalCamPosition = lookAt.transform.InverseTransformPoint(transform.position);
		baseDistanceFromLookAt = (lookAt.position - transform.position).magnitude;
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		lookAtToCamDir = lookAt.transform.TransformPoint(initialLocalCamPosition) - lookAt.transform.position;
		lookAtToCamDir.Normalize();
		UpdateCameraPosition();
		CameraCollision();
		base.UpdateCamera(camController, targetTransform);
	}

	private void CameraCollision()
	{
		Vector3 position = lookAt.transform.position;
		float magnitude = (position - transform.position).magnitude;
		Ray ray = new Ray(lookAt.transform.position, lookAtToCamDir);
		int num = LayerMask.NameToLayer("Default");
		int layerMask = 1 << num;
		if (CollisionDetection.MVSphereCast(ray, cameraRadius, out var voxelHit, magnitude, new HashSet<int>(), layerMask))
		{
			Vector3 intersection = default;
			float distance = 0f;
			if (!MathFunctions.DistancePointLine(voxelHit.point, position, transform.position + lookAtToCamDir, ref distance, ref intersection))
			{
				Debug.Log("Not within line segment");
				Debug.Log(voxelHit.distance);
			}
			transform.position = intersection;
		}
	}

	private void UpdateCameraPosition()
	{
		transform.position = lookAt.position + lookAtToCamDir * baseDistanceFromLookAt;
	}
}
