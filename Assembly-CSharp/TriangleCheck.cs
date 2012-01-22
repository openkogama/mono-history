using System;
using UnityEngine;

public static class TriangleCheck
{
	private static bool GetLowestRoot(float a, float b, float c, float maxR, ref float root)
	{
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.Cross(b - a, p1 - a);
		Vector3 val2 = Vector3.Cross(b - a, p2 - a);
		if (Vector3.Dot(val, val2) >= 0f)
		{
			return true;
		}
		return false;
	}

	private static bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (SameSide(p, a, b, c) && SameSide(p, b, a, c) && SameSide(p, c, a, b))
		{
			return true;
		}
		return false;
	}

	private static bool CheckPointInTriangle(Vector3 point, Vector3 a, Vector3 b, Vector3 c)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		Vector3 val = point - a;
		Vector3 val2 = point - b;
		Vector3 val3 = point - c;
		val.Normalize();
		val2.Normalize();
		val3.Normalize();
		num += (float)Math.Acos(Vector3.Dot(val, val2));
		num += (float)Math.Acos(Vector3.Dot(val2, val3));
		num += (float)Math.Acos(Vector3.Dot(val3, val));
		bool flag = !float.IsInfinity(num);
		if ((double)Mathf.Abs(num - (float)Math.PI * 2f) <= 0.005 && flag)
		{
			return true;
		}
		return false;
	}

	private static bool IsFrontFacingTo(Plane plane, Vector3 direction)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Dot(plane.normal, direction);
		return num <= 0f;
	}

	public static bool CheckTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 localOrigin, Vector3 localDirection, float distance, ref VoxelHit voxelHit)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		Plane plane = new Plane(p1, p2, p3);
		Vector3 val = localDirection * distance;
		if (!IsFrontFacingTo(plane, localDirection))
		{
		}
		if (IsFrontFacingTo(plane, localDirection))
		{
			bool flag = false;
			double num = MathFunctions.SignedDistanceTo(plane, p1, localOrigin);
			float num2 = Vector3.Dot(plane.normal, val);
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
				Vector3 val2 = localOrigin - plane.normal + (float)num3 * val;
				if (CheckPointInTriangle(val2, p1, p2, p3))
				{
					flag2 = true;
					num6 = num3;
					point = val2;
				}
			}
			if (!flag2)
			{
				float sqrMagnitude = val.sqrMagnitude;
				float num7 = 0f;
				float root = 0f;
				float a = sqrMagnitude;
				float b = 2f * Vector3.Dot(val, localOrigin - p1);
				Vector3 val3 = p1 - localOrigin;
				num7 = val3.sqrMagnitude - 1f;
				if (GetLowestRoot(a, b, num7, (float)num6, ref root))
				{
					num6 = root;
					flag2 = true;
					point = p1;
				}
				b = 2f * Vector3.Dot(val, localOrigin - p2);
				Vector3 val4 = p2 - localOrigin;
				num7 = val4.sqrMagnitude - 1f;
				if (GetLowestRoot(a, b, num7, (float)num6, ref root))
				{
					num6 = root;
					flag2 = true;
					point = p2;
				}
				b = 2f * Vector3.Dot(val, localOrigin - p3);
				Vector3 val5 = p3 - localOrigin;
				num7 = val5.sqrMagnitude - 1f;
				if (GetLowestRoot(a, b, num7, (float)num6, ref root))
				{
					num6 = root;
					flag2 = true;
					point = p3;
				}
				Vector3 val6 = p2 - p1;
				Vector3 val7 = p1 - localOrigin;
				float sqrMagnitude2 = val6.sqrMagnitude;
				float num8 = Vector3.Dot(val6, val);
				float num9 = Vector3.Dot(val6, val7);
				a = sqrMagnitude2 * (0f - sqrMagnitude) + num8 * num8;
				b = sqrMagnitude2 * (2f * Vector3.Dot(val, val7)) - 2f * num8 * num9;
				num7 = sqrMagnitude2 * (1f - val7.sqrMagnitude) + num9 * num9;
				if (GetLowestRoot(a, b, num7, (float)num6, ref root))
				{
					float num10 = (num8 * root - num9) / sqrMagnitude2;
					if ((double)num10 >= 0.0 && (double)num10 <= 1.0)
					{
						num6 = root;
						flag2 = true;
						point = p1 + num10 * val6;
					}
				}
				val6 = p3 - p2;
				val7 = p2 - localOrigin;
				sqrMagnitude2 = val6.sqrMagnitude;
				num8 = Vector3.Dot(val6, val);
				num9 = Vector3.Dot(val6, val7);
				a = sqrMagnitude2 * (0f - sqrMagnitude) + num8 * num8;
				b = sqrMagnitude2 * (2f * Vector3.Dot(val, val7)) - 2f * num8 * num9;
				num7 = sqrMagnitude2 * (1f - val7.sqrMagnitude) + num9 * num9;
				if (GetLowestRoot(a, b, num7, (float)num6, ref root))
				{
					float num11 = (num8 * root - num9) / sqrMagnitude2;
					if ((double)num11 >= 0.0 && (double)num11 <= 1.0)
					{
						num6 = root;
						flag2 = true;
						point = p2 + num11 * val6;
					}
				}
				val6 = p1 - p3;
				val7 = p3 - localOrigin;
				sqrMagnitude2 = val6.sqrMagnitude;
				num8 = Vector3.Dot(val6, val);
				num9 = Vector3.Dot(val6, val7);
				a = sqrMagnitude2 * (0f - sqrMagnitude) + num8 * num8;
				b = sqrMagnitude2 * (2f * Vector3.Dot(val, val7)) - 2f * num8 * num9;
				num7 = sqrMagnitude2 * (1f - val7.sqrMagnitude) + num9 * num9;
				if (GetLowestRoot(a, b, num7, (float)num6, ref root))
				{
					float num12 = (num8 * root - num9) / sqrMagnitude2;
					if ((double)num12 >= 0.0 && (double)num12 <= 1.0)
					{
						num6 = root;
						flag2 = true;
						point = p3 + num12 * val6;
					}
				}
			}
			if (flag2)
			{
				voxelHit.point = point;
				if ((voxelHit.distance = (float)num6 * val.magnitude) < 0f)
				{
					Debug.Log((object)"Found negativ distance, this should be ignored");
				}
				return true;
			}
		}
		return false;
	}
}
