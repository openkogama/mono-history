using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class AndroidThirdPersonCamera : MVCameraBase, ICameraSettings
{
	private const float pitchSensitivity = 0.15f;

	private const float yawSensitivity = 0.3f;

	private const float basePitch = 10f;

	private const float localPitch = -10f;

	[SerializeField]
	private AndroidThirdPersonCameraOverTheShoulder overTheShoulderCamera;

	private readonly CameraCollisionWithSliding cameraCollision = new CameraCollisionWithSliding();

	private readonly FallBehindPitch fallBehindPitch = new FallBehindPitch();

	private readonly CameraLerpToDesiredDistance cameraLerpToDesiredDistance = new CameraLerpToDesiredDistance();

	private HashSet<int> ignoreAvatarId;

	private Transform lookAtTransform;

	[SerializeField]
	private float distanceToAvatar = 5f;

	[SerializeField]
	private float minimumY = -60f;

	[SerializeField]
	private float maximumY = 60f;

	[SerializeField]
	private Vector3 lookAtOffset = new Vector3(0f, 2.5f, 0f);

	[SerializeField]
	private CameraShake cameraShake;

	public override CameraType CameraType => CameraType.ThirdPerson;

	public override void Awake()
	{
		MVCameraController.RegisterCameraWithSettings(MVGameType.Classic, this);
	}

	public void SetDefaultSettings()
	{
		distanceToAvatar = 5f;
	}

	public void ScaleCameraValues(float scale)
	{
		Debug.LogWarning("Implement scale stuff");
	}

	public void UpdateFromCameraSettings(Dictionary<object, object> data)
	{
		distanceToAvatar = (float)data["distanceToAvatar"];
	}

	public override void Enter(MVCameraController cameraController)
	{
		lookAtTransform = MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform;
		ignoreAvatarId = new HashSet<int> { MVGameControllerBase.WOCM.AvatarLocal.Id };
		cameraController.AvatarCameraFade.enabled = true;
		cameraController.AvatarCameraFade.Setup(lookAtOffset, 2f, 1f);
		Reset();
	}

	public override void Reset()
	{
		Vector3 euler = new Vector3(10f, lookAtTransform.transform.rotation.eulerAngles.y, 0f);
		transform.rotation = Quaternion.Euler(euler);
		HandlePos();
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		if (MVGameControllerBase.WOCM.AvatarLocal.InGunMode)
		{
			MVGameControllerBase.CameraController.SetCamera(CameraType.OverTheShoulder);
			MVGameControllerBase.CameraController.StartTransitionCam(0.3f);
			return;
		}
		if (InputActive)
		{
			HandleSlide();
			HandleRotation();
		}
		HandlePos();
		HandleCollision();
		targetTransform.position = cameraShake.Shake(transform.position, MVGameControllerBase.WOCM.AvatarLocal.Velocity.magnitude);
		targetTransform.rotation = transform.rotation * Quaternion.Euler(-10f, 0f, 0f);
	}

	private void HandleSlide()
	{
		if (CrossPlatformInputManager.GetAxis("Vertical") < 0f && cameraCollision.CollideWithSliding(out var newCameraPosition, cameraRadius, distanceToAvatar, lookAtTransform.position + lookAtOffset, transform.position, ignoreAvatarId))
		{
			transform.position = newCameraPosition;
		}
	}

	private void HandleCollision()
	{
		Vector3 targetPosition = lookAtTransform.position + lookAtOffset;
		if (cameraCollision.Collide(out var newPos, cameraRadius, distanceToAvatar, targetPosition, transform.position, ignoreAvatarId))
		{
			transform.position = newPos;
		}
		transform.position = cameraLerpToDesiredDistance.Update(targetPosition, transform.position);
	}

	private void HandlePos()
	{
		Vector3 vector = transform.rotation * -Vector3.forward;
		vector.Normalize();
		vector *= distanceToAvatar;
		transform.position = lookAtTransform.position + lookAtOffset + vector;
	}

	private void HandleRotation()
	{
		Quaternion mouseRot = GetMouseRot();
		Vector3 vector = lookAtTransform.transform.position + lookAtOffset - transform.position;
		vector.y = 0f;
		Quaternion quaternion = Quaternion.LookRotation(vector.normalized, Vector3.up);
		transform.rotation = quaternion * mouseRot;
		transform.rotation = fallBehindPitch.Update(transform.rotation, transform.position, 10f);
	}

	private Quaternion GetMouseRot()
	{
		float num = 0f;
		float x = transform.rotation.eulerAngles.x;
		if (MVInputWrapper.GetAxisRaw("Mouse X") != 0f || MVInputWrapper.GetAxisRaw("Mouse Y") != 0f)
		{
			fallBehindPitch.SetCameraRotatePos(transform.position);
		}
		num += MVInputWrapper.GetAxisRaw("Mouse X") * 0.3f;
		x += MVInputWrapper.GetAxisRaw("Mouse Y") * 0.15f;
		x = MathFunctions.NormalizeAngle(x);
		if (x > 180f)
		{
			x -= 360f;
		}
		x = Mathf.Clamp(x, minimumY, maximumY);
		return Quaternion.Euler(x, num, 0f);
	}

	public override void Exit(MVCameraController camController)
	{
		camController.AvatarCameraFade.enabled = false;
	}
}
