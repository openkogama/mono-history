using System.Collections.Generic;
using UnityEngine;

public class PlaymodeCamera : MVPlaymodeCameraBase
{
	protected class TargetRotation
	{
		private Vector3 eulerAngles = default;

		private float maxInertialAngleBehind = 70f;

		public Vector3 EulerAngles => eulerAngles;

		public TargetRotation(float pitch, float yaw)
		{
			SetTargetRotation(pitch, yaw);
		}

		public void SetTargetRotation(float pitch, float yaw)
		{
			eulerAngles.x = pitch;
			eulerAngles.y = yaw;
			eulerAngles.z = 0f;
		}

		public void SetTargetRotation(Quaternion q)
		{
			eulerAngles = q.eulerAngles;
			eulerAngles.z = 0f;
		}

		public Quaternion GetRotation()
		{
			return Quaternion.Euler(eulerAngles);
		}

		public Quaternion GetLerpRotation(Quaternion from, float lerpSpeedX, float lerpSpeedY)
		{
			Vector3 vector = from.eulerAngles;
			float x = Mathf.LerpAngle(vector.x, eulerAngles.x, Time.deltaTime * lerpSpeedX);
			vector.y = ClampDegreeDiff(vector.y, eulerAngles.y, maxInertialAngleBehind);
			float y = Mathf.LerpAngle(vector.y, eulerAngles.y, Time.deltaTime * lerpSpeedY);
			return Quaternion.Euler(x, y, 0f);
		}

		private static float ClampDegreeDiff(float target, float to, float maxDiff)
		{
			float num = Mathf.DeltaAngle(target, to);
			float num2 = 0f;
			if (num > maxDiff)
			{
				num2 = num - maxDiff;
			}
			else if (num < 0f - maxDiff)
			{
				num2 = num + maxDiff;
			}
			return target + num2;
		}

		public float Normalize(float degrees)
		{
			degrees %= 360f;
			if (degrees < 0f)
			{
				degrees += 360f;
			}
			return degrees;
		}
	}

	private class SmoothLookAt
	{
		private int samleLength = 5;

		private Queue<Vector3> prevVelocities = new Queue<Vector3>();

		private float maxMag = 30f;

		public Vector3 GetCurrentLookAt(Vector3 velocity)
		{
			while (prevVelocities.Count >= samleLength)
			{
				prevVelocities.Dequeue();
			}
			velocity.x = (velocity.z = 0f);
			prevVelocities.Enqueue(velocity * Time.deltaTime);
			Vector3 result = Vector3.zero;
			foreach (Vector3 prevVelocity in prevVelocities)
			{
				result += prevVelocity;
			}
			result /= (float)prevVelocities.Count;
			if (result.sqrMagnitude > maxMag * maxMag)
			{
				result = result.normalized * maxMag;
			}
			return result;
		}

		public void Clear()
		{
			prevVelocities.Clear();
		}
	}

	public float distanceToAvatar = 5f;

	public float height = 1.5f;

	public float smoothness = 0.54f;

	public float mouseSensitivity = 2.5f;

	public float aroundYInertiaMouseControlled = 0.5f;

	public float minimumY = -60f;

	public float maximumY = 60f;

	public Vector3 shoulderOffset = new Vector3(1.5f, 0f, -0.2f);

	private Vector3 avatarHeadOffset = new Vector3(0f, 1.5f, 0f);

	public float aroundXInertia = 7f;

	public float targetDistanceStrength = 2f;

	public float followRotationSpeed = 2f;

	public Vector3 lookAtOffset = new Vector3(0f, 2.5f, 0f);

	protected bool autoRotate;

	protected Vector3 currentLookAt = Vector3.zero;

	protected Vector3 actualLookAt = Vector3.zero;

	protected float distance = 2f;

	protected HashSet<int> ignoreAvatarId;

	protected Transform lookAtTransform;

	protected TargetRotation targetRot = new TargetRotation(0f, 0f);

	protected float aroundYInertia;

	protected Vector3 lookAtPos = Vector3.zero;

	private SmoothLookAt smoothLookAt = new SmoothLookAt();

	private Vector3 prevLookAtTransformPos = Vector3.zero;

	public virtual void UpdateFromCameraSettings(Dictionary<object, object> data)
	{
	}

	public virtual void SetDefaultSettings()
	{
	}

	private void Awake()
	{
		distance = distanceToAvatar;
		aroundYInertia = aroundYInertiaMouseControlled;
	}

	public override void Enter(MVCameraController cameraController)
	{
		base.Enter(cameraController);
		lookAtTransform = MVGameController.WOCM.AvatarLocal.GameObject.transform;
		currentLookAt = cameraController.transform.position;
		transform.position = cameraController.transform.position;
		transform.rotation = cameraController.transform.rotation;
		prevLookAtTransformPos = lookAtTransform.transform.position;
		targetRot.SetTargetRotation(cameraController.transform.rotation);
		distance = distanceToAvatar;
		ignoreAvatarId = new HashSet<int> { MVGameController.WOCM.AvatarLocal.Id };
		AvatarCameraFade component = cameraController.gameObject.GetComponent<AvatarCameraFade>();
		if (component != null)
		{
			component.enabled = true;
		}
	}

	public override void Respawn()
	{
		transform.rotation = lookAtTransform.rotation;
		targetRot.SetTargetRotation(lookAtTransform.rotation);
		currentLookAt = lookAtTransform.position + avatarHeadOffset;
		smoothLookAt.Clear();
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		avatarHeadOffset = new Vector3(0f, height, 0f);
		transform.rotation = targetRot.GetLerpRotation(transform.rotation, aroundXInertia, aroundYInertia);
		distance = Mathf.Lerp(distance, distanceToAvatar, targetDistanceStrength * Time.deltaTime);
		UpdatePosition();
		CameraCollision();
		float num = Mathf.Abs(actualLookAt.y - lookAtPos.y);
		num *= num;
		Vector3 fromDirection = currentLookAt - transform.position;
		Vector3 toDirection = lookAtPos - transform.position;
		Quaternion b = Quaternion.FromToRotation(fromDirection, toDirection);
		b = Quaternion.Slerp(Quaternion.identity, b, Time.deltaTime * followRotationSpeed * num);
		targetTransform.position = transform.position + shakeOffset;
		targetTransform.rotation = b * transform.rotation;
	}

	private void UpdatePosition()
	{
		transform.position += lookAtTransform.position - prevLookAtTransformPos;
		float num = distance / distanceToAvatar;
		Quaternion identity = Quaternion.identity;
		identity.eulerAngles = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
		lookAtPos = lookAtTransform.position + identity * (avatarHeadOffset + lookAtOffset * num);
		currentLookAt = lookAtPos;
		Vector3 vector = smoothLookAt.GetCurrentLookAt(MVGameController.WOCM.AvatarLocal.RigidBody.Velocity);
		currentLookAt -= vector;
		Vector3 vector2 = lookAtTransform.position + avatarHeadOffset;
		Vector3 vector3 = (currentLookAt - vector2) * num;
		Vector3 vector4 = transform.position - vector2;
		float magnitude = vector3.magnitude;
		float num2 = Vector3.Dot(vector3.normalized, vector4.normalized);
		float num3 = Mathf.Sqrt(distance * distance + magnitude * magnitude - 2f * distance * magnitude * num2);
		actualLookAt = vector2 + vector3;
		currentLookAt = actualLookAt;
		transform.position = actualLookAt - transform.forward * num3;
		float magnitude2 = MVGameController.WOCM.AvatarLocal.RigidBody.Velocity.magnitude;
		Shake(magnitude2);
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
			float num3 = Mathf.Sqrt(cameraRadius * cameraRadius - num2 * num2);
			float num4 = distance;
			transform.position = intersection - ray.direction * num3;
			distance = (vector - transform.position).magnitude;
			float num5 = distance / num4;
			Vector3 vector3 = currentLookAt - vector;
			currentLookAt = vector + num5 * vector3;
			actualLookAt = currentLookAt;
		}
	}
}
