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
		Mathf.Approximately(direction.magnitude, 0f);
		Vector3 vector = direction;
		Vector3 vector2 = vector;
		vector2.x *= scaledValues.OffsetMagnitudeX;
		vector2.y *= scaledValues.OffsetMagnitudeY;
		Vector3 vector3 = vector2 - curOffset;
		Vector3 vector4 = vector3;
		vector4.Normalize();
		vector4.x *= scaledValues.OffsetSpeedX;
		vector4.y *= scaledValues.OffsetSpeedY;
		vector4 *= Time.deltaTime;
		if (Mathf.Sign(vector3.x - vector4.x) != Mathf.Sign(vector3.x))
		{
			curOffset.x = vector2.x;
			vector4.x = 0f;
		}
		if (Mathf.Sign(vector3.y - vector4.y) != Mathf.Sign(vector3.y))
		{
			curOffset.y = vector2.y;
			vector4.y = 0f;
		}
		curOffset += vector4;
		return curOffset;
	}

	public void Reset()
	{
		curOffset = Vector3.zero;
	}
}
