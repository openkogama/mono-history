using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public static class MathFunctions
{
	public enum IntersectResult
	{
		PARALLEL,
		COINCIDENT,
		NOT_INTERESECTING,
		INTERESECTING
	}

	public static class PerlinSimplexNoise
	{
		private static int[][] grad3;

		private static int[] p;

		private static int[] perm;

		static PerlinSimplexNoise()
		{
			grad3 = new int[12][]
			{
				new int[3] { 1, 1, 0 },
				new int[3] { -1, 1, 0 },
				new int[3] { 1, -1, 0 },
				new int[3] { -1, -1, 0 },
				new int[3] { 1, 0, 1 },
				new int[3] { -1, 0, 1 },
				new int[3] { 1, 0, -1 },
				new int[3] { -1, 0, -1 },
				new int[3] { 0, 1, 1 },
				new int[3] { 0, -1, 1 },
				new int[3] { 0, 1, -1 },
				new int[3] { 0, -1, -1 }
			};
			p = new int[256]
			{
				151, 160, 137, 91, 90, 15, 131, 13, 201, 95,
				96, 53, 194, 233, 7, 225, 140, 36, 103, 30,
				69, 142, 8, 99, 37, 240, 21, 10, 23, 190,
				6, 148, 247, 120, 234, 75, 0, 26, 197, 62,
				94, 252, 219, 203, 117, 35, 11, 32, 57, 177,
				33, 88, 237, 149, 56, 87, 174, 20, 125, 136,
				171, 168, 68, 175, 74, 165, 71, 134, 139, 48,
				27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
				60, 211, 133, 230, 220, 105, 92, 41, 55, 46,
				245, 40, 244, 102, 143, 54, 65, 25, 63, 161,
				1, 216, 80, 73, 209, 76, 132, 187, 208, 89,
				18, 169, 200, 196, 135, 130, 116, 188, 159, 86,
				164, 100, 109, 198, 173, 186, 3, 64, 52, 217,
				226, 250, 124, 123, 5, 202, 38, 147, 118, 126,
				255, 82, 85, 212, 207, 206, 59, 227, 47, 16,
				58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
				119, 248, 152, 2, 44, 154, 163, 70, 221, 153,
				101, 155, 167, 43, 172, 9, 129, 22, 39, 253,
				19, 98, 108, 110, 79, 113, 224, 232, 178, 185,
				112, 104, 218, 246, 97, 228, 251, 34, 242, 193,
				238, 210, 144, 12, 191, 179, 162, 241, 81, 51,
				145, 235, 249, 14, 239, 107, 49, 192, 214, 31,
				181, 199, 106, 157, 184, 84, 204, 176, 115, 121,
				50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
				222, 114, 67, 29, 24, 72, 243, 141, 128, 195,
				78, 66, 215, 61, 156, 180
			};
			perm = new int[512];
			for (int i = 0; i < 512; i++)
			{
				perm[i] = p[i & 0xFF];
			}
		}

		private static int fastfloor(float x)
		{
			return (!(x > 0f)) ? ((int)x - 1) : ((int)x);
		}

		private static float dot(int[] g, float x, float y)
		{
			return (float)g[0] * x + (float)g[1] * y;
		}

		private static float dot(int[] g, float x, float y, float z)
		{
			return (float)g[0] * x + (float)g[1] * y + (float)g[2] * z;
		}

		private static float dot(int[] g, float x, float y, float z, float w)
		{
			return (float)g[0] * x + (float)g[1] * y + (float)g[2] * z + (float)g[3] * w;
		}

		public static float noise(float xin, float yin, float zin)
		{
			float num = 1f / 3f;
			float num2 = (xin + yin + zin) * num;
			int num3 = fastfloor(xin + num2);
			int num4 = fastfloor(yin + num2);
			int num5 = fastfloor(zin + num2);
			float num6 = 1f / 6f;
			float num7 = (float)(num3 + num4 + num5) * num6;
			float num8 = (float)num3 - num7;
			float num9 = (float)num4 - num7;
			float num10 = (float)num5 - num7;
			float num11 = xin - num8;
			float num12 = yin - num9;
			float num13 = zin - num10;
			int num14;
			int num15;
			int num16;
			int num17;
			int num18;
			int num19;
			if (num11 >= num12)
			{
				if (num12 >= num13)
				{
					num14 = 1;
					num15 = 0;
					num16 = 0;
					num17 = 1;
					num18 = 1;
					num19 = 0;
				}
				else if (num11 >= num13)
				{
					num14 = 1;
					num15 = 0;
					num16 = 0;
					num17 = 1;
					num18 = 0;
					num19 = 1;
				}
				else
				{
					num14 = 0;
					num15 = 0;
					num16 = 1;
					num17 = 1;
					num18 = 0;
					num19 = 1;
				}
			}
			else if (num12 < num13)
			{
				num14 = 0;
				num15 = 0;
				num16 = 1;
				num17 = 0;
				num18 = 1;
				num19 = 1;
			}
			else if (num11 < num13)
			{
				num14 = 0;
				num15 = 1;
				num16 = 0;
				num17 = 0;
				num18 = 1;
				num19 = 1;
			}
			else
			{
				num14 = 0;
				num15 = 1;
				num16 = 0;
				num17 = 1;
				num18 = 1;
				num19 = 0;
			}
			float num20 = num11 - (float)num14 + num6;
			float num21 = num12 - (float)num15 + num6;
			float num22 = num13 - (float)num16 + num6;
			float num23 = num11 - (float)num17 + 2f * num6;
			float num24 = num12 - (float)num18 + 2f * num6;
			float num25 = num13 - (float)num19 + 2f * num6;
			float num26 = num11 - 1f + 3f * num6;
			float num27 = num12 - 1f + 3f * num6;
			float num28 = num13 - 1f + 3f * num6;
			int num29 = num3 & 0xFF;
			int num30 = num4 & 0xFF;
			int num31 = num5 & 0xFF;
			int num32 = perm[num29 + perm[num30 + perm[num31]]] % 12;
			int num33 = perm[num29 + num14 + perm[num30 + num15 + perm[num31 + num16]]] % 12;
			int num34 = perm[num29 + num17 + perm[num30 + num18 + perm[num31 + num19]]] % 12;
			int num35 = perm[num29 + 1 + perm[num30 + 1 + perm[num31 + 1]]] % 12;
			float num36 = 0.6f - num11 * num11 - num12 * num12 - num13 * num13;
			float num37;
			if (num36 < 0f)
			{
				num37 = 0f;
			}
			else
			{
				num36 *= num36;
				num37 = num36 * num36 * dot(grad3[num32], num11, num12, num13);
			}
			float num38 = 0.6f - num20 * num20 - num21 * num21 - num22 * num22;
			float num39;
			if (num38 < 0f)
			{
				num39 = 0f;
			}
			else
			{
				num38 *= num38;
				num39 = num38 * num38 * dot(grad3[num33], num20, num21, num22);
			}
			float num40 = 0.6f - num23 * num23 - num24 * num24 - num25 * num25;
			float num41;
			if (num40 < 0f)
			{
				num41 = 0f;
			}
			else
			{
				num40 *= num40;
				num41 = num40 * num40 * dot(grad3[num34], num23, num24, num25);
			}
			float num42 = 0.6f - num26 * num26 - num27 * num27 - num28 * num28;
			float num43;
			if (num42 < 0f)
			{
				num43 = 0f;
			}
			else
			{
				num42 *= num42;
				num43 = num42 * num42 * dot(grad3[num35], num26, num27, num28);
			}
			return 32f * (num37 + num39 + num41 + num43);
		}

		public static float noise(float xin, float yin)
		{
			float num = (float)(0.5 * (Math.Sqrt(3.0) - 1.0));
			float num2 = (xin + yin) * num;
			int num3 = fastfloor(xin + num2);
			int num4 = fastfloor(yin + num2);
			float num5 = (float)((3.0 - Math.Sqrt(3.0)) / 6.0);
			float num6 = (float)(num3 + num4) * num5;
			float num7 = (float)num3 - num6;
			float num8 = (float)num4 - num6;
			float num9 = xin - num7;
			float num10 = yin - num8;
			int num11;
			int num12;
			if (num9 > num10)
			{
				num11 = 1;
				num12 = 0;
			}
			else
			{
				num11 = 0;
				num12 = 1;
			}
			float num13 = num9 - (float)num11 + num5;
			float num14 = num10 - (float)num12 + num5;
			float num15 = num9 - 1f + 2f * num5;
			float num16 = num10 - 1f + 2f * num5;
			int num17 = num3 & 0xFF;
			int num18 = num4 & 0xFF;
			int num19 = perm[num17 + perm[num18]] % 12;
			int num20 = perm[num17 + num11 + perm[num18 + num12]] % 12;
			int num21 = perm[num17 + 1 + perm[num18 + 1]] % 12;
			float num22 = 0.5f - num9 * num9 - num10 * num10;
			float num23;
			if (num22 < 0f)
			{
				num23 = 0f;
			}
			else
			{
				num22 *= num22;
				num23 = num22 * num22 * dot(grad3[num19], num9, num10);
			}
			float num24 = 0.5f - num13 * num13 - num14 * num14;
			float num25;
			if (num24 < 0f)
			{
				num25 = 0f;
			}
			else
			{
				num24 *= num24;
				num25 = num24 * num24 * dot(grad3[num20], num13, num14);
			}
			float num26 = 0.5f - num15 * num15 - num16 * num16;
			float num27;
			if (num26 < 0f)
			{
				num27 = 0f;
			}
			else
			{
				num26 *= num26;
				num27 = num26 * num26 * dot(grad3[num21], num15, num16);
			}
			float num28 = 70f * (num23 + num25 + num27);
			return (num28 + 1f) * 0.5f;
		}
	}

	public static float DotProduct(ref Vector3 a, ref Vector3 b)
	{
		return a.x * b.x + a.y * b.y + a.z * b.z;
	}

	public static Vector3 Multiply(this Vector3 a, Vector3 b)
	{
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	public static Vector3 GetMinVector(Vector3 min0, Vector3 min1)
	{
		Vector3 result = default;
		for (int i = 0; i < 3; i++)
		{
			if (min0[i] < min1[i])
			{
				result[i] = min0[i];
			}
			else
			{
				result[i] = min1[i];
			}
		}
		return result;
	}

	public static Vector3 GetMaxVector(Vector3 max0, Vector3 max1)
	{
		Vector3 result = default;
		for (int i = 0; i < 3; i++)
		{
			if (max0[i] > max1[i])
			{
				result[i] = max0[i];
			}
			else
			{
				result[i] = max1[i];
			}
		}
		return result;
	}

	public static double SignedDistanceTo(ref Plane plane, ref Vector3 planeOrigin, ref Vector3 point)
	{
		return Vector3.Dot(plane.normal, point) + (0f - (plane.normal.x * planeOrigin.x + plane.normal.y * planeOrigin.y + plane.normal.z * planeOrigin.z));
	}

	public static double SignedDistanceTo(ref Vector3 planeNormal, ref Vector3 planeOrigin, ref Vector3 point)
	{
		return Vector3.Dot(planeNormal, point) + (0f - (planeNormal.x * planeOrigin.x + planeNormal.y * planeOrigin.y + planeNormal.z * planeOrigin.z));
	}

	public static void ClampIntVector(ref IntVector target, IntVector min, IntVector max)
	{
		for (int i = 0; i < 3; i++)
		{
			target[i] = (short)Mathf.Clamp(target[i], min[i], max[i]);
		}
	}

	public static Vector3 MultiplyVector(ref Vector3 vec0, ref Vector3 vec1)
	{
		return new Vector3(vec0.x * vec1.x, vec0.y * vec1.y, vec0.z * vec1.z);
	}

	public static Vector3 DivideVector(ref Vector3 vec0, ref Vector3 vec1)
	{
		return new Vector3(vec0.x / vec1.x, vec0.y / vec1.y, vec0.z / vec1.z);
	}

	public static Vector3 AbsVector(Vector3 vec)
	{
		return new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
	}

	public static Vector2 xy(this Vector3 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static Vector3 RoundVector(Vector3 vector, int decimals)
	{
		vector.x = (float)Math.Round(vector.x, decimals);
		vector.y = (float)Math.Round(vector.y, decimals);
		vector.z = (float)Math.Round(vector.z, decimals);
		return vector;
	}

	public static Vector3 FloorVector(Vector3 vector)
	{
		vector.x = (float)Math.Floor(vector.x);
		vector.y = (float)Math.Floor(vector.y);
		vector.z = (float)Math.Floor(vector.z);
		return vector;
	}

	public static Vector3 CeilVector(Vector3 vector)
	{
		vector.x = Mathf.Ceil(vector.x);
		vector.y = Mathf.Ceil(vector.y);
		vector.z = Mathf.Ceil(vector.z);
		return vector;
	}

	public static bool VectorIsNan(Vector3 vector)
	{
		if (float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z))
		{
			return true;
		}
		return false;
	}

	public static bool QuaternionIsNan(Quaternion quaternion)
	{
		if (float.IsNaN(quaternion.x) || float.IsNaN(quaternion.y) || float.IsNaN(quaternion.z) || float.IsNaN(quaternion.w))
		{
			return true;
		}
		return false;
	}

	public static bool VectorIsFinite(Vector3 vector)
	{
		if (float.IsInfinity(vector.x) || float.IsInfinity(vector.y) || float.IsInfinity(vector.z))
		{
			return false;
		}
		return true;
	}

	public static bool QuaternionIsFinite(Quaternion quaternion)
	{
		if (float.IsInfinity(quaternion.x) || float.IsInfinity(quaternion.y) || float.IsInfinity(quaternion.z) || float.IsInfinity(quaternion.w))
		{
			return false;
		}
		return true;
	}

	public static bool IsQuaternionFloatsValid(Quaternion quaternion)
	{
		return !QuaternionIsNan(quaternion) && QuaternionIsFinite(quaternion);
	}

	public static bool IsVectorFloatsValid(Vector3 vector)
	{
		return !VectorIsNan(vector) && VectorIsFinite(vector);
	}

	public static bool IsFloatValid(float floatToValidate)
	{
		return !float.IsInfinity(floatToValidate) && !float.IsNaN(floatToValidate);
	}

	public static Vector3 TruncateVector(Vector3 vector, int digits)
	{
		vector.x = (float)Truncate(vector.x, digits);
		vector.y = (float)Truncate(vector.y, digits);
		vector.z = (float)Truncate(vector.z, digits);
		return vector;
	}

	public static double Truncate(double number, int digits)
	{
		double num = Math.Pow(10.0, digits);
		int num2 = (int)(num * number);
		return (double)num2 / num;
	}

	public static IntVector ToIntVector(this Vector3 v)
	{
		return new IntVector((short)v.x, (short)v.y, (short)v.z);
	}

	public static void Vector3ToVector2(ref Vector3 from, ref Vector2 to, int ignoreAxis)
	{
		if (ignoreAxis == 2)
		{
			to = from;
		}
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			if (i != ignoreAxis)
			{
				to[num] = from[i];
				num++;
			}
		}
	}

	public static void Vector2ToVector3(ref Vector2 from, ref Vector3 to, int addAxis, float addValue = 0f)
	{
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			if (i == addAxis)
			{
				to[i] = addValue;
				continue;
			}
			to[i] = from[num];
			num++;
		}
		if (addAxis == 2)
		{
			to[addAxis] = addValue;
		}
	}

	public static bool DoLinesIntersect(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, ref Vector2 ptIntersection)
	{
		double num = (p3.y - p2.y) * (p1.x - p0.x) - (p3.x - p2.x) * (p1.y - p0.y);
		double num2 = (p3.x - p2.x) * (p0.y - p2.y) - (p3.y - p2.y) * (p0.x - p2.x);
		double num3 = (p1.x - p0.x) * (p0.y - p2.y) - (p1.y - p0.y) * (p0.x - p2.x);
		if (Math.Abs(num2) < 0.01 && Math.Abs(num3) < 0.01)
		{
			if (IsCoincidentalLineSegmentsOverlapping(p0, p1, p2, p3))
			{
				return true;
			}
			return false;
		}
		if (num == 0.0)
		{
			return false;
		}
		double num4 = num2 / num;
		double num5 = num3 / num;
		if (num4 >= 0.0 && num4 <= 1.0 && num5 >= 0.0 && num5 <= 1.0)
		{
			ptIntersection.x = (float)((double)p0.x + num4 * (double)(p1.x - p0.x));
			ptIntersection.y = (float)((double)p0.y + num4 * (double)(p1.y - p0.y));
			return true;
		}
		return false;
	}

	public static bool IsCoincidentalLineSegmentsOverlapping(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
	{
		float[] array = new float[4]
		{
			(p0 - p2).sqrMagnitude,
			(p0 - p3).sqrMagnitude,
			(p1 - p2).sqrMagnitude,
			(p1 - p3).sqrMagnitude
		};
		float num = 0f;
		float[] array2 = array;
		foreach (float num2 in array2)
		{
			if (num2 > num)
			{
				num = num2;
			}
		}
		if ((p0 - p1).sqrMagnitude + (p2 - p3).sqrMagnitude <= num)
		{
			return false;
		}
		return true;
	}

	public static IntersectResult Intersect(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, ref Vector2 ptIntersection)
	{
		float num = (p3.y - p2.y) * (p1.x - p0.x) - (p3.x - p2.x) * (p1.y - p0.y);
		float num2 = (p3.x - p2.x) * (p0.y - p2.y) - (p3.y - p2.y) * (p0.x - p2.x);
		float num3 = (p1.x - p0.x) * (p0.y - p2.y) - (p1.y - p0.y) * (p0.x - p2.x);
		if (num == 0f)
		{
			if (num2 == 0f && num3 == 0f)
			{
				return IntersectResult.COINCIDENT;
			}
			return IntersectResult.PARALLEL;
		}
		float num4 = num2 / num;
		float num5 = num3 / num;
		if (num4 >= 0f && num4 <= 1f && num5 >= 0f && num5 <= 1f)
		{
			ptIntersection.x = p0.x + num4 * (p1.x - p0.x);
			ptIntersection.y = p0.y + num4 * (p1.y - p0.y);
			return IntersectResult.INTERESECTING;
		}
		return IntersectResult.NOT_INTERESECTING;
	}

	public static bool IsPointInShape(IList<Vector2> shapePoints, Vector2 point)
	{
		int num = 0;
		bool flag = false;
		int num2 = 0;
		num = shapePoints.Count - 1;
		while (num2 < shapePoints.Count)
		{
			if (((shapePoints[num2].y <= point.y && point.y < shapePoints[num].y) || (shapePoints[num].y <= point.y && point.y < shapePoints[num2].y)) && point.x < (shapePoints[num].x - shapePoints[num2].x) * (point.y - shapePoints[num2].y) / (shapePoints[num].y - shapePoints[num2].y) + shapePoints[num2].x)
			{
				flag = !flag;
			}
			num = num2++;
		}
		return flag;
	}

	public static int IsLineSegmentIntersectingShape(Vector2 p0, Vector2 p1, List<Vector2> points, bool isOpen)
	{
		int num = points.Count;
		if (isOpen)
		{
			num--;
		}
		Vector2 ptIntersection = new Vector2(-1f, -1f);
		for (int i = 0; i < num; i++)
		{
			if (DoLinesIntersect(p0, p1, points[i % points.Count], points[(i + 1) % points.Count], ref ptIntersection) && ((ptIntersection.x == -1f && ptIntersection.y == -1f) || (double)(ptIntersection - points[points.Count - 1]).SqrMagnitude() > 0.01))
			{
				return i;
			}
		}
		return -1;
	}

	public static float SignedAngle(Vector2 v1, Vector2 v2)
	{
		float num = v1.x * v2.y - v1.y * v2.x;
		return (float)Math.Atan2(num, Vector2.Dot(v1, v2));
	}

	public static float SignedAngle(Vector3 v1, Vector3 v2, Vector3 normal)
	{
		return Mathf.Atan2(Vector3.Dot(normal, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2));
	}

	public static float Yaw(Vector3 dir)
	{
		return 57.29578f * (SignedAngle(Vector3.forward, -dir, Vector3.up) + (float)Math.PI);
	}

	public static float Pitch(Vector3 dir, Vector3 planeNormal)
	{
		return 57.29578f * (SignedAngle(Vector3.up, -dir, planeNormal) + (float)Math.PI);
	}

	public static float SignedYawFromLocalDirection(Vector3 localDirection)
	{
		Vector3 normalized = new Vector3(localDirection.x, 0f, localDirection.z).normalized;
		float num = Vector3.Angle(Vector3.forward, normalized);
		float num2 = Vector3.Dot(normalized, Vector3.right);
		if (num2 < 0f)
		{
			num = 0f - num;
		}
		return num;
	}

	public static float PitchFromLocalDirection(Vector3 localDirection)
	{
		return Vector3.Angle(Vector3.up, localDirection) - 90f;
	}

	public static Quaternion QuaternionFromAngleAndAxis(float angle, Vector3 AxisVector)
	{
		return Quaternion.AngleAxis(angle, AxisVector);
	}

	public static List<Vector2> FlipPolygon(List<Vector2> points)
	{
		List<Vector2> list = new List<Vector2>(points);
		for (int i = 0; i < points.Count / 2; i++)
		{
			Vector2 value = list[i];
			list[i] = list[list.Count - 1 - i];
			list[list.Count - 1 - i] = value;
		}
		return list;
	}

	public static void ClampVector(ref Vector3 v, float min, float max)
	{
		v.x = Mathf.Clamp(v.x, min, max);
		v.y = Mathf.Clamp(v.y, min, max);
		v.z = Mathf.Clamp(v.z, min, max);
	}

	public static void ClampVector(ref Vector3 v, Vector3 min, Vector3 max)
	{
		v.x = Mathf.Clamp(v.x, min.x, max.x);
		v.y = Mathf.Clamp(v.y, min.y, max.y);
		v.z = Mathf.Clamp(v.z, min.z, max.z);
	}

	public static bool DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd, ref float distance)
	{
		Vector3 intersection = default;
		return DistancePointLine(point, lineStart, lineEnd, ref distance, ref intersection);
	}

	public static bool DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd, ref float distance, ref Vector3 intersection)
	{
		intersection = default;
		float magnitude = (lineEnd - lineStart).magnitude;
		float num = ((point.x - lineStart.x) * (lineEnd.x - lineStart.x) + (point.y - lineStart.y) * (lineEnd.y - lineStart.y) + (point.z - lineStart.z) * (lineEnd.z - lineStart.z)) / (magnitude * magnitude);
		if (num < 0f || num > 1f)
		{
			return false;
		}
		intersection = lineStart + num * (lineEnd - lineStart);
		distance = (point - intersection).magnitude;
		return true;
	}

	public static void DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd, out float distance, out Vector3 intersection, out float u)
	{
		float magnitude = (lineEnd - lineStart).magnitude;
		u = ((point.x - lineStart.x) * (lineEnd.x - lineStart.x) + (point.y - lineStart.y) * (lineEnd.y - lineStart.y) + (point.z - lineStart.z) * (lineEnd.z - lineStart.z)) / (magnitude * magnitude);
		intersection = lineStart + u * (lineEnd - lineStart);
		distance = (point - intersection).magnitude;
	}

	public static Vector3 GetNormal(Vector3 pa, Vector3 pb, Vector3 pc)
	{
		return new Vector3((pb.y - pa.y) * (pc.z - pa.z) - (pb.z - pa.z) * (pc.y - pa.y), (pb.z - pa.z) * (pc.x - pa.x) - (pb.x - pa.x) * (pc.z - pa.z), (pb.x - pa.x) * (pc.y - pa.y) - (pb.y - pa.y) * (pc.x - pa.x)).normalized;
	}

	public static bool LineFacetCollision(Vector3 p1, Vector3 p2, Vector3 pa, Vector3 pb, Vector3 pc, Vector3 lineDir, ref Vector3 p, ref Vector3 n)
	{
		n = GetNormal(pa, pb, pc);
		if (Vector3.Dot(n, lineDir) > 0f)
		{
			return false;
		}
		float num = (0f - n.x) * pa.x - n.y * pa.y - n.z * pa.z;
		float num2 = n.x * (p2.x - p1.x) + n.y * (p2.y - p1.y) + n.z * (p2.z - p1.z);
		if (Mathf.Abs(num2) < Mathf.Epsilon)
		{
			return false;
		}
		float num3 = (0f - (num + n.x * p1.x + n.y * p1.y + n.z * p1.z)) / num2;
		p.x = p1.x + num3 * (p2.x - p1.x);
		p.y = p1.y + num3 * (p2.y - p1.y);
		p.z = p1.z + num3 * (p2.z - p1.z);
		if (num3 < 0f || num3 > 1f)
		{
			return false;
		}
		Vector3 vector = default;
		vector.x = pa.x - p.x;
		vector.y = pa.y - p.y;
		vector.z = pa.z - p.z;
		vector.Normalize();
		Vector3 vector2 = default;
		vector2.x = pb.x - p.x;
		vector2.y = pb.y - p.y;
		vector2.z = pb.z - p.z;
		vector2.Normalize();
		Vector3 vector3 = default;
		vector3.x = pc.x - p.x;
		vector3.y = pc.y - p.y;
		vector3.z = pc.z - p.z;
		vector3.Normalize();
		float f = vector.x * vector2.x + vector.y * vector2.y + vector.z * vector2.z;
		float f2 = vector2.x * vector3.x + vector2.y * vector3.y + vector2.z * vector3.z;
		float f3 = vector3.x * vector.x + vector3.y * vector.y + vector3.z * vector.z;
		float num4 = (Mathf.Acos(f) + Mathf.Acos(f2) + Mathf.Acos(f3)) * 57.29578f;
		if (Mathf.Abs(num4 - 360f) > 0.1f)
		{
			return false;
		}
		return true;
	}

	public static bool LineFacet(Vector3 p1, Vector3 p2, Vector3 pa, Vector3 pb, Vector3 pc, ref Vector3 p)
	{
		Vector3 vector = default;
		vector.x = (pb.y - pa.y) * (pc.z - pa.z) - (pb.z - pa.z) * (pc.y - pa.y);
		vector.y = (pb.z - pa.z) * (pc.x - pa.x) - (pb.x - pa.x) * (pc.z - pa.z);
		vector.z = (pb.x - pa.x) * (pc.y - pa.y) - (pb.y - pa.y) * (pc.x - pa.x);
		vector.Normalize();
		float num = (0f - vector.x) * pa.x - vector.y * pa.y - vector.z * pa.z;
		float num2 = vector.x * (p2.x - p1.x) + vector.y * (p2.y - p1.y) + vector.z * (p2.z - p1.z);
		if (Mathf.Abs(num2) < Mathf.Epsilon)
		{
			return false;
		}
		float num3 = (0f - (num + vector.x * p1.x + vector.y * p1.y + vector.z * p1.z)) / num2;
		p.x = p1.x + num3 * (p2.x - p1.x);
		p.y = p1.y + num3 * (p2.y - p1.y);
		p.z = p1.z + num3 * (p2.z - p1.z);
		if (num3 < 0f || num3 > 1f)
		{
			return false;
		}
		Vector3 vector2 = default;
		vector2.x = pa.x - p.x;
		vector2.y = pa.y - p.y;
		vector2.z = pa.z - p.z;
		vector2.Normalize();
		Vector3 vector3 = default;
		vector3.x = pb.x - p.x;
		vector3.y = pb.y - p.y;
		vector3.z = pb.z - p.z;
		vector3.Normalize();
		Vector3 vector4 = default;
		vector4.x = pc.x - p.x;
		vector4.y = pc.y - p.y;
		vector4.z = pc.z - p.z;
		vector4.Normalize();
		float f = vector2.x * vector3.x + vector2.y * vector3.y + vector2.z * vector3.z;
		float f2 = vector3.x * vector4.x + vector3.y * vector4.y + vector3.z * vector4.z;
		float f3 = vector4.x * vector2.x + vector4.y * vector2.y + vector4.z * vector2.z;
		float num4 = (Mathf.Acos(f) + Mathf.Acos(f2) + Mathf.Acos(f3)) * 57.29578f;
		if (Mathf.Abs(num4 - 360f) > 0.1f)
		{
			return false;
		}
		return true;
	}

	public static float NormalizeAngle(float degrees)
	{
		degrees %= 360f;
		if (degrees < 0f)
		{
			degrees += 360f;
		}
		return degrees;
	}

	public static Quaternion InertiaY(Vector3 eulerFrom, Vector3 eulerTo, float speed)
	{
		eulerFrom.x = 0f;
		eulerFrom.z = 0f;
		Quaternion a = Quaternion.Euler(eulerFrom);
		eulerFrom.y = eulerTo.y;
		Quaternion b = Quaternion.Euler(eulerFrom);
		return Quaternion.Slerp(a, b, Time.deltaTime * speed);
	}

	public static Quaternion InertiaX(Vector3 eulerFrom, Vector3 eulerTo, float speed)
	{
		eulerFrom.y = 0f;
		eulerFrom.z = 0f;
		Quaternion a = Quaternion.Euler(eulerFrom);
		eulerFrom.x = eulerTo.x;
		Quaternion b = Quaternion.Euler(eulerFrom);
		return Quaternion.Slerp(a, b, Time.deltaTime * speed);
	}

	public static float Pow2(float val)
	{
		return val * val;
	}

	public static Matrix4x4 AbsMatrix(Matrix4x4 m)
	{
		Matrix4x4 result = default;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				result[j, i] = Mathf.Abs(m[j, i]);
			}
		}
		return result;
	}

	public static Bounds FastAABBTransform(Matrix4x4 m, Bounds b)
	{
		Matrix4x4 matrix4x = AbsMatrix(m);
		Vector3 center = m.MultiplyPoint(b.center);
		Vector3 vector = matrix4x.MultiplyVector(b.extents);
		return new Bounds(center, 2f * vector);
	}
}
