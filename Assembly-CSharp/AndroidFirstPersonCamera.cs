using UnityEngine;

public class AndroidFirstPersonCamera : FirstPersonCamera
{
	[SerializeField]
	private AxisBias axisBias;

	[SerializeField]
	private InputMovementPrecisionModifier inputMovementPrecisionModifier;

	protected override void UpdateCameraRotation()
	{
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
}
