using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class TimeAttackFlagDebriefingCamera : MVCameraBase
{
	[SerializeField]
	private Vector3 offset;

	[SerializeField]
	private float height = 1f;

	private Transform flagTransform;

	private HashSet<int> ignoreAvatarId;

	private MVAvatarLocal avatarLocal;

	public override CameraType CameraType => CameraType.TimeAttackFlagDebriefingCamera;

	public void Initialize(MVAvatarLocal avatarLocal)
	{
		this.avatarLocal = avatarLocal;
	}

	public override void Enter(MVCameraController camController)
	{
		ignoreAvatarId = new HashSet<int> { avatarLocal.Id };
		MVGameControllerBase.MainCameraManager.StartTransitionCam(0.5f);
		flagTransform = GetClosestTimeAttackFlag();
	}

	public override void Exit(MVCameraController camController)
	{
		MVGameControllerBase.MainCameraManager.StartTransitionCam(0.5f);
	}

	public override void Reset()
	{
		transform.rotation = Quaternion.identity;
	}

	public override void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
	{
		Vector3 lookAtPosition = GetLookAtPosition();
		transform.position = lookAtPosition + transform.rotation * offset;
		if (!avatarLocal.IsInMode(SpawnRoleModeType.Hidden))
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
		return flagTransform.position + new Vector3(0f, height, 0f);
	}

	private Transform GetClosestTimeAttackFlag()
	{
		Transform result = null;
		float num = float.MaxValue;
		List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.WOCM.GetWorldObjectsByType(WorldObjectType.TimeAttackFlag);
		if (worldObjectsByType.Count == 0)
		{
			throw new Exception("Entered TimeAttackFlagDebriefingCamera without there being a timeAttackFlag in the game!");
		}
		Vector3 position = avatarLocal.Position;
		for (int i = 0; i < worldObjectsByType.Count; i++)
		{
			float sqrMagnitude = (position - worldObjectsByType[i].Position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = worldObjectsByType[i].Transform;
			}
		}
		return result;
	}
}
