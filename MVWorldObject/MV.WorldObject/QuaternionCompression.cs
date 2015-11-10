using System;
using UnityEngine;

namespace MV.WorldObject;

public static class QuaternionCompression
{
	private static float degreesToByteFactor = 32f / 45f;

	private static float byteToDegreeFactor = 1.40625f;

	private static float degToRad = 57.29578f;

	private static float radToDeg = (float)Math.PI / 180f;

	public static void Test()
	{
		byte[] eulerAnglesByteRange = ToBytes(default);
		ToQuaternion(eulerAnglesByteRange);
	}

	public static byte[] ToBytes(Quaternion quaternion)
	{
		Vector3 vector = ToEuler(quaternion);
		Vector3 vector2 = vector * degreesToByteFactor;
		return new byte[3]
		{
			(byte)vector2.x,
			(byte)vector2.y,
			(byte)vector2.z
		};
	}

	public static Quaternion ToQuaternion(byte[] eulerAnglesByteRange)
	{
		Vector3 eulerAngles = new Vector3((int)eulerAnglesByteRange[0], (int)eulerAnglesByteRange[1], (int)eulerAnglesByteRange[2]) * byteToDegreeFactor;
		return FromEuler(eulerAngles);
	}

	private static Vector3 ToEuler(Quaternion q1)
	{
		double num = q1.w * q1.w;
		double num2 = q1.x * q1.x;
		double num3 = q1.y * q1.y;
		double num4 = q1.z * q1.z;
		double num5 = num2 + num3 + num4 + num;
		double num6 = q1.x * q1.y + q1.z * q1.w;
		float x;
		float y;
		float z;
		if (num6 > 0.499 * num5)
		{
			x = (float)(2.0 * Math.Atan2(q1.x, q1.w));
			y = (float)Math.PI / 2f;
			z = 0f;
			return new Vector3(x, y, z);
		}
		if (num6 < -0.499 * num5)
		{
			x = (float)(-2.0 * Math.Atan2(q1.x, q1.w));
			y = -(float)Math.PI / 2f;
			z = 0f;
			return new Vector3(x, y, z);
		}
		x = (float)Math.Atan2(2.0 * (double)q1.y * (double)q1.w - 2.0 * (double)q1.x * (double)q1.z, num2 - num3 - num4 + num);
		y = (float)Math.Asin(2.0 * num6 / num5);
		z = (float)Math.Atan2(2.0 * (double)q1.x * (double)q1.w - (double)(2f * q1.y * q1.z), 0.0 - num2 + num3 - num4 + num);
		return new Vector3(x, y, z) * degToRad;
	}

	private static Quaternion FromEuler(Vector3 eulerAngles)
	{
		eulerAngles *= radToDeg;
		double num = eulerAngles.x;
		double num2 = eulerAngles.y;
		double num3 = eulerAngles.z;
		double num4 = Math.Cos(num / 2.0);
		double num5 = Math.Sin(num / 2.0);
		double num6 = Math.Cos(num2 / 2.0);
		double num7 = Math.Sin(num2 / 2.0);
		double num8 = Math.Cos(num3 / 2.0);
		double num9 = Math.Sin(num3 / 2.0);
		double num10 = num4 * num6;
		double num11 = num5 * num7;
		float w = (float)(num10 * num8 - num11 * num9);
		float x = (float)(num10 * num9 + num11 * num8);
		float y = (float)(num5 * num6 * num8 + num4 * num7 * num9);
		float z = (float)(num4 * num7 * num8 - num5 * num6 * num9);
		return new Quaternion(x, y, z, w);
	}
}
