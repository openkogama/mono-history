using System;
using UnityEngine;

public static class TriangleCheck
{
	private static bool GetLowestRoot(float a, float b, float c, float maxR, ref float root)
	{
		if (a == 0f)
		{
			return false;
		}
		float num = b * b - 4f * a * c;
		if (num < 0f)
		{
			return false;
		}
		float num2 = Mathf.Sqrt(num);
		float num3 = (0f - b - num2) / (2f * a);
		float num4 = (0f - b + num2) / (2f * a);
		if (num3 > num4)
		{
			float num5 = num4;
			num4 = num3;
			num3 = num5;
		}
		if (num3 > 0f && num3 < maxR)
		{
			root = num3;
			return true;
		}
		if (num4 > 0f && num4 < maxR)
		{
			root = num4;
			return true;
		}
		return false;
	}

	private static bool SameSide(ref Vector3 p1, ref Vector3 p2, ref Vector3 a, ref Vector3 b)
	{
		Vector3 lhs = default;
		Vector3 rhs = default;
		lhs.x = b.x - a.x;
		lhs.y = b.y - a.y;
		lhs.z = b.z - a.z;
		rhs.x = p1.x - a.x;
		rhs.y = p1.y - a.y;
		rhs.z = p1.z - a.z;
		Vector3 a2 = Vector3.Cross(lhs, rhs);
		rhs.x = p2.x - a.x;
		rhs.y = p2.y - a.y;
		rhs.z = p2.z - a.z;
		Vector3 b2 = Vector3.Cross(lhs, rhs);
		if (MathFunctions.DotProduct(ref a2, ref b2) >= 0f)
		{
			return true;
		}
		return false;
	}

	private static bool PointInTriangle(ref Vector3 p, ref Vector3 a, ref Vector3 b, ref Vector3 c)
	{
		if (SameSide(ref p, ref a, ref b, ref c) && SameSide(ref p, ref b, ref a, ref c) && SameSide(ref p, ref c, ref a, ref b))
		{
			return true;
		}
		return false;
	}

	private static bool CheckPointInTriangle(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
	{
		float num = 0f;
		Vector3 a2 = default;
		Vector3 b2 = default;
		Vector3 b3 = default;
		a2.x = point.x - a.x;
		a2.y = point.y - a.y;
		a2.z = point.z - a.z;
		b2.x = point.x - b.x;
		b2.y = point.y - b.y;
		b2.z = point.z - b.z;
		b3.x = point.x - c.x;
		b3.y = point.y - c.y;
		b3.z = point.z - c.z;
		a2.Normalize();
		b2.Normalize();
		b3.Normalize();
		num += (float)Math.Acos(MathFunctions.DotProduct(ref a2, ref b2));
		num += (float)Math.Acos(MathFunctions.DotProduct(ref b2, ref b3));
		num += (float)Math.Acos(MathFunctions.DotProduct(ref b3, ref a2));
		bool flag = !float.IsInfinity(num);
		if ((double)Mathf.Abs(num - (float)Math.PI * 2f) <= 0.005 && flag)
		{
			return true;
		}
		return false;
	}

	private static bool IsFrontFacingTo(ref Plane plane, ref Vector3 direction)
	{
		Vector3 a = plane.normal;
		float num = MathFunctions.DotProduct(ref a, ref direction);
		return num <= 0f;
	}

	private static bool IsFrontFacingTo(ref Vector3 planeNormal, ref Vector3 direction)
	{
		return MathFunctions.DotProduct(ref planeNormal, ref direction) <= 0f;
	}

	public static bool CheckTriangle(ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, ref Vector3 localOrigin, ref Vector3 localDirection, float distance, ref VoxelHit voxelHit)
	{
		Vector3 planeNormal = new Plane(p1, p2, p3).normal;
		Vector3 b = new Vector3
		{
			x = localDirection.x * distance,
			y = localDirection.y * distance,
			z = localDirection.z * distance
		};
		Vector3 planeOrigin = new Vector3
		{
			x = p1.x,
			y = p1.y,
			z = p1.z
		};
		if (IsFrontFacingTo(ref planeNormal, ref localDirection))
		{
			bool flag = false;
			double num = MathFunctions.SignedDistanceTo(ref planeNormal, ref planeOrigin, ref localOrigin);
			float num2 = MathFunctions.DotProduct(ref planeNormal, ref b);
			double num3;
			if (num2 == 0f)
			{
				if (Math.Abs(num) >= 1.0)
				{
					return false;
				}
				flag = true;
				num3 = 0.0;
				double num4 = 1.0;
			}
			else
			{
				num3 = (-1.0 - num) / (double)num2;
				double num4 = (1.0 - num) / (double)num2;
				if (num3 > num4)
				{
					double num5 = num4;
					num4 = num3;
					num3 = num5;
				}
				if (num3 > 1.0 || num4 < 0.0)
				{
					return false;
				}
				if (num3 < 0.0)
				{
					num3 = 0.0;
				}
				if (num4 < 0.0)
				{
					num4 = 0.0;
				}
				if (num3 > 1.0)
				{
					num3 = 1.0;
				}
				if (num4 > 1.0)
				{
					num4 = 1.0;
				}
			}
			Vector3 point = new Vector3(0f, 0f, 0f);
			bool flag2 = false;
			double num6 = 1.0;
			if (!flag)
			{
				float num7 = (float)num3;
				Vector3 p4 = new Vector3
				{
					x = localOrigin.x - planeNormal.x + num7 * b.x,
					y = localOrigin.y - planeNormal.y + num7 * b.y,
					z = localOrigin.z - planeNormal.z + num7 * b.z
				};
				if (PointInTriangle(ref p4, ref p1, ref p2, ref p3))
				{
					flag2 = true;
					num6 = num3;
					point.x = p4.x;
					point.y = p4.y;
					point.z = p4.z;
				}
			}
			if (!flag2)
			{
				float num8 = b.x * b.x + b.y * b.y + b.z * b.z;
				float num9 = 0f;
				float root = 0f;
				float a = num8;
				Vector3 b2 = default;
				Vector3 b3 = default;
				Vector3 b4 = default;
				Vector3 b5 = default;
				Vector3 b6 = default;
				Vector3 b7 = default;
				float num10 = 0f;
				float num11 = 0f;
				float num12 = 0f;
				b2.x = localOrigin.x - p1.x;
				b2.y = localOrigin.y - p1.y;
				b2.z = localOrigin.z - p1.z;
				b3.x = p1.x - localOrigin.x;
				b3.y = p1.y - localOrigin.y;
				b3.z = p1.z - localOrigin.z;
				num10 = b3.x * b3.x + b3.y * b3.y + b3.z * b3.z;
				float b8 = 2f * MathFunctions.DotProduct(ref b, ref b2);
				num9 = num10 - 1f;
				if (GetLowestRoot(a, b8, num9, (float)num6, ref root))
				{
					num6 = root;
					flag2 = true;
					point.x = p1.x;
					point.y = p1.y;
					point.z = p1.z;
				}
				b4.x = localOrigin.x - p2.x;
				b4.y = localOrigin.y - p2.y;
				b4.z = localOrigin.z - p2.z;
				b5.x = p2.x - localOrigin.x;
				b5.y = p2.y - localOrigin.y;
				b5.z = p2.z - localOrigin.z;
				num11 = b5.x * b5.x + b5.y * b5.y + b5.z * b5.z;
				b8 = 2f * MathFunctions.DotProduct(ref b, ref b4);
				num9 = num11 - 1f;
				if (GetLowestRoot(a, b8, num9, (float)num6, ref root))
				{
					num6 = root;
					flag2 = true;
					point.x = p2.x;
					point.y = p2.y;
					point.z = p2.z;
				}
				b6.x = localOrigin.x - p3.x;
				b6.y = localOrigin.y - p3.y;
				b6.z = localOrigin.z - p3.z;
				b7.x = p3.x - localOrigin.x;
				b7.y = p3.y - localOrigin.y;
				b7.z = p3.z - localOrigin.z;
				num12 = b7.x * b7.x + b7.y * b7.y + b7.z * b7.z;
				b8 = 2f * MathFunctions.DotProduct(ref b, ref b6);
				num9 = num12 - 1f;
				if (GetLowestRoot(a, b8, num9, (float)num6, ref root))
				{
					num6 = root;
					flag2 = true;
					point.x = p3.x;
					point.y = p3.y;
					point.z = p3.z;
				}
				Vector3 a2 = new Vector3
				{
					x = p2.x - p1.x,
					y = p2.y - p1.y,
					z = p2.z - p1.z
				};
				float num13 = a2.x * a2.x + a2.y * a2.y + a2.z * a2.z;
				float num14 = MathFunctions.DotProduct(ref a2, ref b);
				float num15 = MathFunctions.DotProduct(ref a2, ref b3);
				a = num13 * (0f - num8) + num14 * num14;
				b8 = num13 * (2f * MathFunctions.DotProduct(ref b, ref b3)) - 2f * num14 * num15;
				num9 = num13 * (1f - num10) + num15 * num15;
				if (GetLowestRoot(a, b8, num9, (float)num6, ref root))
				{
					float num16 = (num14 * root - num15) / num13;
					if ((double)num16 >= 0.0 && (double)num16 <= 1.0)
					{
						num6 = root;
						flag2 = true;
						point.x = p1.x + num16 * a2.x;
						point.y = p1.y + num16 * a2.y;
						point.z = p1.z + num16 * a2.z;
					}
				}
				a2.x = p3.x - p2.x;
				a2.y = p3.y - p2.y;
				a2.z = p3.z - p2.z;
				num13 = a2.x * a2.x + a2.y * a2.y + a2.z * a2.z;
				num14 = MathFunctions.DotProduct(ref a2, ref b);
				num15 = MathFunctions.DotProduct(ref a2, ref b5);
				a = num13 * (0f - num8) + num14 * num14;
				b8 = num13 * (2f * MathFunctions.DotProduct(ref b, ref b5)) - 2f * num14 * num15;
				num9 = num13 * (1f - num11) + num15 * num15;
				if (GetLowestRoot(a, b8, num9, (float)num6, ref root))
				{
					float num17 = (num14 * root - num15) / num13;
					if ((double)num17 >= 0.0 && (double)num17 <= 1.0)
					{
						num6 = root;
						flag2 = true;
						point.x = p2.x + num17 * a2.x;
						point.y = p2.y + num17 * a2.y;
						point.z = p2.z + num17 * a2.z;
					}
				}
				a2.x = p1.x - p3.x;
				a2.y = p1.y - p3.y;
				a2.z = p1.z - p3.z;
				num13 = a2.x * a2.x + a2.y * a2.y + a2.z * a2.z;
				num14 = MathFunctions.DotProduct(ref a2, ref b);
				num15 = MathFunctions.DotProduct(ref a2, ref b7);
				a = num13 * (0f - num8) + num14 * num14;
				b8 = num13 * (2f * MathFunctions.DotProduct(ref b, ref b7)) - 2f * num14 * num15;
				num9 = num13 * (1f - num12) + num15 * num15;
				if (GetLowestRoot(a, b8, num9, (float)num6, ref root))
				{
					float num18 = (num14 * root - num15) / num13;
					if ((double)num18 >= 0.0 && (double)num18 <= 1.0)
					{
						num6 = root;
						flag2 = true;
						point.x = p3.x + num18 * a2.x;
						point.y = p3.y + num18 * a2.y;
						point.z = p3.z + num18 * a2.z;
					}
				}
			}
			if (flag2)
			{
				voxelHit.point = point;
				if ((voxelHit.distance = (float)num6 * b.magnitude) < 0f)
				{
					Debug.Log("Found negativ distance, this should be ignored");
				}
				return true;
			}
		}
		return false;
	}
}
