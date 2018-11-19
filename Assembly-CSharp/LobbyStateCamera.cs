using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class LobbyStateCamera : MVCameraBase
{
	[SerializeField]
	private Vector3 offset;

	[SerializeField]
	private float height = 1f;

	private HashSet<int> ignoreAvatarId;

	public override CameraType CameraType => CameraType.LobbyState;

	public override void Enter(MVCameraController camController)
	{
		ignoreAvatarId = new HashSet<int> { MVGameControllerBase.WOCM.AvatarLocal.Id };
		camController.StartTransitionCam(0.5f);
	}

	public void SetRotation(Quaternion rotation)
	{
		transform.rotation = rotation;
	}

	public override void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
	{
		Vector3 lookAtPosition = GetLookAtPosition();
		transform.position = lookAtPosition + transform.rotation * offset;
		if (!MVGameControllerBase.WOCM.AvatarLocal.IsInMode(AvatarModeTypes.Hidden))
		{
			transform.position = PositionAfterCollision(transform.position, lookAtPosition);
		}
		base.UpdateCamera(camController, targetTransform);
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

	private Vector3 GetLookAtPosition()
	{
		return MVGameControllerBase.WOCM.AvatarLocal.Transform.position + new Vector3(0f, height, 0f);
	}

	public override void Exit(MVCameraController camController)
	{
		camController.StartTransitionCam(0.5f);
	}

	public override void Reset()
	{
		transform.rotation = Quaternion.identity;
	}
}
