using System;
using UnityEngine;

public class FallBehindPitch
{
	private enum State
	{
		FallingBehind,
		RotationSet
	}

	private const float distanceBeforeFallBehind = 2f;

	private const float degreesPrMeter = 3f;

	private Vector3 prevCameraRotatedPosition = Vector3.zero;

	private Vector3 prevPosition;

	private State state;

	public Quaternion Update(Quaternion rotation, Vector3 position, float basePitch)
	{
		switch (state)
		{
		case State.RotationSet:
			if ((prevCameraRotatedPosition - position).magnitude > 2f)
			{
				prevPosition = position;
				state = State.FallingBehind;
			}
			return rotation;
		case State.FallingBehind:
			return FallBehind(rotation, position, basePitch);
		default:
			throw new Exception("Unknown state");
		}
	}

	private Quaternion FallBehind(Quaternion rotation, Vector3 position, float basePitch)
	{
		Vector3 eulerAngles = rotation.eulerAngles;
		eulerAngles.x = DoFallBehind(eulerAngles.x, position, basePitch);
		return Quaternion.Euler(eulerAngles);
	}

	private float DoFallBehind(float currentPitch, Vector3 position, float basePitch)
	{
		currentPitch = MathFunctions.NormalizeAngle(currentPitch);
		if (currentPitch > 180f)
		{
			currentPitch -= 360f;
		}
		if (currentPitch == basePitch)
		{
			return currentPitch;
		}
		float num = (position - prevPosition).magnitude * 3f;
		prevPosition = position;
		if (currentPitch >= basePitch)
		{
			return Mathf.Max(currentPitch - num, basePitch);
		}
		return Mathf.Min(currentPitch + num, basePitch);
	}

	public void SetCameraRotatePos(Vector3 position)
	{
		prevCameraRotatedPosition = position;
		state = State.RotationSet;
	}
}
