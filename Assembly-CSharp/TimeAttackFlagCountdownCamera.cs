using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class TimeAttackFlagCountdownCamera : MVCameraBase
{
	[SerializeField]
	private Vector3 offset;

	private HashSet<int> ignoreAvatarId;

	public override CameraType CameraType => CameraType.TimeAttackFlagCountdownCamera;

	public override void Enter(MVCameraController camController)
	{
		ignoreAvatarId = new HashSet<int> { MVGameControllerBase.WOCM.AvatarLocal.Id };
		camController.StartTransitionCam(0.5f);
		base.Enter(camController);
	}

	public override void Exit(MVCameraController camController)
	{
		camController.StartTransitionCam(0.5f);
	}

	public override void Reset()
	{
		transform.rotation = Quaternion.identity;
	}

	public override void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
	{
		base.UpdateCamera(camController, targetTransform);
		Vector3 position = MVGameControllerBase.WOCM.AvatarLocal.Position;
		transform.position = position + transform.rotation * offset;
		if (!MVGameControllerBase.WOCM.AvatarLocal.IsInMode(AvatarModeTypes.Hidden))
		{
			transform.position = PositionAfterCollision(transform.position, position);
		}
	}

	private Vector3 PositionAfterCollision(Vector3 desiredPosition, Vector3 moveToPosition)
	{
		Vector3 vector = desiredPosition - moveToPosition;
		float magnitude = vector.magnitude;
		vector.Normalize();
		Ray ray = new Ray(moveToPosition, vector);
		int num = LayerMask.NameToLayer("Default");
		int layerMask = 1 << num;
		bool flag = CollisionDetection.MVSphereCast(ray, cameraRadius, out var voxelHit, magnitude, ignoreAvatarId, layerMask);
		if (voxelHit.distance < Mathf.Epsilon)
		{
			return desiredPosition;
		}
		if (flag)
		{
			Vector3 intersection = default;
			float distance = 0f;
			if (!MathFunctions.DistancePointLine(voxelHit.point, moveToPosition, transform.position + vector, ref distance, ref intersection))
			{
				Debug.Log("Not within line segment");
				Debug.Log(voxelHit.distance);
			}
			float num2 = Mathf.Sqrt(cameraRadius * cameraRadius - distance * distance);
			return intersection - ray.direction * num2;
		}
		return desiredPosition;
	}
}
