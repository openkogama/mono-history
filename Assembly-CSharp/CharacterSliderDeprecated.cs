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
		csd = default;
		csd.voxelHits = new List<VoxelHit>();
		bool foundValidPosition = true;
		Vector3 ePos = MathFunctions.DivideVector(R3Position, radius);
		Vector3 eVel = MathFunctions.DivideVector(R3Vel, radius);
		Vector3 ePos2 = ePos;
		Vector3 vector = default;
		collisionRecursionDepth = 0;
		vector = CollideWithWorld(ref ePos, ref eVel, radius, ref foundValidPosition, ref csd);
		if (foundValidPosition)
		{
			ePos2 = vector;
		}
		foundValidPosition = true;
		csd.isGrounded = false;
		eVel = MathFunctions.DivideVector(gravity, radius);
		collisionRecursionDepth = 0;
		vector = CollideWithWorld(ref ePos2, ref eVel, radius, ref foundValidPosition, ref csd);
		if (foundValidPosition)
		{
			ePos2 = vector;
		}
		csd.position = MathFunctions.MultiplyVector(ePos2, radius);
	}

	private static Vector3 CollideWithWorld(ref Vector3 ePos, ref Vector3 eVel, Vector3 radius, ref bool foundValidPosition, ref CharacterSliderDataDeprecated csd)
	{
		if (collisionRecursionDepth == 0 && eVel.magnitude <= 0.005f)
		{
			csd.didCollide |= true;
			return ePos;
		}
		if (collisionRecursionDepth > 10)
		{
			Debug.Log("Did not find valid pos");
			foundValidPosition = false;
			return ePos;
		}
		Vector3 vector = MathFunctions.MultiplyVector(ePos, radius);
		Vector3 vector2 = MathFunctions.MultiplyVector(eVel, radius);
		bool flag = CollisionDetection.MVElipsoidCast(new Ray(vector, vector2.normalized), radius, vector2.magnitude, out var voxelHit);
		if (!flag)
		{
			return ePos + eVel;
		}
		float magnitude = MathFunctions.DivideVector(vector2.normalized * voxelHit.distance, radius).magnitude;
		Vector3 vector3 = MathFunctions.DivideVector(voxelHit.point, radius);
		float collisionAngle = GetCollisionAngle(ePos, eVel, magnitude, vector3);
		if (collisionAngle > collisionMaxAngle)
		{
			Vector3 eVel2 = RecalcDirection(ePos, eVel, magnitude, vector3);
			eVel2 *= eVel.magnitude;
			collisionRecursionDepth++;
			return CollideWithWorld(ref ePos, ref eVel2, radius, ref foundValidPosition, ref csd);
		}
		Vector3 vector4 = ePos + eVel;
		Vector3 ePos2 = ePos;
		GetHitArea(vector, voxelHit.point, voxelHit.distance, radius, vector2.normalized, ref csd.collisionFlags);
		csd.isGrounded |= (csd.collisionFlags & MVCollisionFlags.Below) != 0;
		csd.didCollide |= flag;
		csd.voxelHits.Add(voxelHit);
		if (magnitude >= 0.005f)
		{
			Vector3 vector5 = eVel;
			float num = GetMoveBackDistance(ePos, eVel, magnitude, vector3);
			if (magnitude - num < 0f)
			{
				num = magnitude;
			}
			vector5 = vector5.normalized;
			ePos2 = ePos + vector5 * (magnitude - num);
			vector3 -= num * vector5;
		}
		Vector3 vector6 = vector3;
		Vector3 vector7 = ePos2 - vector3;
		vector7.Normalize();
		Plane plane = new Plane(vector7, vector6);
		double num2 = MathFunctions.SignedDistanceTo(plane, vector6, vector4);
		Vector3 vector8 = vector4 - (float)num2 * vector7;
		Vector3 eVel3 = vector8 - vector3;
		if (eVel3.magnitude < 0.005f)
		{
			return ePos2;
		}
		collisionRecursionDepth++;
		return CollideWithWorld(ref ePos2, ref eVel3, radius, ref foundValidPosition, ref csd);
	}

	private static float GetMoveBackDistance(Vector3 ePos, Vector3 eDir, float distance, Vector3 ePoint)
	{
		float collisionAngle = GetCollisionAngle(ePos, eDir, distance, ePoint);
		float num = 1f / Mathf.Cos(collisionAngle * ((float)Math.PI / 180f));
		float num2 = 0.995f;
		return num - num * num2;
	}

	private static float GetCollisionAngle(Vector3 ePos, Vector3 eDir, float distance, Vector3 ePoint)
	{
		eDir.Normalize();
		Vector3 vector = ePos + eDir * distance;
		Vector3 to = ePoint - vector;
		to.Normalize();
		return Vector3.Angle(eDir, to);
	}

	private static Vector3 RecalcDirection(Vector3 ePos, Vector3 eDir, float distance, Vector3 ePoint)
	{
		eDir.Normalize();
		Vector3 vector = ePos + eDir * distance;
		Vector3 normalized = (ePoint - vector).normalized;
		Vector3 vector2 = Vector3.Cross(eDir, normalized);
		vector2.Normalize();
		Vector3 vector3 = Vector3.Cross(vector2, eDir);
		vector3.Normalize();
		Quaternion quaternion = Quaternion.AngleAxis(90f - collisionAdjustedAngle, -vector2);
		Vector3 vector4 = quaternion * vector3;
		Vector3 vector5 = vector + vector4;
		float angle = Vector3.Angle(vector5 - ePos, ePoint - ePos);
		Quaternion quaternion2 = Quaternion.AngleAxis(angle, vector2);
		return quaternion2 * eDir.normalized;
	}

	private static void GetHitArea(Vector3 rPos, Vector3 rHit, float rDistance, Vector3 radius, Vector3 rDirection, ref MVCollisionFlags collisionFlags)
	{
		Vector3 vec = rHit - rDirection * rDistance - rPos;
		Vector3 vector = MathFunctions.DivideVector(vec, radius);
		if (Mathf.Abs(vector.x) < Mathf.Epsilon && Mathf.Abs(vector.z) < Mathf.Epsilon)
		{
			if (vector.y > 0f)
			{
				collisionFlags |= MVCollisionFlags.Above;
			}
			else
			{
				collisionFlags |= MVCollisionFlags.Below;
			}
			return;
		}
		if (Mathf.Abs(vector.y) < Mathf.Epsilon)
		{
			collisionFlags |= MVCollisionFlags.Sides;
			return;
		}
		Vector3 rhs = vector;
		rhs.y = 0f;
		rhs.Normalize();
		vector.Normalize();
		float num = Vector3.Dot(vector, rhs);
		if (1f - num > sides)
		{
			if (vector.y > 0f)
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
