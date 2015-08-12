using UnityEngine;

public static class MatrixMath
{
	public static Matrix4x4 MakeTransform(Vector3 position, Vector3 scale, Quaternion orientation)
	{
		ToRotationMatrix(out var kRot, orientation);
		return new Matrix4x4
		{
			[0, 0] = scale.x * kRot[0, 0],
			[0, 1] = scale.y * kRot[0, 1],
			[0, 2] = scale.z * kRot[0, 2],
			[0, 3] = position.x,
			[1, 0] = scale.x * kRot[1, 0],
			[1, 1] = scale.y * kRot[1, 1],
			[1, 2] = scale.z * kRot[1, 2],
			[1, 3] = position.y,
			[2, 0] = scale.x * kRot[2, 0],
			[2, 1] = scale.y * kRot[2, 1],
			[2, 2] = scale.z * kRot[2, 2],
			[2, 3] = position.z,
			[3, 0] = 0f,
			[3, 1] = 0f,
			[3, 2] = 0f,
			[3, 3] = 1f
		};
	}

	private static void ToRotationMatrix(out Matrix4x4 kRot, Quaternion q)
	{
		kRot = default;
		float num = q.x + q.x;
		float num2 = q.y + q.y;
		float num3 = q.z + q.z;
		float num4 = num * q.w;
		float num5 = num2 * q.w;
		float num6 = num3 * q.w;
		float num7 = num * q.x;
		float num8 = num2 * q.x;
		float num9 = num3 * q.x;
		float num10 = num2 * q.y;
		float num11 = num3 * q.y;
		float num12 = num3 * q.z;
		kRot[0, 0] = 1f - (num10 + num12);
		kRot[0, 1] = num8 - num6;
		kRot[0, 2] = num9 + num5;
		kRot[1, 0] = num8 + num6;
		kRot[1, 1] = 1f - (num7 + num12);
		kRot[1, 2] = num11 - num4;
		kRot[2, 0] = num9 - num5;
		kRot[2, 1] = num11 + num4;
		kRot[2, 2] = 1f - (num7 + num10);
	}

	public static Matrix4x4 MakeInverseTransform(Vector3 position, Vector3 scale, Quaternion orientation)
	{
		Vector3 vector = -position;
		Vector3 vector2 = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
		Quaternion quaternion = QuaternionMath.Inverse(orientation);
		vector = quaternion * vector;
		vector.x *= vector2.x;
		vector.y *= vector2.y;
		vector.z *= vector2.z;
		ToRotationMatrix(out var kRot, quaternion);
		return new Matrix4x4
		{
			[0, 0] = vector2.x * kRot[0, 0],
			[0, 1] = vector2.x * kRot[0, 1],
			[0, 2] = vector2.x * kRot[0, 2],
			[0, 3] = vector.x,
			[1, 0] = vector2.y * kRot[1, 0],
			[1, 1] = vector2.y * kRot[1, 1],
			[1, 2] = vector2.y * kRot[1, 2],
			[1, 3] = vector.y,
			[2, 0] = vector2.z * kRot[2, 0],
			[2, 1] = vector2.z * kRot[2, 1],
			[2, 2] = vector2.z * kRot[2, 2],
			[2, 3] = vector.z,
			[3, 0] = 0f,
			[3, 1] = 0f,
			[3, 2] = 0f,
			[3, 3] = 1f
		};
	}

	public static Matrix4x4 Inverse(Matrix4x4 m)
	{
		float num = m[0, 0];
		float num2 = m[0, 1];
		float num3 = m[0, 2];
		float num4 = m[0, 3];
		float num5 = m[1, 0];
		float num6 = m[1, 1];
		float num7 = m[1, 2];
		float num8 = m[1, 3];
		float num9 = m[2, 0];
		float num10 = m[2, 1];
		float num11 = m[2, 2];
		float num12 = m[2, 3];
		float num13 = m[3, 0];
		float num14 = m[3, 1];
		float num15 = m[3, 2];
		float num16 = m[3, 3];
		float num17 = num9 * num14 - num10 * num13;
		float num18 = num9 * num15 - num11 * num13;
		float num19 = num9 * num16 - num12 * num13;
		float num20 = num10 * num15 - num11 * num14;
		float num21 = num10 * num16 - num12 * num14;
		float num22 = num11 * num16 - num12 * num15;
		float num23 = num22 * num6 - num21 * num7 + num20 * num8;
		float num24 = 0f - (num22 * num5 - num19 * num7 + num18 * num8);
		float num25 = num21 * num5 - num19 * num6 + num17 * num8;
		float num26 = 0f - (num20 * num5 - num18 * num6 + num17 * num7);
		float num27 = 1f / (num23 * num + num24 * num2 + num25 * num3 + num26 * num4);
		float value = num23 * num27;
		float value2 = num24 * num27;
		float value3 = num25 * num27;
		float value4 = num26 * num27;
		float value5 = (0f - (num22 * num2 - num21 * num3 + num20 * num4)) * num27;
		float value6 = (num22 * num - num19 * num3 + num18 * num4) * num27;
		float value7 = (0f - (num21 * num - num19 * num2 + num17 * num4)) * num27;
		float value8 = (num20 * num - num18 * num2 + num17 * num3) * num27;
		num17 = num5 * num14 - num6 * num13;
		num18 = num5 * num15 - num7 * num13;
		num19 = num5 * num16 - num8 * num13;
		num20 = num6 * num15 - num7 * num14;
		num21 = num6 * num16 - num8 * num14;
		num22 = num7 * num16 - num8 * num15;
		float value9 = (num22 * num2 - num21 * num3 + num20 * num4) * num27;
		float value10 = (0f - (num22 * num - num19 * num3 + num18 * num4)) * num27;
		float value11 = (num21 * num - num19 * num2 + num17 * num4) * num27;
		float value12 = (0f - (num20 * num - num18 * num2 + num17 * num3)) * num27;
		num17 = num10 * num5 - num9 * num6;
		num18 = num11 * num5 - num9 * num7;
		num19 = num12 * num5 - num9 * num8;
		num20 = num11 * num6 - num10 * num7;
		num21 = num12 * num6 - num10 * num8;
		num22 = num12 * num7 - num11 * num8;
		float value13 = (0f - (num22 * num2 - num21 * num3 + num20 * num4)) * num27;
		float value14 = (num22 * num - num19 * num3 + num18 * num4) * num27;
		float value15 = (0f - (num21 * num - num19 * num2 + num17 * num4)) * num27;
		float value16 = (num20 * num - num18 * num2 + num17 * num3) * num27;
		return new Matrix4x4
		{
			[0, 0] = value,
			[0, 1] = value5,
			[0, 2] = value9,
			[0, 3] = value13,
			[1, 0] = value2,
			[1, 1] = value6,
			[1, 2] = value10,
			[1, 3] = value14,
			[2, 0] = value3,
			[2, 1] = value7,
			[2, 2] = value11,
			[2, 3] = value15,
			[3, 0] = value4,
			[3, 1] = value8,
			[3, 2] = value12,
			[3, 3] = value16
		};
	}
}
