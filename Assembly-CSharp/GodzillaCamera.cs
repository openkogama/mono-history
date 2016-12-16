using System;
using UnityEngine;

public class GodzillaCamera : MVCameraBase
{
	public const float localBodyTransparancy = 0.9f;

	[SerializeField]
	private Vector3 cameraOffset;

	[SerializeField]
	private float mouseSensitivity = 1f;

	[SerializeField]
	private float avatarTurnRate;

	[SerializeField]
	private float maxLookAngle;

	[SerializeField]
	private Vector2 targetRotationVector;

	[SerializeField]
	private TargetRotation targetRotation;

	[SerializeField]
	private GodzillaGUI gui;

	private MVAvatar player;

	public override CameraType CameraType => CameraType.GodzillaModeMainCamera;

	public override void Awake()
	{
		gui.transform.parent = null;
	}

	private void OnDestroy()
	{
		if (gui != null)
		{
			UnityEngine.Object.Destroy(gui.gameObject);
		}
	}

	private void UpdateCameraPosition()
	{
		Transform relativeTo = player.Transform;
		transform.position = player.Body.BodyData.GetPartBone(BodyData.PartIndex.Head).transform.position;
		transform.Translate(cameraOffset * player.Scale.y, relativeTo);
	}

	private float DegreesBetween(float eulerA, float eulerB)
	{
		eulerA = Mathf.Repeat(eulerA, 360f);
		eulerB = Mathf.Repeat(eulerB, 360f);
		float num = Mathf.Abs(eulerA - eulerB);
		return (!(num > 180f)) ? num : (360f - num);
	}

	private float EulerClamp(float a, float min, float max)
	{
		a = Mathf.Repeat(a, 360f);
		min = Mathf.Repeat(min, 360f);
		max = Mathf.Repeat(max, 360f);
		if (a >= min || a <= max)
		{
			return a;
		}
		return (!(DegreesBetween(a, min) < DegreesBetween(a, max))) ? max : min;
	}

	protected virtual Vector2 GetMouseInput()
	{
		return new Vector2(0f - MVInputWrapper.GetAxis("Mouse Y"), MVInputWrapper.GetAxis("Mouse X"));
	}

	private void UpdateCameraRotation()
	{
		if (InputActive)
		{
			float y = player.Transform.rotation.eulerAngles.y;
			float y2 = transform.rotation.eulerAngles.y;
			float num = DegreesBetween(y, y2);
			num /= maxLookAngle;
			float num2 = mouseSensitivity * Mathf.Clamp(1f - Mathf.Sqrt(num), 0f, 1f);
			Vector2 vector = GetMouseInput() * num2;
			vector *= Time.deltaTime;
			targetRotationVector.x += vector.x;
			targetRotationVector.x = EulerClamp(targetRotationVector.x, -90f, 90f);
			targetRotationVector.y += vector.y;
			targetRotationVector.y = EulerClamp(targetRotationVector.y, y - 90f, y + 90f);
			targetRotation.SetTargetRotation(targetRotationVector);
			transform.rotation = targetRotation.GetLerpRotation(transform.rotation);
		}
	}

	public void UpdateAvatar()
	{
		Vector3 eulerAngles = player.Transform.rotation.eulerAngles;
		Vector3 eulerAngles2 = transform.eulerAngles;
		Quaternion to = Quaternion.Euler(eulerAngles.x, eulerAngles2.y, eulerAngles.z);
		player.Transform.rotation = Quaternion.RotateTowards(player.Transform.rotation, to, avatarTurnRate * Time.deltaTime);
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		UpdateCameraPosition();
		UpdateCameraRotation();
		UpdateAvatar();
		targetTransform.position = transform.position;
		targetTransform.rotation = transform.rotation;
	}

	public override void Enter(MVCameraController camController)
	{
		base.Enter(camController);
		camController.AvatarCameraFade.enabled = false;
		player = MVGameControllerBase.WOCM.AvatarLocal;
		player.SetTransparency = 0.9f;
		transform.rotation = MVGameControllerBase.WOCM.AvatarLocal.Transform.rotation;
		targetRotationVector.x = player.Transform.rotation.eulerAngles.x;
		targetRotationVector.y = player.Transform.rotation.eulerAngles.y;
		MVGameControllerBase.CameraController.StartTransitionCam(0.3f);
		MVAvatar mVAvatar = player;
		mVAvatar.OnDamageTaken = (Action<float, float>)Delegate.Combine(mVAvatar.OnDamageTaken, new Action<float, float>(gui.AdaptiveFlash));
	}

	public override void Exit(MVCameraController camController)
	{
		base.Exit(camController);
		camController.AvatarCameraFade.enabled = true;
		MVGameControllerBase.WOCM.AvatarLocal.SetTransparency = 1f;
		MVAvatar mVAvatar = player;
		mVAvatar.OnDamageTaken = (Action<float, float>)Delegate.Remove(mVAvatar.OnDamageTaken, new Action<float, float>(gui.AdaptiveFlash));
	}
}
