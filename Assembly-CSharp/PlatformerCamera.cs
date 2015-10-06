using System.Collections.Generic;
using UnityEngine;

public class PlatformerCamera : PlaymodeCamera
{
	public float speedDistModifier = 8f;

	public float tiltAdjust = 1.8f;

	public float maxCamDist = 60f;

	public bool avatarWorldCollision;

	private float avatarLookAheadAddX;

	private float avatarLookAheadFactor = 0.38f;

	private float avatarLookAheadLerp = 2f;

	private float positionLerpFactor = 1.8f;

	private float rotationLerpFactor = 4.5f;

	private float distLerpFactorIncrease = 0.8f;

	private float distLerpFactorDecrease = 0.2f;

	private float curCamDist;

	private Vector3 lookAtAvatarOffset = new Vector3(0f, 2.5f, 0f);

	public override CameraType CameraType => CameraType.Platformer;

	public override Vector3 FireDirection
	{
		get
		{
			Vector3 forward = GameDB.LocalAvatar.GameObject.transform.forward;
			forward.y = 0f;
			return Vector3.Normalize(forward);
		}
	}

	public override Vector3 FireOrigin => MVGameController.WOCM.AvatarLocal.GameObject.transform.position + lookAtOffset;

	public override void Enter(MVCameraController cameraController)
	{
		base.Enter(cameraController);
		curCamDist = distanceToAvatar;
	}

	public override void UpdateFromCameraSettings(Dictionary<object, object> data)
	{
		height = (float)data["height"];
		distanceToAvatar = (float)data["distanceToAvatar"];
		smoothness = (float)data["smoothness"];
		speedDistModifier = (float)data["speedDistanceModifier"];
		tiltAdjust = (float)data["tiltAdjust"];
		avatarWorldCollision = (bool)data["avatarWorldCollision"];
		lookAtAvatarOffset = new Vector3(0f, tiltAdjust, 0f);
	}

	public override void SetDefaultSettings()
	{
		height = 3f;
		distanceToAvatar = 12f;
		smoothness = 0.54f;
		speedDistModifier = 12f;
		tiltAdjust = 1.8f;
		avatarWorldCollision = false;
		lookAtAvatarOffset = new Vector3(0f, tiltAdjust, 0f);
	}

	public override void ScaleCameraValues(float scale)
	{
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		CalcCamDist();
		float num = (positionLerpFactor + (10f - smoothness * 10f)) * Time.deltaTime;
		float t = (rotationLerpFactor + (10f - smoothness * 10f)) * Time.deltaTime;
		Vector3 position = GameDB.LocalAvatar.GameObject.transform.position;
		avatarLookAheadAddX = Mathf.Lerp(avatarLookAheadAddX, GameDB.LocalAvatar.GetAbsoluteVelocity().x * avatarLookAheadFactor, avatarLookAheadLerp * Time.deltaTime);
		float num2 = curCamDist * (Camera.main.fieldOfView / 90f) + num * Mathf.Abs(avatarLookAheadAddX);
		avatarLookAheadAddX = Mathf.Clamp(avatarLookAheadAddX, 0f - num2, num2);
		position.x += avatarLookAheadAddX;
		Vector3 vector = position + new Vector3(0f, 0f, 0f - distanceToAvatar) + new Vector3(0f, height, 0f);
		Vector3 normalized = (position - vector).normalized;
		Vector3 b = position + normalized * (0f - curCamDist) + new Vector3(0f, height, 0f);
		Vector3 position2 = transform.position;
		position2.x = position.x;
		Quaternion b2 = Quaternion.LookRotation(position - position2, Vector3.up);
		transform.position = Vector3.Lerp(transform.position, b, num);
		transform.rotation = Quaternion.Lerp(transform.rotation, b2, t);
		if (avatarWorldCollision)
		{
			Vector3 avatarPos = GameDB.LocalAvatar.GameObject.transform.position + lookAtAvatarOffset;
			AdjustForCameraCollision(avatarPos);
		}
		targetTransform.position = transform.position;
		targetTransform.rotation = transform.rotation;
	}

	private void CalcCamDist()
	{
		Vector3 absoluteVelocity = GameDB.LocalAvatar.GetAbsoluteVelocity();
		absoluteVelocity.y = 0f;
		float num = distanceToAvatar + absoluteVelocity.magnitude * speedDistModifier * Time.deltaTime;
		if (num > curCamDist)
		{
			curCamDist = Mathf.Lerp(curCamDist, num, (distLerpFactorIncrease + (4f - smoothness * 4f)) * Time.deltaTime);
		}
		else
		{
			curCamDist = Mathf.Lerp(curCamDist, num, (distLerpFactorDecrease + (2f - smoothness * 2f)) * Time.deltaTime);
		}
		curCamDist = Mathf.Clamp(curCamDist, distanceToAvatar, maxCamDist);
	}

	public override void Respawn()
	{
	}

	private void AdjustForCameraCollision(Vector3 avatarPos)
	{
		Vector3 vector = transform.position - avatarPos;
		float magnitude = vector.magnitude;
		vector.Normalize();
		Ray ray = new Ray(avatarPos, vector);
		int num = LayerMask.NameToLayer("Default");
		int layerMask = 1 << num;
		bool flag = CollisionDetection.MVSphereCast(ray, cameraRadius, out var voxelHit, magnitude, ignoreAvatarId, layerMask);
		if (!(voxelHit.distance < Mathf.Epsilon) && flag)
		{
			Vector3 intersection = default;
			float num2 = 0f;
			if (!MathFunctions.DistancePointLine(voxelHit.point, avatarPos, transform.position + vector, ref num2, ref intersection))
			{
				Debug.Log("Not within line segment");
				Debug.Log(voxelHit.distance);
				Debug.Log(distance);
			}
			else
			{
				float num3 = Mathf.Sqrt(cameraRadius * cameraRadius - num2 * num2);
				transform.position = intersection - ray.direction * num3;
			}
		}
	}
}
