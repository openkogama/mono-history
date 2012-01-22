using UnityEngine;

public static class ElipsoidSlider
{
	private const float unitsPerMeter = 100f;

	private const float unitScale = 1f;

	private const float veryCloseDistance = 0.005f;

	private static int collisionRecursionDepth;

	private static Matrix4x4 worldToElipsoidSpace = default;

	private static Matrix4x4 elipsoidSpaceToWorld = default;

	static ElipsoidSlider()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
	}

	public static Vector3 CollideAndSlide(Vector3 R3Vel, Transform transform, Bounds localBounds)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		collisionRecursionDepth = 0;
		Vector3 val = MathFunctions.MultiplyVector(localBounds.size / 2f, transform.localScale);
		elipsoidSpaceToWorld = Matrix4x4.TRS(Vector3.zero, transform.rotation, val);
		worldToElipsoidSpace = elipsoidSpaceToWorld.inverse;
		Vector3 ePos = worldToElipsoidSpace.MultiplyPoint(transform.position);
		Vector3 eVel = worldToElipsoidSpace.MultiplyVector(R3Vel);
		bool foundValidPosition = true;
		Vector3 val2 = CollideWithWorld(ref ePos, ref eVel, transform, localBounds, ref foundValidPosition);
		if (foundValidPosition)
		{
			return elipsoidSpaceToWorld.MultiplyPoint(val2);
		}
		return transform.position;
	}

	private static Vector3 CollideWithWorld(ref Vector3 ePos, ref Vector3 eVel, Transform transform, Bounds localBounds, ref bool foundValidPosition)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		if (collisionRecursionDepth == 0 && eVel.magnitude <= 0.005f)
		{
			return ePos;
		}
		if (collisionRecursionDepth > 5)
		{
			Debug.Log((object)"did not find valid pos");
			foundValidPosition = false;
			return ePos;
		}
		Vector3 val = elipsoidSpaceToWorld.MultiplyPoint(ePos);
		Vector3 val2 = elipsoidSpaceToWorld.MultiplyVector(eVel);
		if (!CollisionDetection.MVElipsoidCast(new Ray(val, val2.normalized), transform, localBounds, val2.magnitude, out var voxelHit))
		{
			return ePos + eVel - eVel.normalized * 0.005f;
		}
		Vector3 val3 = ePos + eVel;
		Vector3 ePos2 = ePos;
		Vector3 val4 = worldToElipsoidSpace.MultiplyVector(val2.normalized * voxelHit.distance);
		float magnitude = val4.magnitude;
		Vector3 val5 = worldToElipsoidSpace.MultiplyPoint(voxelHit.point);
		if (magnitude >= 0.005f)
		{
			Vector3 val6 = eVel;
			val6 = val6.normalized * (magnitude - 0.005f);
			ePos2 = ePos + val6;
			val6.Normalize();
			val5 -= 0.005f * val6;
		}
		Vector3 val7 = val5;
		Vector3 val8 = ePos2 - val5;
		val8.Normalize();
		Plane plane = new Plane(val8, val7);
		double num = MathFunctions.SignedDistanceTo(plane, val7, val3);
		Vector3 val9 = val3 - (float)num * val8;
		Vector3 eVel2 = val9 - val5;
		if (eVel2.magnitude < 0.005f)
		{
			return ePos2;
		}
		collisionRecursionDepth++;
		return CollideWithWorld(ref ePos2, ref eVel2, transform, localBounds, ref foundValidPosition);
	}
}
