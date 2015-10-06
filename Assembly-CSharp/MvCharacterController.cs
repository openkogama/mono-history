using System;
using System.Collections.Generic;
using UnityEngine;

public class MvCharacterController : MonoBehaviour
{
	private struct NoCollisionData(Vector3 pos, bool val)
	{
		private Vector3 position = pos;

		private bool valid = val;

		public Vector3 Position => position;

		public bool Valid => valid;
	}

	private const float unitsPerMeter = 100f;

	private const float unitScale = 1f;

	private const float veryCloseDistance = 0.005f;

	private static readonly int layerMask = -5 & ~(1 << LayerMask.NameToLayer("Player")) & ~(1 << LayerMask.NameToLayer("Logic"));

	private Vector3 center;

	private float stepOffset;

	private Vector3 elipsoidRadius;

	private bool sendCollisionData = true;

	public Vector3 centerBase;

	public Vector3 radiusBase;

	private float offsetFactor = 0.1f;

	private float offsetBase = 0.1f;

	public HashSet<int> IgnoreWoIds;

	public Action<MVControllerColliderHit> OnControllerColliderHit;

	private static int collisionRecursionDepth;

	private float sides = 0.9f;

	private static float collisionMaxAngle = 89.95f;

	private static float collisionAdjustedAngle = 60f;

	public bool IsGrounded { get; set; }

	public Vector3 Velocity { get; set; }

	public float Radius => elipsoidRadius.x;

	public float Height => elipsoidRadius.y * 2f;

	public Vector3 Center => center;

	public void SetScale(float scale)
	{
		center = centerBase * scale;
		elipsoidRadius = new Vector3(radiusBase.x * scale, radiusBase.y * scale, radiusBase.z * scale);
		offsetFactor = offsetBase * scale;
	}

	public void Init(float radius, float height, Vector3 center)
	{
		this.center = center;
		elipsoidRadius = new Vector3(radius, height / 2f, radius);
		centerBase = new Vector3(center.x, center.y, center.z);
		radiusBase = new Vector3(elipsoidRadius.x, elipsoidRadius.y, elipsoidRadius.z);
	}

	public MvCharacterController CloneToGameObject(GameObject targetGameObject, GameObject seat)
	{
		MvCharacterController mvCharacterController = targetGameObject.AddComponent<MvCharacterController>();
		mvCharacterController.Init(Radius, elipsoidRadius.y * 2f, seat.transform.localPosition);
		return mvCharacterController;
	}

	public MVCollisionFlags Move(Vector3 motion, bool sendCollisionData)
	{
		this.sendCollisionData = sendCollisionData;
		MVCollisionFlags result = Move(motion);
		this.sendCollisionData = true;
		return result;
	}

	public MVCollisionFlags Move(Vector3 motion)
	{
		Vector3 R3Vel = motion;
		Vector3 vector = CollideAndSlide(ref R3Vel, transform.position + center);
		transform.position = vector - center;
		return MVCollisionFlags.None;
	}

	public bool TestWithOutSliding(float distance, Vector3 direction, Vector3 motion, bool sendCollisionData, out MVControllerColliderHit colliderHit)
	{
		this.sendCollisionData = sendCollisionData;
		bool result = TestWithOutSliding(distance, direction, motion, out colliderHit);
		this.sendCollisionData = true;
		return result;
	}

	public bool TestWithOutSliding(float distance, Vector3 direction, Vector3 motion, out MVControllerColliderHit colliderHit)
	{
		colliderHit = default;
		Vector3 radius = new Vector3(elipsoidRadius.x, elipsoidRadius.y - offsetFactor, elipsoidRadius.z);
		direction.Normalize();
		Vector3 vector = transform.position + center;
		if (CollisionDetection.MVElipsoidCast(new Ray(vector, direction), radius, distance + offsetFactor, out var voxelHit, IgnoreWoIds, layerMask))
		{
			MVCollisionFlags collisionFlags = MVCollisionFlags.None;
			GetHitArea(vector, voxelHit.point, voxelHit.distance, elipsoidRadius, direction, ref collisionFlags);
			colliderHit = new MVControllerColliderHit(voxelHit, vector, radius, motion, testWithOutMoving: true, collisionFlags);
			SendCharacterCollision(colliderHit);
			return true;
		}
		return false;
	}

	public bool CheckOverLap()
	{
		return OverlapCheckCollision(transform.position + center);
	}

	public List<MVOverlapResult> GetOverlappingObjects()
	{
		return OverlappingObjects(transform.position + center);
	}

	public float GetGradientAngle(Vector3 gradientDirection)
	{
		return Vector3.Angle(Vector3.up, gradientDirection) - 90f;
	}

	public Vector3 GetGradientDirection(VoxelHit elipsoidHit)
	{
		Vector3 vec = transform.position + center + Vector3.down * elipsoidHit.distance;
		Vector3 vector = MathFunctions.DivideVector(elipsoidHit.point, elipsoidRadius);
		Vector3 vector2 = MathFunctions.DivideVector(vec, elipsoidRadius);
		Vector3 normalized = (vector2 - vector).normalized;
		if (normalized.y == 0f)
		{
			return Vector3.down;
		}
		if (normalized == Vector3.up)
		{
			return Vector3.zero;
		}
		Vector3 rhs = Vector3.Cross(Vector3.up, normalized);
		Vector3 vec2 = Vector3.Cross(normalized, rhs);
		return -MathFunctions.MultiplyVector(vec2, elipsoidRadius).normalized;
	}

	private Vector3 CollideAndSlide(ref Vector3 R3Vel, Vector3 R3Position)
	{
		if (R3Vel.sqrMagnitude == 0f)
		{
			Velocity = Vector3.zero;
			return R3Position;
		}
		bool foundValidPosition = true;
		Vector3 ePos = MathFunctions.DivideVector(R3Position, elipsoidRadius);
		Vector3 eVel = MathFunctions.DivideVector(R3Vel, elipsoidRadius);
		Vector3 vec = ePos;
		Vector3 vector = default;
		collisionRecursionDepth = 0;
		vector = CollideWithWorld(ref ePos, ref eVel, ref foundValidPosition);
		Vector3 r3Position = MathFunctions.MultiplyVector(vector, elipsoidRadius);
		bool flag = OverlapCheckCollision(r3Position);
		Vector3 offset = Vector3.zero;
		if (flag && NoOverlapPosition(r3Position, R3Vel, ref offset))
		{
			flag = false;
		}
		if (foundValidPosition && !flag)
		{
			vec = vector;
		}
		Vector3 vector2 = MathFunctions.MultiplyVector(vec, elipsoidRadius);
		vector2 += offset;
		R3Position += offset;
		Vector3 velocity = vector2 - R3Position;
		Velocity = velocity;
		return vector2;
	}

	private Vector3 CollideWithWorld(ref Vector3 ePos, ref Vector3 eVel, ref bool foundValidPosition)
	{
		if (collisionRecursionDepth > 15)
		{
			NoCollisionData noCollisionData = HandleNoCollision(ePos, eVel, adjustVerticalOnly: false);
			if (noCollisionData.Valid)
			{
				return noCollisionData.Position;
			}
			foundValidPosition = false;
			return ePos;
		}
		Vector3 vector = MathFunctions.MultiplyVector(ePos, elipsoidRadius);
		Vector3 vector2 = MathFunctions.MultiplyVector(eVel, elipsoidRadius);
		if (!CollisionDetection.MVElipsoidCast(new Ray(vector, vector2.normalized), elipsoidRadius, vector2.magnitude, out var voxelHit, IgnoreWoIds, layerMask))
		{
			NoCollisionData noCollisionData2 = HandleNoCollision(ePos, eVel, adjustVerticalOnly: false);
			if (noCollisionData2.Valid)
			{
				return noCollisionData2.Position;
			}
			return ePos;
		}
		float num = DistanceR3SpaceToESpace(voxelHit.distance, vector2, elipsoidRadius);
		Vector3 ePoint = MathFunctions.DivideVector(voxelHit.point, elipsoidRadius);
		float collisionAngle = GetCollisionAngle(ePos, eVel, num, ePoint);
		if (collisionAngle > collisionMaxAngle && num != 0f)
		{
			Vector3 eVel2 = RecalcDirectionMoveAway(ePos, eVel, num, ePoint);
			eVel2 *= eVel.magnitude;
			collisionRecursionDepth++;
			return CollideWithWorld(ref ePos, ref eVel2, ref foundValidPosition);
		}
		Vector3 eDestinationPoint = ePos + eVel;
		Vector3 vector3 = ePos;
		MVCollisionFlags collisionFlags = MVCollisionFlags.None;
		GetHitArea(vector, voxelHit.point, voxelHit.distance, elipsoidRadius, vector2.normalized, ref collisionFlags);
		Vector3 vector4 = eVel;
		float moveBackDistance = GetMoveBackDistance(ePos, eVel, num, ePoint);
		vector4 = vector4.normalized;
		if ((num - moveBackDistance >= 0f && moveBackDistance > 0f) ? true : false)
		{
			vector3 = ePos + vector4 * (num - moveBackDistance);
			ePoint -= moveBackDistance * vector4;
			Vector3 slidePlaneNormal = default;
			Vector3 eVel3 = GetNextVelocity(ePoint, vector3, eDestinationPoint, ref slidePlaneNormal);
			if (voxelHit.isCubeHit)
			{
				SendCharacterCollision(new MVControllerColliderHit(voxelHit, vector, elipsoidRadius, vector2, testWithOutMoving: false, collisionFlags));
			}
			collisionRecursionDepth++;
			return CollideWithWorld(ref vector3, ref eVel3, ref foundValidPosition);
		}
		Vector3 eVel4 = RecalcDirectionMoveAway(ePos, eVel, num, ePoint);
		eVel4 *= eVel.magnitude;
		collisionRecursionDepth++;
		return CollideWithWorld(ref ePos, ref eVel4, ref foundValidPosition);
	}

	private bool HitIsEdge(VoxelHit elipsoidHit, Vector3 R3Pos, Vector3 R3Velocity)
	{
		Vector3 vector = R3Velocity.normalized * elipsoidHit.distance + R3Pos;
		Vector3 normalized = (vector - elipsoidHit.point).normalized;
		Vector3 normalized2 = MathFunctions.DivideVector(MathFunctions.DivideVector(normalized, elipsoidRadius), elipsoidRadius).normalized;
		if (Vector3.Dot(normalized2, elipsoidHit.normal) < 0.98f)
		{
			return true;
		}
		return false;
	}

	public void SendCharacterCollision(MVControllerColliderHit controllerColliderHit)
	{
		if (sendCollisionData)
		{
			OnControllerColliderHit(controllerColliderHit);
		}
	}

	private static float DistanceR3SpaceToESpace(float distance, Vector3 R3Dir, Vector3 R3Radius)
	{
		return MathFunctions.DivideVector(R3Dir.normalized * distance, R3Radius).magnitude;
	}

	private static float DistanceESpaceToR3Space(float eDistance, Vector3 eDir, Vector3 R3Radius)
	{
		return MathFunctions.MultiplyVector(eDir.normalized * eDistance, R3Radius).magnitude;
	}

	private bool HandleStepOffset(VoxelHit elipsoidHit, Vector3 R3Pos, Vector3 R3Velocity, Vector3 ePoint, Vector3 eNewBasePoint, Vector3 eVelocity, ref Vector3 offset)
	{
		if (!HitIsEdge(elipsoidHit, R3Pos, R3Velocity))
		{
			return false;
		}
		eVelocity.Normalize();
		eVelocity.y = 0f;
		if (eVelocity.magnitude < 0.5f)
		{
			return false;
		}
		Vector3 normalized = eVelocity.normalized;
		Vector3 rhs = ePoint - eNewBasePoint;
		rhs.y = 0f;
		rhs.Normalize();
		float num = (ePoint.y - (eNewBasePoint.y - 1f)) * elipsoidRadius.y / 2f;
		if (num >= stepOffset)
		{
			return false;
		}
		if (Vector3.Dot(normalized, rhs) <= 0.2f)
		{
			return false;
		}
		if (num < stepOffset && Vector3.Dot(normalized, rhs) > 0.2f)
		{
			float num2 = num / (elipsoidRadius.y / 2f);
			float num3 = MVPhysics.CalculateJumpVerticalSpeed(num2 + 0.1f);
			offset = Vector3.up * num3 * Time.deltaTime;
			return true;
		}
		return false;
	}

	private static Vector3 GetNextVelocity(Vector3 ePoint, Vector3 eNewBasePoint, Vector3 eDestinationPoint, ref Vector3 slidePlaneNormal)
	{
		slidePlaneNormal = eNewBasePoint - ePoint;
		slidePlaneNormal.Normalize();
		Plane plane = new Plane(slidePlaneNormal, ePoint);
		double num = MathFunctions.SignedDistanceTo(plane, ePoint, eDestinationPoint);
		Vector3 vector = eDestinationPoint - (float)num * slidePlaneNormal;
		return vector - ePoint;
	}

	private static float GetMoveBackDistance(Vector3 ePos, Vector3 eDir, float distance, Vector3 ePoint)
	{
		float collisionAngle = GetCollisionAngle(ePos, eDir, distance, ePoint);
		float num = 1f / Mathf.Cos(collisionAngle * ((float)Math.PI / 180f));
		float num2 = 0.995f;
		return num - num * num2;
	}

	private static Vector3 GetNormal(Vector3 ePos, Vector3 eDir, float distance, Vector3 ePoint)
	{
		eDir.Normalize();
		Vector3 vector = ePos + eDir * distance;
		Vector3 result = ePoint - vector;
		result.Normalize();
		return result;
	}

	private static float GetCollisionAngle(Vector3 ePos, Vector3 eDir, float distance, Vector3 ePoint)
	{
		return Vector3.Angle(eDir, GetNormal(ePos, eDir, distance, ePoint));
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

	private static Vector3 RecalcDirectionMoveAway(Vector3 ePos, Vector3 eDir, float distance, Vector3 ePoint)
	{
		eDir.Normalize();
		Vector3 vector = ePos + eDir * distance;
		Vector3 normalized = (vector - ePoint).normalized;
		Vector3 vector2 = vector + normalized * 0.005f;
		return (vector2 - ePos).normalized;
	}

	private void GetHitArea(Vector3 rPos, Vector3 rHit, float rDistance, Vector3 radius, Vector3 rDirection, ref MVCollisionFlags collisionFlags)
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

	private NoCollisionData HandleNoCollision(Vector3 ePos, Vector3 eVel, bool adjustVerticalOnly)
	{
		Vector3 vector = MathFunctions.MultiplyVector(ePos, elipsoidRadius);
		Vector3 vector2 = MathFunctions.MultiplyVector(eVel, elipsoidRadius);
		Vector3 normalizedVector = GetNormalizedVector(vector2);
		Vector3 normalizedVector2 = GetNormalizedVector(eVel);
		NoCollisionData result = new NoCollisionData(ePos + eVel, val: true);
		float distance = DistanceESpaceToR3Space(0.005f, Vector3.down, elipsoidRadius);
		bool flag = CollisionDetection.MVElipsoidCast(new Ray(vector + vector2, Vector3.down), elipsoidRadius, distance, out var voxelHit, IgnoreWoIds, layerMask);
		Vector3 vector3 = eVel;
		if (flag)
		{
			float num = DistanceR3SpaceToESpace(voxelHit.distance, Vector3.down, elipsoidRadius);
			result = ((!CollisionDetection.MVElipsoidCast(new Ray(vector + vector2, Vector3.up), elipsoidRadius, distance, out voxelHit, IgnoreWoIds, layerMask)) ? new NoCollisionData(ePos + eVel + (0.005f - num) * Vector3.up, val: true) : new NoCollisionData(ePos + eVel + (0.005f - num) * Vector3.up, val: false));
			vector3 = result.Position - ePos;
			normalizedVector2 = GetNormalizedVector(vector3);
			vector2 = MathFunctions.MultiplyVector(vector3, elipsoidRadius);
			normalizedVector = GetNormalizedVector(vector2);
		}
		if (!adjustVerticalOnly)
		{
			float num2 = DistanceESpaceToR3Space(0.005f, normalizedVector2, elipsoidRadius);
			if (CollisionDetection.MVElipsoidCast(new Ray(vector, normalizedVector), elipsoidRadius, num2 + vector2.magnitude, out var voxelHit2, IgnoreWoIds, layerMask))
			{
				float num3 = DistanceR3SpaceToESpace(voxelHit2.distance, normalizedVector, elipsoidRadius);
				float num4 = vector3.magnitude + 0.005f - num3;
				result = new NoCollisionData(ePos + normalizedVector2 * (vector3.magnitude - num4), val: true);
			}
		}
		return result;
	}

	private Vector3 GetNormalizedVector(Vector3 InpVec)
	{
		Vector3 normalized = InpVec.normalized;
		if (normalized.sqrMagnitude == 0f)
		{
			normalized = (InpVec * 10000f).normalized;
		}
		return normalized;
	}

	private bool NoOverlapPosition(Vector3 R3Position, Vector3 R3Direction, ref Vector3 offset)
	{
		R3Direction.Normalize();
		float num = Mathf.Sqrt(2f);
		float num2 = Vector3.Dot(R3Direction, Vector3.up);
		float num3 = 0.99f;
		Vector3 lhs = Vector3.up;
		if (num2 > num3 || num2 < 0f - num3)
		{
			lhs = Vector3.right;
		}
		Vector3 normalized = Vector3.Cross(lhs, R3Direction).normalized;
		Vector3 normalized2 = Vector3.Cross(normalized, R3Direction).normalized;
		normalized *= 0.005f;
		normalized2 *= 0.005f;
		offset = normalized;
		if (!OverlapCheckCollision(R3Position + offset))
		{
			return true;
		}
		offset = -normalized;
		if (!OverlapCheckCollision(R3Position + offset))
		{
			return true;
		}
		offset = normalized2;
		if (!OverlapCheckCollision(R3Position + offset))
		{
			return true;
		}
		offset = -normalized2;
		if (!OverlapCheckCollision(R3Position + offset))
		{
			return true;
		}
		offset = (normalized + normalized2) / num;
		if (!OverlapCheckCollision(R3Position + offset))
		{
			return true;
		}
		offset = -offset;
		if (!OverlapCheckCollision(R3Position + offset))
		{
			return true;
		}
		offset = (normalized - normalized2) / num;
		if (!OverlapCheckCollision(R3Position + offset))
		{
			return true;
		}
		offset = -offset;
		if (!OverlapCheckCollision(R3Position + offset))
		{
			return true;
		}
		offset = Vector3.zero;
		return false;
	}

	private bool OverlapCheckCollision(Vector3 R3Position)
	{
		Vector3 radius = elipsoidRadius;
		if (MVElipsoidOverlapCheck.ElipsoidOverlapCheckBool(radius, R3Position, Quaternion.identity, layerMask, IgnoreWoIds))
		{
			return true;
		}
		return false;
	}

	private List<MVOverlapResult> OverlappingObjects(Vector3 R3Position)
	{
		Vector3 radius = elipsoidRadius;
		return MVElipsoidOverlapCheck.ElipsoidOverlapCheckSector(radius, R3Position, Quaternion.identity, layerMask, IgnoreWoIds);
	}
}
