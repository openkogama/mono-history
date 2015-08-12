using System;
using UnityEngine;

public class TransitionCamera : MVCameraBase
{
	private bool superSoft;

	private Vector3 position;

	private Quaternion rotation;

	public float time = 5f;

	private float rotPercentage = 1f;

	public float RotPercentage => rotPercentage;

	public override CameraType CameraType => CameraType.TransitionCamera;

	public void InitTransition(MVCameraController camController, Transform targetCameraTransform, float transitionTime = 2f, bool soft = false)
	{
		position = camController.transform.position;
		rotation = camController.transform.rotation;
		transform.position = position;
		transform.rotation = rotation;
		time = transitionTime;
		superSoft = soft;
		rotPercentage = 0f;
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		rotPercentage += 1f / time * Time.deltaTime;
		if (rotPercentage > 1f)
		{
			rotPercentage = 1f;
		}
		if (superSoft)
		{
			Quaternion quaternion = RotateTowardsX(transform.eulerAngles, camController.CurCamera.transform.eulerAngles, HalfBell(rotPercentage));
			Quaternion quaternion2 = RotateTowardsY(transform.eulerAngles, camController.CurCamera.transform.eulerAngles, HalfBell(rotPercentage));
			transform.position = Vector3.Slerp(transform.position, camController.CurCamera.transform.position, rotPercentage);
			transform.rotation = quaternion2 * quaternion;
		}
		else
		{
			Quaternion quaternion3 = RotateTowardsX(rotation.eulerAngles, camController.CurCamera.transform.eulerAngles, HalfBell(rotPercentage));
			Quaternion quaternion4 = RotateTowardsY(rotation.eulerAngles, camController.CurCamera.transform.eulerAngles, HalfBell(rotPercentage));
			transform.position = Vector3.Slerp(position, camController.CurCamera.transform.position, rotPercentage);
			transform.rotation = quaternion4 * quaternion3;
		}
		base.UpdateCamera(camController, targetTransform);
	}

	private float HalfBell(float percentage)
	{
		return (Mathf.Sin(-(float)Math.PI / 2f + percentage * (float)Math.PI) + 1f) / 2f;
	}

	private Quaternion RotateTowardsY(Vector3 eulerFrom, Vector3 eulerTo, float percentage)
	{
		eulerTo.x = 0f;
		eulerTo.z = 0f;
		Quaternion to = Quaternion.Euler(eulerTo);
		eulerFrom.x = 0f;
		eulerFrom.z = 0f;
		Quaternion quaternion = Quaternion.Euler(eulerFrom);
		return Quaternion.Slerp(quaternion, to, percentage);
	}

	private Quaternion RotateTowardsX(Vector3 eulerFrom, Vector3 eulerTo, float percentage)
	{
		eulerTo.y = 0f;
		eulerTo.z = 0f;
		Quaternion to = Quaternion.Euler(eulerTo);
		eulerFrom.y = 0f;
		eulerFrom.z = 0f;
		Quaternion quaternion = Quaternion.Euler(eulerFrom);
		return Quaternion.Slerp(quaternion, to, percentage);
	}
}
