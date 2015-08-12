using CodeStage.AntiCheat.ObscuredTypes;
using MV.WorldObject;
using UnityEngine;

public static class MVPhysics
{
	public static ObscuredFloat gravity = 30f;

	public static PhysicalProperties airPhysicalProperties = new PhysicalProperties(0f, 0f, 0f, 0f, 0f);

	public static ObscuredFloat Gravity
	{
		get
		{
			return gravity;
		}
		set
		{
			gravity = value;
		}
	}

	public static float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		return Mathf.Sqrt(2f * targetJumpHeight * (float)Gravity);
	}

	public static float CalculateJumpForceFromVerticalVelocity(float velocity)
	{
		return velocity * velocity / 2f / (float)Gravity;
	}
}
