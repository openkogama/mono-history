using UnityEngine;

public static class MatrixMath
{
	public static Matrix4x4 MakeTransform(Vector3 position, Vector3 scale, Quaternion orientation)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		ToRotationMatrix(out var kRot, orientation);
		Matrix4x4 result = default;
		result[0, 0] = scale.x * kRot[0, 0];
		result[0, 1] = scale.y * kRot[0, 1];
		result[0, 2] = scale.z * kRot[0, 2];
		result[0, 3] = position.x;
		result[1, 0] = scale.x * kRot[1, 0];
		result[1, 1] = scale.y * kRot[1, 1];
		result[1, 2] = scale.z * kRot[1, 2];
		result[1, 3] = position.y;
		result[2, 0] = scale.x * kRot[2, 0];
		result[2, 1] = scale.y * kRot[2, 1];
		result[2, 2] = scale.z * kRot[2, 2];
		result[2, 3] = position.z;
		result[3, 0] = 0f;
		result[3, 1] = 0f;
		result[3, 2] = 0f;
		result[3, 3] = 1f;
		return result;
	}

	private static void ToRotationMatrix(out Matrix4x4 kRot, Quaternion q)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = -position;
		Vector3 val2 = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
		Quaternion val3 = QuaternionMath.Inverse(orientation);
		val = val3 * val;
		val.x *= val2.x;
		val.y *= val2.y;
		val.z *= val2.z;
		ToRotationMatrix(out var kRot, val3);
		Matrix4x4 result = default;
		result[0, 0] = val2.x * kRot[0, 0];
		result[0, 1] = val2.x * kRot[0, 1];
		result[0, 2] = val2.x * kRot[0, 2];
		result[0, 3] = val.x;
		result[1, 0] = val2.y * kRot[1, 0];
		result[1, 1] = val2.y * kRot[1, 1];
		result[1, 2] = val2.y * kRot[1, 2];
		result[1, 3] = val.y;
		result[2, 0] = val2.z * kRot[2, 0];
		result[2, 1] = val2.z * kRot[2, 1];
		result[2, 2] = val2.z * kRot[2, 2];
		result[2, 3] = val.z;
		result[3, 0] = 0f;
		result[3, 1] = 0f;
		result[3, 2] = 0f;
		result[3, 3] = 1f;
		return result;
	}

	public static Matrix4x4 Inverse(Matrix4x4 m)
	{
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
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
		float num28 = num23 * num27;
		float num29 = num24 * num27;
		float num30 = num25 * num27;
		float num31 = num26 * num27;
		float num32 = (0f - (num22 * num2 - num21 * num3 + num20 * num4)) * num27;
		float num33 = (num22 * num - num19 * num3 + num18 * num4) * num27;
		float num34 = (0f - (num21 * num - num19 * num2 + num17 * num4)) * num27;
		float num35 = (num20 * num - num18 * num2 + num17 * num3) * num27;
		num17 = num5 * num14 - num6 * num13;
		num18 = num5 * num15 - num7 * num13;
		num19 = num5 * num16 - num8 * num13;
		num20 = num6 * num15 - num7 * num14;
		num21 = num6 * num16 - num8 * num14;
		num22 = num7 * num16 - num8 * num15;
		float num36 = (num22 * num2 - num21 * num3 + num20 * num4) * num27;
		float num37 = (0f - (num22 * num - num19 * num3 + num18 * num4)) * num27;
		float num38 = (num21 * num - num19 * num2 + num17 * num4) * num27;
		float num39 = (0f - (num20 * num - num18 * num2 + num17 * num3)) * num27;
		num17 = num10 * num5 - num9 * num6;
		num18 = num11 * num5 - num9 * num7;
		num19 = num12 * num5 - num9 * num8;
		num20 = num11 * num6 - num10 * num7;
		num21 = num12 * num6 - num10 * num8;
		num22 = num12 * num7 - num11 * num8;
		float num40 = (0f - (num22 * num2 - num21 * num3 + num20 * num4)) * num27;
		float num41 = (num22 * num - num19 * num3 + num18 * num4) * num27;
		float num42 = (0f - (num21 * num - num19 * num2 + num17 * num4)) * num27;
		float num43 = (num20 * num - num18 * num2 + num17 * num3) * num27;
		Matrix4x4 result = default;
		result[0, 0] = num28;
		result[0, 1] = num32;
		result[0, 2] = num36;
		result[0, 3] = num40;
		result[1, 0] = num29;
		result[1, 1] = num33;
		result[1, 2] = num37;
		result[1, 3] = num41;
		result[2, 0] = num30;
		result[2, 1] = num34;
		result[2, 2] = num38;
		result[2, 3] = num42;
		result[3, 0] = num31;
		result[3, 1] = num35;
		result[3, 2] = num39;
		result[3, 3] = num43;
		return result;
	}
}
