using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MvCharacterController : MonoBehaviour
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

	protected const float veryCloseDistance = 0.005f;

	private static int maxRecursions = 15;

	private static float collisionMaxAngle = 89.95f;

	private static readonly int layerMask = -5 & ~(1 << LayerMask.NameToLayer("Player")) & ~(1 << LayerMask.NameToLayer("Logic"));

	protected Vector3 center;

	protected Vector3 elipsoidRadius;

	private bool sendCollisionData = true;

	protected int collisionRecursionDepth;

	private float offsetFactor = 0.1f;

	private float offsetBase = 0.1f;

	public Vector3 centerBase;

	public Vector3 radiusBase;

	public HashSet<int> IgnoreWoIds;

	public Action<MVControllerColliderHit> OnControllerColliderHit;

	public bool IsGrounded { get; set; }

	public Vector3 Velocity { get; set; }

	public float Radius => elipsoidRadius.x;

	public float Height => elipsoidRadius.y * 2f;

	public Vector3 Center => center;

	public abstract MvCharacterController CloneToGameObject(GameObject targetGameObject, GameObject seat);

	public abstract void Move(Vector3 motion);

	protected abstract Vector3 GetNextVelocity(Vector3 ePoint, Vector3 eNewBasePoint, Vector3 eDestinationPoint, ref Vector3 slidePlaneNormal);

	protected abstract bool NoOverlapPosition(Vector3 R3Position, Vector3 R3Direction, ref Vector3 offset);

	protected abstract Vector3 RecalcDirectionMoveAway(Vector3 ePos, Vector3 eDir, float distance, Vector3 ePoint);

	public void Init(float radius, float height, Vector3 center)
	{
		this.center = center;
		elipsoidRadius = new Vector3(radius, height / 2f, radius);
		centerBase = new Vector3(center.x, center.y, center.z);
		radiusBase = new Vector3(elipsoidRadius.x, elipsoidRadius.y, elipsoidRadius.z);
	}

	public void SetScale(float scale)
	{
		center = centerBase * scale;
		elipsoidRadius = new Vector3(radiusBase.x * scale, radiusBase.y * scale, radiusBase.z * scale);
		offsetFactor = offsetBase * scale;
	}

	public void Move(Vector3 motion, bool sendCollisionData)
	{
		this.sendCollisionData = sendCollisionData;
		Move(motion);
		this.sendCollisionData = true;
	}

	public bool CheckOverLap()
	{
		return OverlapCheckCollision(transform.position + center);
	}

	public List<MVOverlapResult> GetOverlappingObjects()
	{
		return OverlappingObjects(transform.position + center);
	}

	public Vector3 GetGradientDirection(VoxelHit elipsoidHit)
	{
		Vector3 vec = transform.position + center + Vector3.down * elipsoidHit.distance;
		Vector3 vector = MathFunctions.DivideVector(ref elipsoidHit.point, ref elipsoidRadius);
		Vector3 vector2 = MathFunctions.DivideVector(ref vec, ref elipsoidRadius);
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
		return -MathFunctions.MultiplyVector(ref vec2, ref elipsoidRadius).normalized;
	}

	public bool TestWithOutSliding(float distance, Vector3 direction, Vector3 motion, out MVControllerColliderHit colliderHit)
	{
		colliderHit = default;
		Vector3 radius = new Vector3(elipsoidRadius.x, elipsoidRadius.y - offsetFactor, elipsoidRadius.z);
		direction.Normalize();
		Vector3 vector = transform.position + center;
		if (CollisionDetection.MVElipsoidCast(new Ray(vector, direction), radius, distance + offsetFactor, out var voxelHit, IgnoreWoIds, layerMask))
		{
			colliderHit = new MVControllerColliderHit(voxelHit, vector, radius, motion, testWithOutMoving: true);
			SendCharacterCollision(colliderHit);
			return true;
		}
		return false;
	}

	protected abstract Vector3 CollideAndSlide(Vector3 R3Vel, Vector3 R3Position);

	protected Vector3 CollideWithWorld(ref Vector3 ePos, ref Vector3 eVel, ref bool foundValidPosition)
	{
		if (collisionRecursionDepth > maxRecursions)
		{
			NoCollisionData noCollisionData = HandleNoCollision(ePos, eVel, adjustVerticalOnly: true);
			if (noCollisionData.Valid)
			{
				return noCollisionData.Position;
			}
			foundValidPosition = false;
			return ePos;
		}
		Vector3 vector = MathFunctions.MultiplyVector(ref ePos, ref elipsoidRadius);
		Vector3 vector2 = MathFunctions.MultiplyVector(ref eVel, ref elipsoidRadius);
		if (!CollisionDetection.MVElipsoidCast(new Ray(vector, vector2.normalized), elipsoidRadius, vector2.magnitude, out var voxelHit, IgnoreWoIds, layerMask))
		{
			NoCollisionData noCollisionData2 = HandleNoCollision(ePos, eVel, adjustVerticalOnly: true);
			if (noCollisionData2.Valid)
			{
				return noCollisionData2.Position;
			}
			return ePos;
		}
		float num = DistanceR3SpaceToESpace(voxelHit.distance, vector2, elipsoidRadius);
		Vector3 ePoint = MathFunctions.DivideVector(ref voxelHit.point, ref elipsoidRadius);
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
				SendCharacterCollision(new MVControllerColliderHit(voxelHit, vector, elipsoidRadius, vector2, testWithOutMoving: false));
			}
			collisionRecursionDepth++;
			return CollideWithWorld(ref vector3, ref eVel3, ref foundValidPosition);
		}
		Vector3 eVel4 = RecalcDirectionMoveAway(ePos, eVel, num, ePoint);
		eVel4 *= eVel.magnitude;
		collisionRecursionDepth++;
		return CollideWithWorld(ref ePos, ref eVel4, ref foundValidPosition);
	}

	private void SendCharacterCollision(MVControllerColliderHit controllerColliderHit)
	{
		if (sendCollisionData)
		{
			OnControllerColliderHit(controllerColliderHit);
		}
	}

	private static float DistanceR3SpaceToESpace(float distance, Vector3 R3Dir, Vector3 R3Radius)
	{
		Vector3 vec = R3Dir.normalized;
		vec.x *= distance;
		vec.y *= distance;
		vec.z *= distance;
		return MathFunctions.DivideVector(ref vec, ref R3Radius).magnitude;
	}

	private static float DistanceESpaceToR3Space(float eDistance, Vector3 eDir, Vector3 R3Radius)
	{
		Vector3 vec = eDir.normalized;
		vec.x *= eDistance;
		vec.y *= eDistance;
		vec.z *= eDistance;
		return MathFunctions.MultiplyVector(ref vec, ref R3Radius).magnitude;
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
		Vector3 normal = GetNormal(ePos, eDir, distance, ePoint);
		return Vector3.Angle(eDir, normal);
	}

	private NoCollisionData HandleNoCollision(Vector3 ePos, Vector3 eVel, bool adjustVerticalOnly)
	{
		Vector3 vector = MathFunctions.MultiplyVector(ref ePos, ref elipsoidRadius);
		Vector3 vector2 = MathFunctions.MultiplyVector(ref eVel, ref elipsoidRadius);
		Vector3 normalizedVector = GetNormalizedVector(vector2);
		Vector3 normalizedVector2 = GetNormalizedVector(eVel);
		NoCollisionData result = new NoCollisionData(ePos + eVel, val: true);
		float distance = DistanceESpaceToR3Space(0.005f, Vector3.down, elipsoidRadius);
		bool flag = CollisionDetection.MVElipsoidCast(new Ray(vector + vector2, Vector3.down), elipsoidRadius, distance, out var voxelHit, IgnoreWoIds, layerMask);
		Vector3 vec = eVel;
		if (flag)
		{
			float num = DistanceR3SpaceToESpace(voxelHit.distance, Vector3.down, elipsoidRadius);
			result = ((!CollisionDetection.MVElipsoidCast(new Ray(vector + vector2, Vector3.up), elipsoidRadius, distance, out voxelHit, IgnoreWoIds, layerMask)) ? new NoCollisionData(ePos + eVel + (0.005f - num) * Vector3.up, val: true) : new NoCollisionData(ePos + eVel + (0.005f - num) * Vector3.up, val: false));
			vec = result.Position - ePos;
			normalizedVector2 = GetNormalizedVector(vec);
			vector2 = MathFunctions.MultiplyVector(ref vec, ref elipsoidRadius);
			normalizedVector = GetNormalizedVector(vector2);
		}
		if (!adjustVerticalOnly)
		{
			float num2 = DistanceESpaceToR3Space(0.005f, normalizedVector2, elipsoidRadius);
			if (CollisionDetection.MVElipsoidCast(new Ray(vector, normalizedVector), elipsoidRadius, num2 + vector2.magnitude, out var voxelHit2, IgnoreWoIds, layerMask))
			{
				float num3 = DistanceR3SpaceToESpace(voxelHit2.distance, normalizedVector, elipsoidRadius);
				float num4 = vec.magnitude + 0.005f - num3;
				result = new NoCollisionData(ePos + normalizedVector2 * (vec.magnitude - num4), val: true);
			}
		}
		return result;
	}

	private static Vector3 GetNormalizedVector(Vector3 InpVec)
	{
		Vector3 normalized = InpVec.normalized;
		if (normalized.sqrMagnitude == 0f)
		{
			normalized = (InpVec * 10000f).normalized;
		}
		return normalized;
	}

	protected bool OverlapCheckCollision(Vector3 R3Position)
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
