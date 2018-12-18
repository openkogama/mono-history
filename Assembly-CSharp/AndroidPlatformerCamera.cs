using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class AndroidPlatformerCamera : MVCameraBase, ICameraSettings
{
	public const bool snapbackMode = true;

	private const float baseDistanceToAvatar = 18f;

	private const float distanceToAvatarOffset = 10f;

	private readonly Offset2DCameraInDirection offset2DCameraInDirection = new Offset2DCameraInDirection();

	private float distanceToAvatar = 18f;

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
		offset2DCameraInDirection.Scale = distanceToAvatar / 18f;
	}

	public void SetDefaultSettings()
	{
		distanceToAvatar = 18f;
		offset2DCameraInDirection.Scale = distanceToAvatar / 18f;
	}

	public void ScaleCameraValues(float scale)
	{
		Debug.Log("Platformer camera does not scale");
	}

	public override void UpdateCamera(MVCameraController camController, ProtectedTransform targetTransform)
	{
		transform.rotation = Quaternion.identity;
		Vector3 direction = new Vector3(MVInputWrapper.GetAxisWithoutSensitivity("Mouse X"), MVInputWrapper.GetAxisWithoutSensitivity("Mouse Y"));
		Vector3 movementOffset = offset2DCameraInDirection.GetMovementOffset(direction);
		transform.position = MVGameControllerBase.WOCM.AvatarLocal.LookAtPos - transform.rotation * Vector3.forward * distanceToAvatar + movementOffset;
		base.UpdateCamera(camController, targetTransform);
	}
}
