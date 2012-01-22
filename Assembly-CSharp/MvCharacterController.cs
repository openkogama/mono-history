using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MvCharacterController : MonoBehaviour
{
	public struct CharacterSliderData
	{
		public List<VoxelHit> voxelHits;

		public bool isGrounded;

		public Vector3 position;

		public bool didCollide;

		public float friction;

		public Vector3 bounce;

		public int damage;

		public MVCollisionFlags collisionFlags;
	}

	public delegate void OnControllerColliderHitDelegate(MVControllerColliderHit hit);

	private const float unitsPerMeter = 100f;

	private const float unitScale = 1f;

	private const float veryCloseDistance = 0.005f;

	public float radius;

	public float height;

	public Vector3 center;

	public float slopeLimit = 10f;

	public float stepOffset;

	private Vector3 elipsoidRadius;

	private HashSet<int> ignoreAvatarId;

	public OnControllerColliderHitDelegate OnControllerColliderHit;

	private static int collisionRecursionDepth;

	private static float sides = 0.9f;

	private static float collisionMaxAngle = 89.95f;

	private static float collisionAdjustedAngle = 60f;

	public bool IsGrounded { get; set; }

	public Vector3 Velocity
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return field;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			field = value;
		}
	}

	public int CollisionFlags { get; set; }

	public bool DetectCollisions { get; set; }

	public Vector3 ElipsoidRadius
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return elipsoidRadius;
		}
	}

	public void Init(int ownerWoId)
	{
		ignoreAvatarId = new HashSet<int> { ownerWoId };
	}

	public MVCollisionFlags Move(Vector3 motion)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		elipsoidRadius = Vector3.one * radius;
		elipsoidRadius.y = height / 2f;
		Vector3 R3Vel = motion;
		CollideAndSlide(ref R3Vel, ((Component)this).transform.position + center, out var csd);
		((Component)this).transform.position = csd.position - center;
		IsGrounded = csd.isGrounded;
		return MVCollisionFlags.None;
	}

	public bool TestWithOutSliding(float distance, Vector3 direction, out MVControllerColliderHit colliderHit)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		colliderHit = default;
		elipsoidRadius = Vector3.one * radius;
		elipsoidRadius.y = height / 2f;
		direction.Normalize();
		Vector3 val = ((Component)this).transform.position + center;
		if (CollisionDetection.MVElipsoidCast(new Ray(val, direction), elipsoidRadius, distance, out var voxelHit, ignoreAvatarId))
		{
			colliderHit = new MVControllerColliderHit(this, voxelHit, val, elipsoidRadius, direction * distance);
			return true;
		}
		return false;
	}

	private void CollideAndSlide(ref Vector3 R3Vel, Vector3 R3Position, out CharacterSliderData csd)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		csd = default;
		if (R3Vel.sqrMagnitude == 0f)
		{
			csd.position = R3Position;
			return;
		}
		csd.voxelHits = new List<VoxelHit>();
		bool foundValidPosition = true;
		Vector3 ePos = MathFunctions.DivideVector(R3Position, elipsoidRadius);
		Vector3 eVel = MathFunctions.DivideVector(R3Vel, elipsoidRadius);
		Vector3 vec = ePos;
		Vector3 val = default;
		collisionRecursionDepth = 0;
		val = CollideWithWorld(ref ePos, ref eVel, ref foundValidPosition, ref csd);
		Vector3 r3Position = MathFunctions.MultiplyVector(val, elipsoidRadius);
		bool flag = OverlapCheckCollision(r3Position);
		Vector3 offset = Vector3.zero;
		if (flag && NoOverlapPosition(r3Position, R3Vel, ref offset))
		{
			flag = false;
		}
		if (foundValidPosition && !flag)
		{
			vec = val;
		}
		Vector3 val2 = MathFunctions.MultiplyVector(vec, elipsoidRadius);
		val2 += offset;
		R3Position += offset;
		Velocity = val2 - R3Position;
		csd.position = val2;
	}

	private Vector3 CollideWithWorld(ref Vector3 ePos, ref Vector3 eVel, ref bool foundValidPosition, ref CharacterSliderData csd)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		if (collisionRecursionDepth > 5)
		{
			foundValidPosition = false;
			return ePos;
		}
		Vector3 val = MathFunctions.MultiplyVector(ePos, elipsoidRadius);
		Vector3 val2 = MathFunctions.MultiplyVector(eVel, elipsoidRadius);
		bool flag = CollisionDetection.MVElipsoidCast(new Ray(val, val2.normalized), elipsoidRadius, val2.magnitude, out var voxelHit, ignoreAvatarId);
		if (!flag)
		{
			return HandleNoCollision(ePos, eVel);
		}
		float num = DistanceR3SpaceToESpace(voxelHit.distance, val2, elipsoidRadius);
		Vector3 val3 = MathFunctions.DivideVector(voxelHit.point, elipsoidRadius);
		float collisionAngle = GetCollisionAngle(ePos, eVel, num, val3);
		if (collisionAngle > collisionMaxAngle && num != 0f)
		{
			Vector3 val4 = RecalcDirectionMoveAway(ePos, eVel, num, val3);
			val4 *= eVel.magnitude;
			collisionRecursionDepth++;
			return CollideWithWorld(ref ePos, ref val4, ref foundValidPosition, ref csd);
		}
		Vector3 eDestinationPoint = ePos + eVel;
		Vector3 val5 = ePos;
		GetHitArea(val, voxelHit.point, voxelHit.distance, elipsoidRadius, val2.normalized, ref csd.collisionFlags);
		csd.isGrounded |= (csd.collisionFlags & MVCollisionFlags.Below) != 0;
		csd.didCollide |= flag;
		csd.voxelHits.Add(voxelHit);
		Vector3 val6 = eVel;
		float moveBackDistance = GetMoveBackDistance(ePos, eVel, num, val3);
		val6 = val6.normalized;
		val5 = ePos + val6 * (num - moveBackDistance);
		val3 -= moveBackDistance * val6;
		Vector3 slidePlaneNormal = default;
		Vector3 eVel2 = GetNextVelocity(val3, val5, eDestinationPoint, ref slidePlaneNormal);
		SendCharacterCollision(voxelHit, val, val2);
		Vector3 offset = default;
		float num2 = Vector3.Angle(slidePlaneNormal, Vector3.up);
		if (HandleStepOffset(voxelHit, val, val2, val3, val5, eVel, ref offset))
		{
			eVel2.y = offset.y;
		}
		else if (num2 > slopeLimit && slopeLimit > 0f && num2 < 90f && eVel2.y > 0f)
		{
			Vector3 val7 = val3 - val5;
			val7.y = 0f;
			val7.Normalize();
			Vector3 ePoint = val5 + val7;
			eVel2 = GetNextVelocity(ePoint, val5, eDestinationPoint, ref slidePlaneNormal);
		}
		collisionRecursionDepth++;
		return CollideWithWorld(ref val5, ref eVel2, ref foundValidPosition, ref csd);
	}

	private bool HitIsEdge(VoxelHit elipsoidHit, Vector3 R3Pos, Vector3 R3Velocity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = R3Velocity.normalized * elipsoidHit.distance + R3Pos;
		Vector3 val2 = val - elipsoidHit.point;
		Vector3 normalized = val2.normalized;
		Vector3 val3 = MathFunctions.DivideVector(MathFunctions.DivideVector(normalized, elipsoidRadius), elipsoidRadius);
		Vector3 normalized2 = val3.normalized;
		if (Vector3.Dot(normalized2, elipsoidHit.normal) < 0.98f)
		{
			return true;
		}
		return false;
	}

	private void SendCharacterCollision(VoxelHit hit, Vector3 R3Pos, Vector3 R3Velocity)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (hit.isCubeHit)
		{
			OnControllerColliderHit(new MVControllerColliderHit(this, hit, R3Pos, elipsoidRadius, R3Velocity));
		}
	}

	private static float DistanceR3SpaceToESpace(float distance, Vector3 R3Dir, Vector3 R3Radius)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = MathFunctions.DivideVector(R3Dir.normalized * distance, R3Radius);
		return val.magnitude;
	}

	private static float DistanceESpaceToR3Space(float eDistance, Vector3 eDir, Vector3 R3Radius)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = MathFunctions.MultiplyVector(eDir.normalized * eDistance, R3Radius);
		return val.magnitude;
	}

	private bool HandleStepOffset(VoxelHit elipsoidHit, Vector3 R3Pos, Vector3 R3Velocity, Vector3 ePoint, Vector3 eNewBasePoint, Vector3 eVelocity, ref Vector3 offset)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
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
		Vector3 val = ePoint - eNewBasePoint;
		val.y = 0f;
		val.Normalize();
		float num = (ePoint.y - (eNewBasePoint.y - 1f)) * height / 2f;
		if (num >= stepOffset)
		{
			return false;
		}
		if (Vector3.Dot(normalized, val) <= 0.2f)
		{
			return false;
		}
		if (num < stepOffset && Vector3.Dot(normalized, val) > 0.2f)
		{
			float num2 = num / (height / 2f);
			float num3 = MVPhysics.CalculateJumpVerticalSpeed(num2 + 0.1f);
			offset = Vector3.up * num3 * Time.deltaTime;
			return true;
		}
		return false;
	}

	private static Vector3 GetNextVelocity(Vector3 ePoint, Vector3 eNewBasePoint, Vector3 eDestinationPoint, ref Vector3 slidePlaneNormal)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		slidePlaneNormal = eNewBasePoint - ePoint;
		slidePlaneNormal.Normalize();
		Plane plane = new Plane(slidePlaneNormal, ePoint);
		double num = MathFunctions.SignedDistanceTo(plane, ePoint, eDestinationPoint);
		Vector3 val = eDestinationPoint - (float)num * slidePlaneNormal;
		return val - ePoint;
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

	private static Vector3 RecalcDirectionMoveAway(Vector3 ePos, Vector3 eDir, float distance, Vector3 ePoint)
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
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		eDir.Normalize();
		Vector3 val = ePos + eDir * distance;
		Vector3 val2 = val - ePoint;
		Vector3 normalized = val2.normalized;
		Vector3 val3 = val + normalized * 0.005f;
		Vector3 val4 = val3 - ePos;
		return val4.normalized;
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

	private Vector3 HandleNoCollision(Vector3 ePos, Vector3 eVel)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = MathFunctions.MultiplyVector(ePos, elipsoidRadius);
		Vector3 r3Dir = MathFunctions.MultiplyVector(eVel, elipsoidRadius);
		float num = DistanceESpaceToR3Space(0.005f, eVel, elipsoidRadius);
		if (CollisionDetection.MVElipsoidCast(new Ray(val, r3Dir.normalized), elipsoidRadius, num + r3Dir.magnitude, out var voxelHit, ignoreAvatarId))
		{
			float num2 = DistanceR3SpaceToESpace(voxelHit.distance, r3Dir, elipsoidRadius);
			float num3 = eVel.magnitude + 0.005f - num2;
			return ePos + eVel.normalized * (eVel.magnitude - num3);
		}
		return ePos + eVel;
	}

	private bool NoOverlapPosition(Vector3 R3Position, Vector3 R3Direction, ref Vector3 offset)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		R3Direction.Normalize();
		float num = Mathf.Sqrt(2f);
		float num2 = Vector3.Dot(R3Direction, Vector3.up);
		float num3 = 0.99f;
		Vector3 val = Vector3.up;
		if (num2 > num3 || num2 < 0f - num3)
		{
			val = Vector3.right;
		}
		Vector3 val2 = Vector3.Cross(val, R3Direction);
		Vector3 normalized = val2.normalized;
		Vector3 val3 = Vector3.Cross(normalized, R3Direction);
		Vector3 normalized2 = val3.normalized;
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = elipsoidRadius;
		if (MVElipsoidOverlapCheck.ElipsoidOverlapCheckBool(val, R3Position, Quaternion.identity, -5, ignoreAvatarId))
		{
			return true;
		}
		return false;
	}
}
