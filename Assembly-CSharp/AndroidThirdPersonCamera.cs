using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class AndroidThirdPersonCamera : MVCameraBase, ICameraSettings
{
	[SerializeField]
	private SmoothTouchAxis SmoothTouchAxis;

	[SerializeField]
	private AxisBias axisBias;

	[SerializeField]
	private TargetRotation targetRotation;

	[SerializeField]
	private InputMovementPrecisionModifier inputMovementPrecisionModifier;

	[SerializeField]
	private float minimumY = -60f;

	[SerializeField]
	private float maximumY = 60f;

	[SerializeField]
	private Vector3 lookAtOffsetBase = new Vector3(0f, 2.5f, 0f);

	[SerializeField]
	private Vector3 lookAtHeightOffsetBase = new Vector3(0f, 2.5f, 0f);

	[SerializeField]
	private CameraShake cameraShake;

	private readonly CameraCollisionWithSliding cameraCollision = new CameraCollisionWithSliding();

	private readonly CameraLerpToDesiredDistance cameraLerpToDesiredDistance = new CameraLerpToDesiredDistance();

	private const float pitchSensitivity = 0.167f;

	private const float yawSensitivity = 0.208f;

	private const float basePitch = 20f;

	private HashSet<int> ignoreAvatarId;

	private Transform lookAtTransform;

	private float distanceToAvatarBase = 5f;

	private float currentDistanceToAvatar;

	private float desiredDistanceToAvatar;

	private Vector3 lookAtOffset;

	private Vector3 lookAtHeightOffset;

	private MVAvatarLocal avatarLocal;

	private AvatarCameraDistTransparency avatarCameraDistTransparency;

	private const string mouseX = "Mouse X";

	private const string mouseY = "Mouse Y";

	public override CameraType CameraType => CameraType.ThirdPerson;

	public override void Awake()
	{
		lookAtOffset = lookAtOffsetBase;
		lookAtHeightOffset = lookAtHeightOffsetBase;
		currentDistanceToAvatar = distanceToAvatarBase;
		desiredDistanceToAvatar = distanceToAvatarBase;
		MainCameraManager.RegisterCameraWithSettings(MVGameType.Classic, this);
	}

	public void Initialize(MVAvatarLocal avatarLocal)
	{
		this.avatarLocal = avatarLocal;
		avatarCameraDistTransparency = new AvatarCameraDistTransparency(lookAtOffset, 2f, 1f);
	}

	public void SetDefaultSettings()
	{
		distanceToAvatarBase = 5f;
		desiredDistanceToAvatar = distanceToAvatarBase;
	}

	private void ResetScaledValues()
	{
		desiredDistanceToAvatar = distanceToAvatarBase;
		lookAtOffset = lookAtOffsetBase;
		lookAtHeightOffset = lookAtHeightOffsetBase;
	}

	public void ScaleCameraValues(float scale)
	{
		ResetScaledValues();
		desiredDistanceToAvatar *= scale;
		lookAtOffset *= scale;
		lookAtHeightOffset *= scale;
		cameraRadius *= scale;
		avatarCameraDistTransparency.SetScaleFadeDistance(scale);
	}

	public void UpdateFromCameraSettings(Dictionary<object, object> data)
	{
		distanceToAvatarBase = (float)data["distanceToAvatar"];
		desiredDistanceToAvatar = distanceToAvatarBase;
	}

	public override void Enter(MVCameraController cameraController)
	{
		lookAtTransform = avatarLocal.GameObject.transform;
		ignoreAvatarId = new HashSet<int> { avatarLocal.Id };
		Reset();
	}

	public override void Resume(MVCameraController camController)
	{
		Reset();
	}

	public override void Reset()
	{
		Vector3 euler = new Vector3(20f, lookAtTransform.transform.rotation.eulerAngles.y, 0f);
		transform.rotation = Quaternion.Euler(euler);
		targetRotation.SetTargetRotation(euler.x, euler.y);
		UpdatePosition();
	}

	public override void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
	{
		LerpCurrentDistanceToDesiredDistance();
		UpdateTargetRotation();
		UpdatePosition();
		HandleCollision();
		targetTransform.position = cameraShake.Shake(transform.position, avatarLocal.VelocityRelative.magnitude);
		targetTransform.rotation = transform.rotation;
		avatarCameraDistTransparency.Update(avatarLocal);
	}

	private void LerpCurrentDistanceToDesiredDistance()
	{
		float num = desiredDistanceToAvatar - currentDistanceToAvatar;
		float num2 = 0f;
		if (desiredDistanceToAvatar > currentDistanceToAvatar)
		{
			num2 = Mathf.Min(4f * Time.deltaTime, num);
		}
		else if (desiredDistanceToAvatar < currentDistanceToAvatar)
		{
			num2 = 0f - Mathf.Min(4f * Time.deltaTime, Mathf.Abs(num));
		}
		currentDistanceToAvatar += num2;
	}

	private void HandleCollision()
	{
		Vector3 targetPosition = lookAtTransform.position + lookAtHeightOffset;
		if (cameraCollision.Collide(out var newPos, cameraRadius, currentDistanceToAvatar, targetPosition, transform.position, ignoreAvatarId))
		{
			transform.position = newPos;
		}
		transform.position = cameraLerpToDesiredDistance.Update(targetPosition, transform.position);
	}

	private void UpdatePosition()
	{
		Quaternion identity = Quaternion.identity;
		identity.eulerAngles = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
		Vector3 vector = lookAtTransform.position + identity * lookAtOffset;
		transform.position = vector - transform.forward * currentDistanceToAvatar;
	}

	private void UpdateTargetRotation()
	{
	}

	public override void Exit(MVCameraController camController)
	{
	}
}
