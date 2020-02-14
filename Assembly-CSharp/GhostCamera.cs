using System.Collections.Generic;
using UnityEngine;

public class GhostCamera : MVCameraBase
{
	[SerializeField]
	private Vector3 offset;

	[SerializeField]
	private float height = 1f;

	[SerializeField]
	private float desiredDistance = 5f;

	private Vector3 avatarHeadOffset = new Vector3(0f, 1.5f, 0f);

	private MVAvatarLocal avatarLocal;

	private HashSet<int> ignoreAvatarId;

	protected AvatarCameraDistTransparency avatarCameraDistTransparency;

	protected Transform lookAtTransform;

	private float distance = 5f;

	private Vector3 currentLookAt;

	protected Vector3 lookAtPos = Vector3.zero;

	private Vector3 actualLookAt = Vector3.zero;

	private Vector3 prevLookAtTransformPos = default;

	public override CameraType CameraType => CameraType.GhostCamera;

	public void Initialize(MVAvatarLocal avatarLocal)
	{
		this.avatarLocal = avatarLocal;
		lookAtTransform = avatarLocal.GameObject.transform;
		distance = (transform.position - lookAtTransform.position).magnitude;
	}

	public override void Enter(MVCameraController camController)
	{
		ignoreAvatarId = new HashSet<int> { avatarLocal.Id };
		currentLookAt = lookAtTransform.position + avatarHeadOffset;
		Vector3 lookAtPosition = GetLookAtPosition();
		transform.position = lookAtPosition + transform.rotation * offset;
		if (MVClientSettings.ReviveEnabled && MVGameControllerBase.SpawnRoleDataMediatorLocal.LastAvatarRespawnType.Value == LastRespawnType.Revive)
		{
			SafeSpotData safeGroundedDataAtSelectedIndex = MVGameControllerBase.SpawnRoleDataMediatorLocal.ReviveState.Value.GetSafeGroundedDataAtSelectedIndex();
			transform.position = safeGroundedDataAtSelectedIndex.CameraPosition;
			transform.rotation = safeGroundedDataAtSelectedIndex.CameraRotation;
		}
	}

	public override void Exit(MVCameraController camController)
	{
		base.Exit(camController);
		MVGameControllerBase.MainCameraManager.StartTransitionCam(0.5f);
	}

	public override void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
	{
		distance = Mathf.Lerp(distance, desiredDistance, 2f * Time.deltaTime);
		UpdatePosition();
		CameraCollision();
		float num = Mathf.Abs(actualLookAt.y - lookAtPos.y);
		num *= num;
		Vector3 fromDirection = currentLookAt - transform.position;
		Vector3 toDirection = lookAtPos - transform.position;
		Quaternion b = Quaternion.FromToRotation(fromDirection, toDirection);
		b = Quaternion.Slerp(Quaternion.identity, b, Time.deltaTime * 2f * num);
		targetTransform.position = transform.position;
		targetTransform.rotation = b * transform.rotation;
	}

	private void UpdatePosition()
	{
		transform.position += lookAtTransform.position - prevLookAtTransformPos;
		float num = distance / desiredDistance;
		Quaternion identity = Quaternion.identity;
		identity.eulerAngles = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
		lookAtPos = lookAtTransform.position + identity * (avatarHeadOffset + offset * num);
		Vector3 vector = lookAtTransform.position + avatarHeadOffset;
		Vector3 vector2 = (lookAtPos - vector) * num;
		Vector3 vector3 = transform.position - vector;
		float magnitude = vector2.magnitude;
		float num2 = Vector3.Dot(vector2.normalized, vector3.normalized);
		float num3 = Mathf.Sqrt(distance * distance + magnitude * magnitude - 2f * distance * magnitude * num2);
		actualLookAt = vector + vector2;
		currentLookAt = actualLookAt;
		if (num3 > Mathf.Epsilon)
		{
			transform.position = actualLookAt - transform.forward * num3;
		}
		prevLookAtTransformPos = lookAtTransform.position;
	}

	protected virtual void CameraCollision()
	{
		Vector3 vector = lookAtTransform.position + avatarHeadOffset;
		Vector3 vector2 = transform.position - vector;
		float magnitude = vector2.magnitude;
		vector2.Normalize();
		Ray ray = new Ray(vector, vector2);
		int num = LayerMask.NameToLayer("Default");
		int layerMask = 1 << num;
		bool flag = CollisionDetection.MVSphereCast(ray, cameraRadius, out var voxelHit, magnitude, ignoreAvatarId, layerMask);
		if (!(voxelHit.distance < Mathf.Epsilon) && flag)
		{
			Vector3 intersection = default;
			float num2 = 0f;
			if (!MathFunctions.DistancePointLine(voxelHit.point, vector, transform.position + vector2, ref num2, ref intersection))
			{
				Debug.Log("Not within line segment");
				Debug.Log(voxelHit.distance);
				Debug.Log(distance);
			}
			float num3 = 0f;
			if (cameraRadius > num2)
			{
				num3 = Mathf.Sqrt(cameraRadius * cameraRadius - num2 * num2);
			}
			float num4 = distance;
			transform.position = intersection - ray.direction * num3;
			distance = (vector - transform.position).magnitude;
			float num5 = distance / num4;
			Vector3 vector3 = currentLookAt - vector;
			currentLookAt = vector + num5 * vector3;
		}
	}

	private Vector3 GetLookAtPosition()
	{
		return avatarLocal.Transform.position + new Vector3(0f, height, 0f);
	}

	public override void Reset()
	{
		transform.rotation = Quaternion.identity;
	}
}
