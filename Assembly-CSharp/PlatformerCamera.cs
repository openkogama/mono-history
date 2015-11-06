using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class PlatformerCamera : MVCameraBase, ICameraSettings
{
	private const float distanceToAvatarOffset = 10f;

	private Vector3 currentOffset = Vector3.zero;

	private float rotOffset = 10f;

	private float baseDistanceToAvatar = 20f;

	private float distanceToAvatar = 20f;

	private float lerpSpeed = 10f;

	public override CameraType CameraType => CameraType.Platformer;

	public float Scale => distanceToAvatar / baseDistanceToAvatar;

	public override void Awake()
	{
		base.Awake();
		SetDefaultSettings();
		MVCameraController.RegisterCameraWithSettings(MVGameType.Platformer, this);
	}

	public void UpdateFromCameraSettings(Dictionary<object, object> data)
	{
		distanceToAvatar = (float)data["distanceToAvatar"] + 10f;
	}

	public void SetDefaultSettings()
	{
		distanceToAvatar = baseDistanceToAvatar;
	}

	public void ScaleCameraValues(float scale)
	{
		Debug.Log("Platformer camera does not scale");
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		transform.eulerAngles = new Vector3(rotOffset, 0f, 0f);
		((MVGUICrossHairLegacy)MVGameControllerBase.IPlayModeUI.GetCrossHair()).UpdateCrosshairPosition();
		currentOffset = Vector3.Lerp(currentOffset, transform.rotation * MVGameControllerBase.IPlayModeUI.GetCrossHair().Direction, lerpSpeed * Time.deltaTime);
		transform.position = MVGameControllerBase.IPlayModeUI.GetCrossHair().Origin - transform.rotation * Vector3.forward * distanceToAvatar + currentOffset;
		base.UpdateCamera(camController, targetTransform);
	}
}
