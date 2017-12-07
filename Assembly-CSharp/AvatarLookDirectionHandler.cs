using System;
using UnityEngine;

public class AvatarLookDirectionHandler
{
	public Action<float> OnLookDirectionYawChange;

	public Action<float> OnLookDirectionPitchChange;

	private MVAvatar avatar;

	private Vector3 localLookDirection;

	private float previousYaw;

	private float previousPitch;

	public Vector3 LocalLookDirection => localLookDirection;

	public void Initialize(MVAvatar avatar)
	{
		this.avatar = avatar;
	}

	public void Update(Vector3 lookDirection)
	{
		Transform transform = avatar.Transform;
		localLookDirection = transform.InverseTransformPoint(transform.position + lookDirection);
		UpdateYaw();
		UpdatePitch();
	}

	private void UpdateYaw()
	{
		float y = MVGameControllerBase.CameraController.transform.rotation.eulerAngles.y;
		if (y != previousYaw)
		{
			if (OnLookDirectionYawChange != null)
			{
				OnLookDirectionYawChange(y);
			}
			previousYaw = y;
		}
	}

	private void UpdatePitch()
	{
		float x = MVGameControllerBase.CameraController.transform.rotation.eulerAngles.x;
		if (x != previousPitch)
		{
			if (OnLookDirectionPitchChange != null)
			{
				OnLookDirectionPitchChange(x);
			}
			previousPitch = x;
		}
	}
}
