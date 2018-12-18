using System;

namespace MV.WorldObject;

public static class MVMath
{
	public const float Epsilon = float.Epsilon;

	public static bool ValidateFloat(float validateFloat)
	{
		if (float.IsInfinity(validateFloat) || float.IsNaN(validateFloat))
		{
			return false;
		}
		return true;
	}

	public static float TryValidateFloat(float f)
	{
		if (float.IsInfinity(f) || float.IsNaN(f))
		{
			throw new InvalidFloatException();
		}
		return f;
	}

	public static bool IsApproximatelyEqual(float a, float b, float delta)
	{
		if (Math.Abs(a - b) < delta)
		{
			return true;
		}
		return false;
	}
}
