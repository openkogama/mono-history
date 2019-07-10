using System.Collections.Generic;
using UnityEngine;

public abstract class PlaymodeCamera : MVPlaymodeCameraBase
{
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

	public float minimumY = -60f;

	public float maximumY = 60f;

	public Vector3 shoulderOffset = new Vector3(1.5f, 0f, -0.2f);

	private Vector3 avatarHeadOffset = new Vector3(0f, 1.5f, 0f);

	protected AvatarCameraDistTransparency avatarCameraDistTransparency;

	public float targetDistanceStrength = 2f;

	public float followRotationSpeed = 2f;

	public Vector3 lookAtOffset = new Vector3(0f, 2.5f, 0f);

	protected Vector3 currentLookAtOffset;

	protected bool autoRotate;

	protected Vector3 currentLookAt = Vector3.zero;

	protected Vector3 actualLookAt = Vector3.zero;

	protected float distance = 2f;

	protected HashSet<int> ignoreAvatarId;

	protected Transform lookAtTransform;

	[SerializeField]
	protected TargetRotation targetRot;

	protected Vector3 lookAtPos = Vector3.zero;

	public float mouseSensitivity = 0.25f;

	protected float aroundYInertia;

	protected float lookAtScaleCorrection = 1f;

	protected MVAvatarLocal avatarLocal;

	private SmoothLookAt smoothLookAt = new SmoothLookAt();

	private Vector3 prevLookAtTransformPos = Vector3.zero;

	public override void Awake()
	{
		base.Awake();
		distance = distanceToAvatar;
		currentLookAtOffset = lookAtOffset;
	}

	public virtual void Initialize(MVAvatarLocal avatarLocal)
	{
		this.avatarLocal = avatarLocal;
		avatarCameraDistTransparency = new AvatarCameraDistTransparency(avatarHeadOffset, 4f, 1f);
	}

	public override void Enter(MVCameraController cameraController)
	{
		base.Enter(cameraController);
		lookAtTransform = avatarLocal.GameObject.transform;
		currentLookAt = MVGameControllerBase.MainCameraManager.transform.position;
		transform.position = MVGameControllerBase.MainCameraManager.transform.position;
		transform.rotation = MVGameControllerBase.MainCameraManager.transform.rotation;
		prevLookAtTransformPos = lookAtTransform.transform.position;
		targetRot.SetTargetRotation(MVGameControllerBase.MainCameraManager.transform.rotation);
		distance = distanceToAvatar;
		ignoreAvatarId = new HashSet<int> { avatarLocal.Id };
	}

	public override void Reset()
	{
		transform.rotation = lookAtTransform.rotation;
		targetRot.SetTargetRotation(lookAtTransform.rotation);
		currentLookAt = lookAtTransform.position + avatarHeadOffset;
		smoothLookAt.Clear();
	}

	public override void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
	{
		avatarHeadOffset = new Vector3(0f, height, 0f);
		transform.rotation = targetRot.GetLerpRotation(transform.rotation);
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
		UpdateImpactSimulation(targetTransform);
		avatarCameraDistTransparency.Update(avatarLocal);
	}

	private void UpdatePosition()
	{
		transform.position += lookAtTransform.position - prevLookAtTransformPos;
		float num = distance / distanceToAvatar * lookAtScaleCorrection;
		Quaternion identity = Quaternion.identity;
		identity.eulerAngles = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
		lookAtPos = lookAtTransform.position + identity * (avatarHeadOffset + currentLookAtOffset * num);
		currentLookAt = lookAtPos;
		Vector3 vector = smoothLookAt.GetCurrentLookAt(avatarLocal.VelocityRelative);
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
		float magnitude2 = avatarLocal.VelocityRelative.magnitude;
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
			actualLookAt = currentLookAt;
		}
	}
}
