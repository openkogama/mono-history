using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class ThirdPersonCamera : PlaymodeCamera, ICameraSettings
{
	private float baseDistanceSettings = 5f;

	private const string mouseX = "Mouse X";

	private const string mouseY = "Mouse Y";

	public override CameraType CameraType => CameraType.ThirdPerson;

	public override void Awake()
	{
		base.Awake();
		MVCameraController.RegisterCameraWithSettings(MVGameType.Classic, this);
	}

	public override void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
	{
		UpdateTargetRotation();
		HandleGunMode();
		base.UpdateCamera(camController, targetTransform);
	}

	protected override void HandleGunMode()
	{
		if (MVGameControllerBase.WOCM.AvatarLocal.InGunMode)
		{
			MVGameControllerBase.CameraController.PushCamera(CameraType.FirstPersonCamera);
		}
	}

	public void UpdateFromCameraSettings(Dictionary<object, object> data)
	{
		distanceToAvatar = (float)data["distanceToAvatar"];
		baseDistanceSettings = (float)data["distanceToAvatar"];
	}

	public void SetDefaultSettings()
	{
		baseDistanceSettings = 5f;
		distanceToAvatar = baseDistanceSettings;
	}

	public void ScaleCameraValues(float scale)
	{
		ResetScaleValues();
		height *= scale;
		distanceToAvatar *= scale;
		cameraRadius *= scale;
		currentLookAtOffset *= scale;
		lookAtTransform.position *= scale;
		if (scale < 1f)
		{
			lookAtScaleCorrection *= scale;
		}
		shoulderOffset *= scale;
		targetDistanceStrength *= scale;
		MVGameControllerBase.CameraController.AvatarCameraFade.SetScaleFadeDistance(scale);
	}

	private void ResetScaleValues()
	{
		distanceToAvatar = baseDistanceSettings;
		height = 1.5f;
		cameraRadius = 0.3f;
		lookAtTransform = MVGameControllerBase.WOCM.AvatarLocal.GameObject.transform;
		currentLookAtOffset = lookAtOffset;
		lookAtScaleCorrection = 1f;
		shoulderOffset = new Vector3(1.5f, 0f, -0.2f);
		targetDistanceStrength = 2f;
	}

	private void UpdateTargetRotation()
	{
		if (MVGameControllerDesktop.LockCursorManager.CursorLock)
		{
			Vector3 eulerAngles = targetRot.EulerAngles;
			float num = 0f - eulerAngles.x;
			float num2 = eulerAngles.y;
			autoRotate = !MVGameControllerBase.PlayModeUI.InLobbyState;
			if (autoRotate && (ignoreInputTypes & IgnoreInputTypes.MouseMovement) == 0)
			{
				num2 += MVInputWrapper.GetAxis("Mouse X") * mouseSensitivity;
				num += MVInputWrapper.GetAxis("Mouse Y") * mouseSensitivity;
			}
			num = MathFunctions.NormalizeAngle(num);
			if (num > 180f)
			{
				num -= 360f;
			}
			num = Mathf.Clamp(num, minimumY, maximumY);
			eulerAngles = new Vector3(0f - num, num2, 0f);
			targetRot.SetTargetRotation(eulerAngles.x, eulerAngles.y);
		}
	}
}
