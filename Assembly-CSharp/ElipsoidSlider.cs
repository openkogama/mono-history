using UnityEngine;

public static class ElipsoidSlider
{
	private const float unitsPerMeter = 100f;

	private const float unitScale = 1f;

	private const float veryCloseDistance = 0.005f;

	private static int collisionRecursionDepth;

	private static Matrix4x4 worldToElipsoidSpace = default;

	private static Matrix4x4 elipsoidSpaceToWorld = default;

	public static Vector3 CollideAndSlide(Vector3 R3Vel, Transform transform, Bounds localBounds)
	{
		collisionRecursionDepth = 0;
		Vector3 s = MathFunctions.MultiplyVector(localBounds.size / 2f, transform.localScale);
		elipsoidSpaceToWorld = Matrix4x4.TRS(Vector3.zero, transform.rotation, s);
		worldToElipsoidSpace = elipsoidSpaceToWorld.inverse;
		Vector3 ePos = worldToElipsoidSpace.MultiplyPoint(transform.position);
		Vector3 eVel = worldToElipsoidSpace.MultiplyVector(R3Vel);
		bool foundValidPosition = true;
		Vector3 v = CollideWithWorld(ref ePos, ref eVel, transform, localBounds, ref foundValidPosition);
		if (foundValidPosition)
		{
			return elipsoidSpaceToWorld.MultiplyPoint(v);
		}
		return transform.position;
	}

	private static Vector3 CollideWithWorld(ref Vector3 ePos, ref Vector3 eVel, Transform transform, Bounds localBounds, ref bool foundValidPosition)
	{
		if (collisionRecursionDepth == 0 && eVel.magnitude <= 0.005f)
		{
			return ePos;
		}
		if (collisionRecursionDepth > 5)
		{
			Debug.Log("did not find valid pos");
			foundValidPosition = false;
			return ePos;
		}
		Vector3 origin = elipsoidSpaceToWorld.MultiplyPoint(ePos);
		Vector3 vector = elipsoidSpaceToWorld.MultiplyVector(eVel);
		if (!CollisionDetection.MVElipsoidCast(new Ray(origin, vector.normalized), transform, localBounds, vector.magnitude, out var voxelHit))
		{
			return ePos + eVel - eVel.normalized * 0.005f;
		}
		Vector3 vector2 = ePos + eVel;
		Vector3 ePos2 = ePos;
		float magnitude = worldToElipsoidSpace.MultiplyVector(vector.normalized * voxelHit.distance).magnitude;
		Vector3 vector3 = worldToElipsoidSpace.MultiplyPoint(voxelHit.point);
		if (magnitude >= 0.005f)
		{
			Vector3 vector4 = eVel;
			vector4 = vector4.normalized * (magnitude - 0.005f);
			ePos2 = ePos + vector4;
			vector4.Normalize();
			vector3 -= 0.005f * vector4;
		}
		Vector3 vector5 = vector3;
		Vector3 vector6 = ePos2 - vector3;
		vector6.Normalize();
		Plane plane = new Plane(vector6, vector5);
		double num = MathFunctions.SignedDistanceTo(plane, vector5, vector2);
		Vector3 vector7 = vector2 - (float)num * vector6;
		Vector3 eVel2 = vector7 - vector3;
		if (eVel2.magnitude < 0.005f)
		{
			return ePos2;
		}
		collisionRecursionDepth++;
		return CollideWithWorld(ref ePos2, ref eVel2, transform, localBounds, ref foundValidPosition);
	}
}
