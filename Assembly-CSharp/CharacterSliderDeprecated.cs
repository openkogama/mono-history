using System;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterSliderDeprecated
{
	private const float unitsPerMeter = 100f;

	private const float unitScale = 1f;

	private const float veryCloseDistance = 0.005f;

	private static int collisionRecursionDepth;

	private static float sides = 0.9f;

	private static float collisionMaxAngle = 85f;

	private static float collisionAdjustedAngle = 80f;

	public static void CollideAndSlide(ref Vector3 R3Vel, ref Vector3 gravity, Vector3 R3Position, Vector3 radius, out CharacterSliderDataDeprecated csd)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		csd = default;
		csd.voxelHits = new List<VoxelHit>();
		bool foundValidPosition = true;
		Vector3 ePos = MathFunctions.DivideVector(R3Position, radius);
		Vector3 eVel = MathFunctions.DivideVector(R3Vel, radius);
		Vector3 ePos2 = ePos;
		Vector3 val = default;
		collisionRecursionDepth = 0;
		val = CollideWithWorld(ref ePos, ref eVel, radius, ref foundValidPosition, ref csd);
		if (foundValidPosition)
		{
			ePos2 = val;
		}
		foundValidPosition = true;
		csd.isGrounded = false;
		eVel = MathFunctions.DivideVector(gravity, radius);
		collisionRecursionDepth = 0;
		val = CollideWithWorld(ref ePos2, ref eVel, radius, ref foundValidPosition, ref csd);
		if (foundValidPosition)
		{
			ePos2 = val;
		}
		csd.position = MathFunctions.MultiplyVector(ePos2, radius);
	}

	private static Vector3 CollideWithWorld(ref Vector3 ePos, ref Vector3 eVel, Vector3 radius, ref bool foundValidPosition, ref CharacterSliderDataDeprecated csd)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		if (collisionRecursionDepth == 0 && eVel.magnitude <= 0.005f)
		{
			csd.didCollide |= true;
			return ePos;
		}
		if (collisionRecursionDepth > 10)
		{
			Debug.Log((object)"Did not find valid pos");
			foundValidPosition = false;
			return ePos;
		}
		Vector3 val = MathFunctions.MultiplyVector(ePos, radius);
		Vector3 val2 = MathFunctions.MultiplyVector(eVel, radius);
		bool flag = CollisionDetection.MVElipsoidCast(new Ray(val, val2.normalized), radius, val2.magnitude, out var voxelHit);
		if (!flag)
		{
			return ePos + eVel;
		}
		Vector3 val3 = MathFunctions.DivideVector(val2.normalized * voxelHit.distance, radius);
		float magnitude = val3.magnitude;
		Vector3 val4 = MathFunctions.DivideVector(voxelHit.point, radius);
		float collisionAngle = GetCollisionAngle(ePos, eVel, magnitude, val4);
		if (collisionAngle > collisionMaxAngle)
		{
			Vector3 val5 = RecalcDirection(ePos, eVel, magnitude, val4);
			val5 *= eVel.magnitude;
			collisionRecursionDepth++;
			return CollideWithWorld(ref ePos, ref val5, radius, ref foundValidPosition, ref csd);
		}
		Vector3 val6 = ePos + eVel;
		Vector3 ePos2 = ePos;
		GetHitArea(val, voxelHit.point, voxelHit.distance, radius, val2.normalized, ref csd.collisionFlags);
		csd.isGrounded |= (csd.collisionFlags & MVCollisionFlags.Below) != 0;
		csd.didCollide |= flag;
		csd.voxelHits.Add(voxelHit);
		if (magnitude >= 0.005f)
		{
			Vector3 val7 = eVel;
			float num = GetMoveBackDistance(ePos, eVel, magnitude, val4);
			if (magnitude - num < 0f)
			{
				num = magnitude;
			}
			val7 = val7.normalized;
			ePos2 = ePos + val7 * (magnitude - num);
			val4 -= num * val7;
		}
		Vector3 val8 = val4;
		Vector3 val9 = ePos2 - val4;
		val9.Normalize();
		Plane plane = new Plane(val9, val8);
		double num2 = MathFunctions.SignedDistanceTo(plane, val8, val6);
		Vector3 val10 = val6 - (float)num2 * val9;
		Vector3 eVel2 = val10 - val4;
		if (eVel2.magnitude < 0.005f)
		{
			return ePos2;
		}
		collisionRecursionDepth++;
		return CollideWithWorld(ref ePos2, ref eVel2, radius, ref foundValidPosition, ref csd);
	}

	private static float GetMoveBackDistance(Vector3 ePos, Vector3 eDir, float distance, Vector3 ePoint)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		float collisionAngle = GetCollisionAngle(ePos, eDir, distance, ePoint);
		float num = 1f / Mathf.Cos(collisionAngle * ((float)Math.PI / 180f));
		float num2 = 0.995f;
		return num - num * num2;
	}

	private static float GetCollisionAngle(Vector3 ePos, Vector3 eDir, float distance, Vector3 ePoint)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		eDir.Normalize();
		Vector3 val = ePos + eDir * distance;
		Vector3 val2 = ePoint - val;
		val2.Normalize();
		return Vector3.Angle(eDir, val2);
	}

	private static Vector3 RecalcDirection(Vector3 ePos, Vector3 eDir, float distance, Vector3 ePoint)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		eDir.Normalize();
		Vector3 val = ePos + eDir * distance;
		Vector3 val2 = ePoint - val;
		Vector3 normalized = val2.normalized;
		Vector3 val3 = Vector3.Cross(eDir, normalized);
		val3.Normalize();
		Vector3 val4 = Vector3.Cross(val3, eDir);
		val4.Normalize();
		Quaternion val5 = Quaternion.AngleAxis(90f - collisionAdjustedAngle, -val3);
		Vector3 val6 = val5 * val4;
		Vector3 val7 = val + val6;
		float num = Vector3.Angle(val7 - ePos, ePoint - ePos);
		Quaternion val8 = Quaternion.AngleAxis(num, val3);
		return val8 * eDir.normalized;
	}

	private static void GetHitArea(Vector3 rPos, Vector3 rHit, float rDistance, Vector3 radius, Vector3 rDirection, ref MVCollisionFlags collisionFlags)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		Vector3 vec = rHit - rDirection * rDistance - rPos;
		Vector3 val = MathFunctions.DivideVector(vec, radius);
		if (Mathf.Abs(val.x) < float.Epsilon && Mathf.Abs(val.z) < float.Epsilon)
		{
			if (val.y > 0f)
			{
				collisionFlags |= MVCollisionFlags.Above;
			}
			else
			{
				collisionFlags |= MVCollisionFlags.Below;
			}
			return;
		}
		if (Mathf.Abs(val.y) < float.Epsilon)
		{
			collisionFlags |= MVCollisionFlags.Sides;
			return;
		}
		Vector3 val2 = val;
		val2.y = 0f;
		val2.Normalize();
		val.Normalize();
		float num = Vector3.Dot(val, val2);
		if (1f - num > sides)
		{
			if (val.y > 0f)
			{
				collisionFlags |= MVCollisionFlags.Above;
			}
			else
			{
				collisionFlags |= MVCollisionFlags.Below;
			}
		}
		else
		{
			collisionFlags |= MVCollisionFlags.Sides;
		}
	}
}
