using System.Collections.Generic;
using UnityEngine;

public class AndroidThirdPersonCameraOverTheShoulder : MVCameraBase
{
	private readonly Vector3 avatarHeadOffset = new Vector3(0f, 1.5f, 0f);

	private readonly Vector3 lookAtOffset = new Vector3(1.5f, 0f, 0f);

	private HashSet<int> ignoreAvatarId;

	private Transform lookAtTransform;

	private float pitchSensitivity = 0.2f;

	private float yawSensitivity = 0.2f;

	private readonly CameraCollision cameraCollision = new CameraCollision();

	[SerializeField]
	private float distanceToAvatar = 5f;

	[SerializeField]
	private float minimumY = -60f;

	[SerializeField]
	private float maximumY = 60f;

	[SerializeField]
	private CameraShake cameraShake;

	public override CameraType CameraType => CameraType.OverTheShoulder;

	public override void Enter(MVCameraController cameraController)
	{
		lookAtTransform = MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform;
		Vector3 eulerAngles = lookAtTransform.transform.rotation.eulerAngles;
		eulerAngles.x = 0f;
		transform.rotation = Quaternion.Euler(eulerAngles);
		cameraController.AvatarCameraFade.enabled = true;
		cameraController.AvatarCameraFade.Setup(avatarHeadOffset, 2f, 1f);
		ignoreAvatarId = new HashSet<int> { MVGameControllerBase.WOCM.AvatarLocal.Id };
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		if (!MVGameControllerBase.WOCM.AvatarLocal.InGunMode)
		{
			MVGameControllerBase.CameraController.SetCamera(CameraType.ThirdPerson);
			MVGameControllerBase.CameraController.StartTransitionCam(0.3f);
			return;
		}
		UpdateTargetRotation();
		UpdatePosition();
		if (cameraCollision.Collide(out var newPos, cameraRadius, distanceToAvatar, lookAtTransform.position + avatarHeadOffset, transform.position, ignoreAvatarId))
		{
			transform.position = newPos;
		}
		targetTransform.position = cameraShake.Shake(transform.position, MVGameControllerBase.WOCM.AvatarLocal.Velocity.magnitude);
		targetTransform.rotation = transform.rotation;
	}

	public override void Exit(MVCameraController camController)
	{
		camController.AvatarCameraFade.enabled = false;
	}

	private void UpdatePosition()
	{
		Quaternion identity = Quaternion.identity;
		identity.eulerAngles = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
		Vector3 vector = lookAtTransform.position + identity * (avatarHeadOffset + lookAtOffset);
		Vector3 vector2 = lookAtTransform.position + avatarHeadOffset;
		Vector3 vector3 = vector - vector2;
		Vector3 vector4 = transform.position - vector2;
		float magnitude = vector3.magnitude;
		float num = Vector3.Dot(vector3.normalized, vector4.normalized);
		float num2 = Mathf.Sqrt(distanceToAvatar * distanceToAvatar + magnitude * magnitude - 2f * distanceToAvatar * magnitude * num);
		transform.position = vector - transform.forward * num2;
	}

	private void UpdateTargetRotation()
	{
		Vector3 eulerAngles = transform.rotation.eulerAngles;
		float num = eulerAngles.x;
		float num2 = eulerAngles.y;
		if (!MVGameControllerBase.IPlayModeUI.InLobbyState && (ignoreInputTypes & IgnoreInputTypes.MouseMovement) == 0)
		{
			num2 += MVInputWrapper.GetAxis("Mouse X") * yawSensitivity;
			num += MVInputWrapper.GetAxis("Mouse Y") * pitchSensitivity;
		}
		num = MathFunctions.NormalizeAngle(num);
		if (num > 180f)
		{
			num -= 360f;
		}
		num = Mathf.Clamp(num, minimumY, maximumY);
		eulerAngles = new Vector3(num, num2, 0f);
		transform.rotation = Quaternion.Euler(eulerAngles);
	}
}
