using UnityEngine;

public class Offset2DCameraInDirection
{
	private class DistanceScaledValues
	{
		private const float offsetSpeedX = 12f;

		private const float offsetSpeedY = 12f;

		private const float offsetMagnitudeX = 3.5f;

		private const float offsetMagnitudeY = 3.5f;

		public float OffsetSpeedX => 12f * Scale;

		public float OffsetSpeedY => 12f * Scale;

		public float OffsetMagnitudeX => 3.5f * Scale;

		public float OffsetMagnitudeY => 3.5f * Scale;

		public float Scale { get; set; }
	}

	private Vector3 curOffset = Vector3.zero;

	private readonly DistanceScaledValues scaledValues = new DistanceScaledValues();

	public float Scale
	{
		get
		{
			return scaledValues.Scale;
		}
		set
		{
			scaledValues.Scale = value;
		}
	}

	public Vector3 CurOffset => curOffset;

	public Vector3 GetMovementOffset(Vector3 direction)
	{
		Vector3 vector = direction;
		vector.x *= scaledValues.OffsetMagnitudeX;
		vector.y *= scaledValues.OffsetMagnitudeY;
		Vector3 vector2 = vector - curOffset;
		Vector3 vector3 = vector2;
		vector3.Normalize();
		vector3.x *= scaledValues.OffsetSpeedX;
		vector3.y *= scaledValues.OffsetSpeedY;
		vector3 *= Time.deltaTime;
		if (Mathf.Sign(vector2.x - vector3.x) != Mathf.Sign(vector2.x))
		{
			curOffset.x = vector.x;
			vector3.x = 0f;
		}
		if (Mathf.Sign(vector2.y - vector3.y) != Mathf.Sign(vector2.y))
		{
			curOffset.y = vector.y;
			vector3.y = 0f;
		}
		curOffset += vector3;
		return curOffset;
	}

	public void Reset()
	{
		curOffset = Vector3.zero;
	}
}
