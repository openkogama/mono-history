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

	private static bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
	{
		Vector3 lhs = Vector3.Cross(b - a, p1 - a);
		Vector3 rhs = Vector3.Cross(b - a, p2 - a);
		if (Vector3.Dot(lhs, rhs) >= 0f)
		{
			return true;
		}
		return false;
	}

	private static bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
	{
		if (SameSide(p, a, b, c) && SameSide(p, b, a, c) && SameSide(p, c, a, b))
		{
			return true;
		}
		return false;
	}

	private static bool CheckPointInTriangle(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
	{
		float num = 0f;
		Vector3 vector = point - a;
		Vector3 vector2 = point - b;
		Vector3 vector3 = point - c;
		vector.Normalize();
		vector2.Normalize();
		vector3.Normalize();
		num += (float)Math.Acos(Vector3.Dot(vector, vector2));
		num += (float)Math.Acos(Vector3.Dot(vector2, vector3));
		num += (float)Math.Acos(Vector3.Dot(vector3, vector));
		bool flag = !float.IsInfinity(num);
		if ((double)Mathf.Abs(num - (float)Math.PI * 2f) <= 0.005 && flag)
		{
			return true;
		}
		return false;
	}

	private static bool IsFrontFacingTo(Plane plane, Vector3 direction)
	{
		float num = Vector3.Dot(plane.normal, direction);
		return num <= 0f;
	}

	public static bool CheckTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 localOrigin, Vector3 localDirection, float distance, ref VoxelHit voxelHit)
	{
		Plane plane = new Plane(p1, p2, p3);
		Vector3 vector = localDirection * distance;
		if (!IsFrontFacingTo(plane, localDirection))
		{
		}
		if (IsFrontFacingTo(plane, localDirection))
		{
			bool flag = false;
			double num = MathFunctions.SignedDistanceTo(plane, p1, localOrigin);
			float num2 = Vector3.Dot(plane.normal, vector);
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
			Vector3 point = Vector3.zero;
			bool flag2 = false;
			double num6 = 1.0;
			if (!flag)
			{
				Vector3 vector2 = localOrigin - plane.normal + (float)num3 * vector;
				if (PointInTriangle(vector2, p1, p2, p3))
				{
					flag2 = true;
					num6 = num3;
					point = vector2;
				}
			}
			if (!flag2)
			{
				float sqrMagnitude = vector.sqrMagnitude;
				float num7 = 0f;
				float root = 0f;
				float a = sqrMagnitude;
				float b = 2f * Vector3.Dot(vector, localOrigin - p1);
				num7 = (p1 - localOrigin).sqrMagnitude - 1f;
				if (GetLowestRoot(a, b, num7, (float)num6, ref root))
				{
					num6 = root;
					flag2 = true;
					point = p1;
				}
				b = 2f * Vector3.Dot(vector, localOrigin - p2);
				num7 = (p2 - localOrigin).sqrMagnitude - 1f;
				if (GetLowestRoot(a, b, num7, (float)num6, ref root))
				{
					num6 = root;
					flag2 = true;
					point = p2;
				}
				b = 2f * Vector3.Dot(vector, localOrigin - p3);
				num7 = (p3 - localOrigin).sqrMagnitude - 1f;
				if (GetLowestRoot(a, b, num7, (float)num6, ref root))
				{
					num6 = root;
					flag2 = true;
					point = p3;
				}
				Vector3 vector3 = p2 - p1;
				Vector3 rhs = p1 - localOrigin;
				float sqrMagnitude2 = vector3.sqrMagnitude;
				float num8 = Vector3.Dot(vector3, vector);
				float num9 = Vector3.Dot(vector3, rhs);
				a = sqrMagnitude2 * (0f - sqrMagnitude) + num8 * num8;
				b = sqrMagnitude2 * (2f * Vector3.Dot(vector, rhs)) - 2f * num8 * num9;
				num7 = sqrMagnitude2 * (1f - rhs.sqrMagnitude) + num9 * num9;
				if (GetLowestRoot(a, b, num7, (float)num6, ref root))
				{
					float num10 = (num8 * root - num9) / sqrMagnitude2;
					if ((double)num10 >= 0.0 && (double)num10 <= 1.0)
					{
						num6 = root;
						flag2 = true;
						point = p1 + num10 * vector3;
					}
				}
				vector3 = p3 - p2;
				rhs = p2 - localOrigin;
				sqrMagnitude2 = vector3.sqrMagnitude;
				num8 = Vector3.Dot(vector3, vector);
				num9 = Vector3.Dot(vector3, rhs);
				a = sqrMagnitude2 * (0f - sqrMagnitude) + num8 * num8;
				b = sqrMagnitude2 * (2f * Vector3.Dot(vector, rhs)) - 2f * num8 * num9;
				num7 = sqrMagnitude2 * (1f - rhs.sqrMagnitude) + num9 * num9;
				if (GetLowestRoot(a, b, num7, (float)num6, ref root))
				{
					float num11 = (num8 * root - num9) / sqrMagnitude2;
					if ((double)num11 >= 0.0 && (double)num11 <= 1.0)
					{
						num6 = root;
						flag2 = true;
						point = p2 + num11 * vector3;
					}
				}
				vector3 = p1 - p3;
				rhs = p3 - localOrigin;
				sqrMagnitude2 = vector3.sqrMagnitude;
				num8 = Vector3.Dot(vector3, vector);
				num9 = Vector3.Dot(vector3, rhs);
				a = sqrMagnitude2 * (0f - sqrMagnitude) + num8 * num8;
				b = sqrMagnitude2 * (2f * Vector3.Dot(vector, rhs)) - 2f * num8 * num9;
				num7 = sqrMagnitude2 * (1f - rhs.sqrMagnitude) + num9 * num9;
				if (GetLowestRoot(a, b, num7, (float)num6, ref root))
				{
					float num12 = (num8 * root - num9) / sqrMagnitude2;
					if ((double)num12 >= 0.0 && (double)num12 <= 1.0)
					{
						num6 = root;
						flag2 = true;
						point = p3 + num12 * vector3;
					}
				}
			}
			if (flag2)
			{
				voxelHit.point = point;
				if ((voxelHit.distance = (float)num6 * vector.magnitude) < 0f)
				{
					Debug.Log("Found negativ distance, this should be ignored");
				}
				return true;
			}
		}
		return false;
	}
}
