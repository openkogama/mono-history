using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class AndroidPlatformerCamera : MVCameraBase, ICameraSettings
{
	private const float distanceToAvatarOffset = 10f;

	private float baseDistanceToAvatar = 18f;

	private float distanceToAvatar = 18f;

	private readonly Offset2DCameraInDirection offset2DCameraInDirection = new Offset2DCameraInDirection();

	public static bool snapbackMode = true;

	public override CameraType CameraType => CameraType.Platformer;

	public override void Awake()
	{
		base.Awake();
		SetDefaultSettings();
		MVCameraController.RegisterCameraWithSettings(MVGameType.Platformer, this);
	}

	public override void Reset()
	{
		base.Reset();
		offset2DCameraInDirection.Reset();
	}

	public void UpdateFromCameraSettings(Dictionary<object, object> data)
	{
		distanceToAvatar = (float)data["distanceToAvatar"] + 10f;
		offset2DCameraInDirection.Scale = distanceToAvatar / baseDistanceToAvatar;
	}

	public void SetDefaultSettings()
	{
		distanceToAvatar = baseDistanceToAvatar;
		offset2DCameraInDirection.Scale = distanceToAvatar / baseDistanceToAvatar;
	}

	public void ScaleCameraValues(float scale)
	{
		Debug.Log("Platformer camera does not scale");
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		transform.rotation = Quaternion.identity;
		Vector3 direction = new Vector3(MVInputWrapper.GetAxis("Mouse X"), MVInputWrapper.GetAxis("Mouse Y"));
		Vector3 movementOffset = offset2DCameraInDirection.GetMovementOffset(direction);
		transform.position = MVGameControllerBase.WOCM.AvatarLocal.LookAtPos - transform.rotation * Vector3.forward * distanceToAvatar + movementOffset;
		base.UpdateCamera(camController, targetTransform);
	}
}
