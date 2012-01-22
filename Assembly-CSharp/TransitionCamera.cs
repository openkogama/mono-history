using System;
using UnityEngine;

public class TransitionCamera : MVCameraBase
{
	private bool superSoft;

	public float rotPercentage;

	public float time = 5f;

	private Vector3 position;

	private Quaternion rotation;

	public override void Init(MVCameraController camController)
	{
		rotPercentage = 1f;
	}

	public void InitTransition(MVCameraController camController, Transform targetCameraTransform, float transitionTime = 2f, bool soft = false)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		position = ((Component)camController).transform.position;
		rotation = ((Component)camController).transform.rotation;
		((Component)this).transform.position = position;
		((Component)this).transform.rotation = rotation;
		time = transitionTime;
		superSoft = soft;
		rotPercentage = 0f;
	}

	public override void UpdateCamera(MVCameraController camController, Transform targetTransform)
	{
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		rotPercentage += 1f / time * Time.deltaTime;
		if (rotPercentage > 1f)
		{
			rotPercentage = 1f;
		}
		if (superSoft)
		{
			Quaternion val = RotateTowardsX(((Component)this).transform.eulerAngles, ((Component)camController.CurCamera).transform.eulerAngles, HalfBell(rotPercentage));
			Quaternion val2 = RotateTowardsY(((Component)this).transform.eulerAngles, ((Component)camController.CurCamera).transform.eulerAngles, HalfBell(rotPercentage));
			((Component)this).transform.position = Vector3.Slerp(((Component)this).transform.position, ((Component)camController.CurCamera).transform.position, rotPercentage);
			((Component)this).transform.rotation = val2 * val;
		}
		else
		{
			Quaternion val3 = RotateTowardsX(rotation.eulerAngles, ((Component)camController.CurCamera).transform.eulerAngles, HalfBell(rotPercentage));
			Quaternion val4 = RotateTowardsY(rotation.eulerAngles, ((Component)camController.CurCamera).transform.eulerAngles, HalfBell(rotPercentage));
			((Component)this).transform.position = Vector3.Slerp(position, ((Component)camController.CurCamera).transform.position, rotPercentage);
			((Component)this).transform.rotation = val4 * val3;
		}
		base.UpdateCamera(camController, targetTransform);
	}

	private float HalfBell(float percentage)
	{
		return (Mathf.Sin(-(float)Math.PI / 2f + percentage * (float)Math.PI) + 1f) / 2f;
	}

	private Quaternion RotateTowardsY(Vector3 eulerFrom, Vector3 eulerTo, float percentage)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		eulerTo.x = 0f;
		eulerTo.z = 0f;
		Quaternion val = Quaternion.Euler(eulerTo);
		eulerFrom.x = 0f;
		eulerFrom.z = 0f;
		Quaternion val2 = Quaternion.Euler(eulerFrom);
		return Quaternion.Slerp(val2, val, percentage);
	}

	private Quaternion RotateTowardsX(Vector3 eulerFrom, Vector3 eulerTo, float percentage)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		eulerTo.y = 0f;
		eulerTo.z = 0f;
		Quaternion val = Quaternion.Euler(eulerTo);
		eulerFrom.y = 0f;
		eulerFrom.z = 0f;
		Quaternion val2 = Quaternion.Euler(eulerFrom);
		return Quaternion.Slerp(val2, val, percentage);
	}
}
