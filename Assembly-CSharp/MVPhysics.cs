using MV.WorldObject;
using UnityEngine;

public static class MVPhysics
{
	public const float gravity = 30f;

	public static PhysicalProperties airPhysicalProperties = new PhysicalProperties(0f, 0f, 0f, 0f, 0f);

	public static float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		return Mathf.Sqrt(2f * targetJumpHeight * 30f);
	}

	public static float CalculateJumpForceFromVerticalVelocity(float velocity)
	{
		return velocity * velocity / 2f / 30f;
	}
}
