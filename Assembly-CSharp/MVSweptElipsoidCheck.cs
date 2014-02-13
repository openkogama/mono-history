using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public static class MVSweptElipsoidCheck
{
	private const int RECT_NUM = 4;

	private const float SQUARE_ROOT_2 = 1.414214f;

	private const float MOVEBACK_FROM_BOX_COLLIDER = 1E-05f;

	private static bool DEBUG_MODE = false;

	private static bool DEBUG_DRAW = false;

	private static HashSet<IntVector> debugTestedIntVector = new HashSet<IntVector>();

	private static CellTraverser cellTraverser = new CellTraverser();

	private static Plane collisionPlane = default;

	private static Plane collisionPlane0 = default;

	private static Matrix4x4 worldToElipsoidSpace;

	private static Matrix4x4 elipsoidSpaceToWorld;

	private static HashSet<int> foundWos = new HashSet<int>();

	private static Vector3[] cachedCornersElipsoidSpace = new Vector3[8];

	private static Vector3[] cachedFaceElipsoidSpace = new Vector3[4];

	private static VoxelHit vhCached = default;

	static MVSweptElipsoidCheck()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
	}

	public static bool MVElipsoidCast(Ray ray, Transform transform, Bounds localBounds, float distance, out VoxelHit voxelHit, HashSet<int> ignoreWoIds = null, int layerMask = -5)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		voxelHit = default;
		List<VoxelHit> list = MVElipsoidCast(ray, transform, localBounds, all: false, distance, ignoreWoIds, layerMask);
		if (list.Count == 0)
		{
			return false;
		}
		if (list.Count > 1)
		{
			Debug.LogError((object)"Hit counter greater than 1!");
			return false;
		}
		voxelHit = list[0];
		return true;
	}

	public static List<VoxelHit> MVElipsoidCastAll(Ray ray, Transform transform, Bounds localBounds, float distance, HashSet<int> ignoreWoIds = null, int layerMask = -5)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return MVElipsoidCast(ray, transform, localBounds, all: true, distance, ignoreWoIds, layerMask);
	}

	public static bool MVElipsoidCast(Ray ray, Vector3 radius, Quaternion rotation, float distance, out VoxelHit voxelHit, HashSet<int> ignoreWoIds = null, int layerMask = -5)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		voxelHit = default;
		if (radius.x == 0f || radius.y == 0f || radius.z == 0f)
		{
			return false;
		}
		List<VoxelHit> list = MVElipsoidCast(ray, radius, rotation, distance, all: false, ignoreWoIds, layerMask);
		if (list.Count == 0)
		{
			return false;
		}
		if (list.Count > 1)
		{
			Debug.LogError((object)"Hit counter greater than 1!");
			return false;
		}
		voxelHit = list[0];
		return true;
	}

	public static List<VoxelHit> MVElipsoidCastAll(Ray ray, Vector3 radius, Quaternion rotation, float distance, HashSet<int> ignoreWoIds = null, int layerMask = -5)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return MVElipsoidCast(ray, radius, rotation, distance, all: true, ignoreWoIds, layerMask);
	}

	private static List<VoxelHit> MVElipsoidCast(Ray ray, Transform transform, Bounds localBounds, bool all, float distance, HashSet<int> ignoreWoIds, int layerMask = -5)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = transform.TransformPoint(localBounds.center);
		Vector3 radius = MathFunctions.MultiplyVector(localBounds.size / 2f, transform.localScale);
		Vector3 val2 = val - transform.position;
		ray.origin += val2;
		return MVElipsoidCast(ray, radius, transform.rotation, distance, all, ignoreWoIds, layerMask);
	}

	private static List<VoxelHit> MVElipsoidCast(Ray ray, Vector3 radius, Quaternion rotation, float distance, bool all, HashSet<int> ignoreWoIds, int layerMask = -5)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		List<VoxelHit> list = new List<VoxelHit>();
		if (ray.direction == Vector3.zero)
		{
			return list;
		}
		if (all)
		{
			foundWos.Clear();
		}
		float num = 0f;
		for (int i = 0; i < 3; i++)
		{
			if (radius[i] > num)
			{
				num = radius[i];
			}
		}
		Collider[] overlapResult = Physics.OverlapSphere(ray.origin, num, layerMask);
		RaycastHit[] hits = Physics.SphereCastAll(ray, num, distance, layerMask);
		PhysicsCollisionData[] physicsCollisionData = SharedCollisionFunctions.GetPhysicsCollisionData(overlapResult, hits, ray.origin);
		for (int j = 0; j < physicsCollisionData.Length; j++)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(physicsCollisionData[j].transform);
			if (!SharedCollisionFunctions.IgnoreCollision(mVObject, ignoreWoIds) && (!all || !foundWos.Contains(mVObject.Id)) && SphereHitDetectOnWo(ray, radius, rotation, num, distance, j, mVObject, physicsCollisionData, !all, out var voxelHit, ignoreWoIds))
			{
				list.Add(voxelHit);
				foundWos.Add(mVObject.Id);
				if (!all)
				{
					return list;
				}
			}
		}
		return list;
	}

	private static bool SphereHitDetectOnWo(Ray ray, Vector3 radius, Quaternion rotation, float maxRadius, float distance, int i, MVWorldObjectClient wo, PhysicsCollisionData[] collisionData, bool handleObjectsInsideBoxCollider, out VoxelHit voxelHit, HashSet<int> ignoreWoIds)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		voxelHit = default;
		if (wo is MVCubeModelBase)
		{
			elipsoidSpaceToWorld = Matrix4x4.TRS(Vector3.zero, rotation, radius);
			worldToElipsoidSpace = elipsoidSpaceToWorld.inverse;
			Ray[] boundRays = GetBoundRays(ray, (MVCubeModelBase)wo);
			if (LayerScan(ref voxelHit, radius, maxRadius, ((Component)collisionData[i].transform).gameObject, distance, (MVCubeModelBase)wo, collisionData[i], ray, boundRays))
			{
				if (handleObjectsInsideBoxCollider)
				{
					HandleObjectsInsideBoxCollider(ray, radius, rotation, maxRadius, distance, i, collisionData, ref voxelHit, boundRays, ignoreWoIds);
				}
				return true;
			}
		}
		else if (!collisionData[i].isInsideCollider)
		{
			SharedCollisionFunctions.SetToNoneVoxelHit(ref voxelHit, collisionData[i], wo.Id);
			return true;
		}
		return false;
	}

	private static void HandleObjectsInsideBoxCollider(Ray ray, Vector3 radius, Quaternion rotation, float maxRadius, float distance, int indexOfFirstHit, PhysicsCollisionData[] collisionData, ref VoxelHit voxelHit, Ray[] boundRays, HashSet<int> ignoreWoIds)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		for (int i = indexOfFirstHit + 1; i < collisionData.Length; i++)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(collisionData[i].transform);
			if (SharedCollisionFunctions.IgnoreCollision(mVObject, ignoreWoIds))
			{
				continue;
			}
			if (mVObject is MVCubeModelBase)
			{
				if (voxelHit.distance <= collisionData[i].distance)
				{
					break;
				}
				VoxelHit vh = default;
				if (LayerScan(ref vh, radius, maxRadius, ((Component)collisionData[i].transform).gameObject, distance, (MVCubeModelBase)mVObject, collisionData[i], ray, boundRays) && voxelHit.distance > vh.distance)
				{
					SharedCollisionFunctions.SetToVoxelHit(ref voxelHit, ref vh);
				}
			}
			else if (!collisionData[i].isInsideCollider && voxelHit.distance > collisionData[i].distance)
			{
				SharedCollisionFunctions.SetToNoneVoxelHit(ref voxelHit, collisionData[i], mVObject.Id);
				break;
			}
		}
	}

	private static bool LayerScan(ref VoxelHit vh, Vector3 radius, float maxRadius, GameObject chunk, float distance, MVCubeModelBase wo, PhysicsCollisionData collisionData, Ray ray, Ray[] boundRays)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		CollisionState collisionState = new CollisionState
		{
			firstHitScanAxis = -1,
			firstHitDetected = false,
			minBounds = default,
			maxBounds = default
		};
		SharedCollisionFunctions.GetVoxelBounds(ref collisionState.minBounds, ref collisionState.maxBounds, chunk.GetComponent<MeshFilter>().sharedMesh.bounds);
		collisionState.localHitPoint = collisionData.transform.InverseTransformPoint(collisionData.point);
		collisionState.localNormal = collisionData.transform.InverseTransformDirection(collisionData.normal);
		collisionState.localDirection = collisionData.transform.InverseTransformDirection(ray.direction);
		collisionState.localToElipsoidSpace = worldToElipsoidSpace * chunk.transform.localToWorldMatrix;
		collisionState.cmb = wo;
		Vector3 one = Vector3.one;
		Vector3 vec = one.normalized * maxRadius;
		vec = MathFunctions.DivideVector(vec, collisionState.cmb.Scale);
		collisionState.scaledMaxRadius = vec.magnitude;
		collisionState.origin = ray.origin;
		collisionState.direction = ray.direction;
		float num = 0f;
		float num2 = distance;
		if (!collisionData.isInsideCollider)
		{
			collisionState.origin = ray.origin + ray.direction * (collisionData.distance - 1E-05f);
			Vector3 val = ray.origin - collisionState.origin;
			num = val.magnitude;
			num2 -= num;
		}
		collisionState.elipsoidSpaceOrigin = worldToElipsoidSpace.MultiplyPoint(collisionState.origin);
		Vector3 val2 = worldToElipsoidSpace.MultiplyVector(collisionState.direction);
		collisionState.elipsoidSpaceDirection = val2.normalized;
		Vector3 val3 = worldToElipsoidSpace.MultiplyVector(collisionState.direction * num2);
		collisionState.elipsoidSpaceDistance = val3.magnitude;
		collisionState.localOrigin = collisionData.transform.InverseTransformPoint(collisionState.origin);
		Vector3 planeNormal = GetPlaneNormal(collisionState.localDirection);
		if (DEBUG_DRAW)
		{
			Debug.DrawLine(collisionState.origin, collisionState.origin + planeNormal, Color.red, 1f);
		}
		if (planeNormal == Vector3.zero)
		{
			Debug.Log((object)"scanRect cant be zero");
		}
		collisionState.scanAxis = GetScanAxis(planeNormal);
		collisionPlane.SetNormalAndPosition(collisionData.transform.TransformDirection(planeNormal), collisionData.point);
		Vector3[] raysProjectedOnPlane = GetRaysProjectedOnPlane(boundRays, ref collisionPlane, collisionData.point, collisionData.transform.TransformDirection(planeNormal));
		if (DEBUG_DRAW)
		{
			Debug.DrawLine(ray.origin, boundRays[0].origin, Color.red);
			Debug.DrawLine(ray.origin, boundRays[1].origin, Color.red);
			Debug.DrawLine(ray.origin, boundRays[2].origin, Color.red);
			Debug.DrawLine(ray.origin, boundRays[3].origin, Color.red);
			Debug.DrawLine(ray.origin, ray.origin + Vector3.up, Color.black);
			Debug.DrawLine(raysProjectedOnPlane[0], raysProjectedOnPlane[2]);
			Debug.DrawLine(raysProjectedOnPlane[1], raysProjectedOnPlane[3]);
			Debug.DrawLine(collisionData.point, collisionData.point + Vector3.left, Color.red);
			Debug.DrawLine(collisionData.point, collisionData.point + collisionData.transform.TransformDirection(planeNormal), Color.red);
		}
		if (collisionData.isInsideCollider)
		{
			MoveAxisAlignedRectBackward(raysProjectedOnPlane, ray.origin, ray.direction);
		}
		Vector3[] array = CalculateAxisAlignedRect(raysProjectedOnPlane, collisionData.transform);
		if (!collisionData.isInsideCollider)
		{
			MoveAxisAlignedRectOutOfBox(array, collisionState.localNormal, collisionState.localHitPoint, collisionState.localDirection);
		}
		if (DEBUG_DRAW)
		{
			DrawAxisAlignedRect(array[0], array[1], collisionState.scanAxis, chunk.transform, array[0][collisionState.scanAxis]);
		}
		if (LayerScan(radius, array, ref vh, num2, ref collisionState))
		{
			Vector3 val4 = elipsoidSpaceToWorld.MultiplyVector(collisionState.elipsoidSpaceDirection * vh.distance);
			float magnitude = val4.magnitude;
			vh.distance = magnitude;
			vh.distance += num;
			SetFoundHitVariables(ref vh, collisionData, wo.Id, wo.InteractionFlags);
			return true;
		}
		return false;
	}

	private static bool LayerScan(Vector3 radius, Vector3[] alignedRect, ref VoxelHit vh, float distance, ref CollisionState collisionState)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		vh.distance = float.PositiveInfinity;
		Vector3 val = alignedRect[0];
		Vector3 val2 = alignedRect[0];
		Vector3 val3 = alignedRect[0];
		bool flag = false;
		for (int i = 0; i < 3; i++)
		{
			if (i != collisionState.scanAxis && !flag)
			{
				val2[i] = alignedRect[1][i];
				flag = true;
			}
			else if (i != collisionState.scanAxis && flag)
			{
				val3[i] = alignedRect[1][i];
			}
		}
		Vector3 val4 = val2 - val;
		Vector3 normalized = val4.normalized;
		Vector3 val5 = val3 - val;
		Vector3 normalized2 = val5.normalized;
		IntVector intVector = CubeMathFunctions.LocalPosToLocalIntVector(normalized);
		IntVector intVector2 = CubeMathFunctions.LocalPosToLocalIntVector(normalized2);
		cellTraverser.Init(val, collisionState);
		IntVector intVector3 = CubeMathFunctions.LocalPosToLocalIntVector(val2);
		IntVector intVector4 = CubeMathFunctions.LocalPosToLocalIntVector(val3);
		intVector3[collisionState.scanAxis] = (short)Mathf.Clamp((int)intVector3[collisionState.scanAxis], (int)collisionState.minBounds[collisionState.scanAxis], (int)collisionState.maxBounds[collisionState.scanAxis]);
		intVector4[collisionState.scanAxis] = (short)Mathf.Clamp((int)intVector3[collisionState.scanAxis], (int)collisionState.minBounds[collisionState.scanAxis], (int)collisionState.maxBounds[collisionState.scanAxis]);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		for (int j = 0; j < 3; j++)
		{
			if (intVector[j] != 0)
			{
				num = Mathf.Abs(intVector3[j] - cellTraverser.VoxelPos[j]);
				num3 = j;
			}
			if (intVector2[j] != 0)
			{
				num2 = Mathf.Abs(intVector4[j] - cellTraverser.VoxelPos[j]);
				num4 = j;
			}
		}
		num++;
		num2++;
		IntVector intVector5 = new IntVector(cellTraverser.VoxelPos.x, cellTraverser.VoxelPos.y, cellTraverser.VoxelPos.z);
		int num5 = 0;
		int num6 = 1000;
		if (DEBUG_MODE)
		{
			debugTestedIntVector.Clear();
		}
		while (intVector5[collisionState.scanAxis] <= collisionState.maxBounds[collisionState.scanAxis] && intVector5[collisionState.scanAxis] >= collisionState.minBounds[collisionState.scanAxis])
		{
			if (DEBUG_MODE && num5 > num6)
			{
				Debug.Log((object)("scanAxis" + collisionState.scanAxis));
				Debug.Log((object)("testVector " + intVector5));
				Debug.Log((object)("testVector[scanAxis] " + intVector5[collisionState.scanAxis]));
				Debug.Log((object)("min[scanAxis] " + collisionState.minBounds[collisionState.scanAxis]));
				Debug.Log((object)("max[scanAxis] " + collisionState.maxBounds[collisionState.scanAxis]));
				cellTraverser.DebugAll();
				Debug.LogError((object)"While loop did not exit");
				break;
			}
			num5++;
			for (int k = 0; k <= num; k++)
			{
				for (int l = 0; l <= num2; l++)
				{
					HandleCube(ref vh, intVector5, radius, distance, ref collisionState);
					intVector5 += intVector2;
				}
				intVector5 -= (num2 + 1) * intVector2;
				intVector5 += intVector;
			}
			cellTraverser.Step();
			int num7 = 0;
			int num8 = 1000;
			while (cellTraverser.StepDir[collisionState.scanAxis] == 0)
			{
				if (DEBUG_MODE)
				{
					if (num7 > num8)
					{
						Debug.Log((object)("scanAxis " + collisionState.scanAxis));
						Debug.Log((object)("testVector " + intVector5));
						Debug.Log((object)("testVector[scanAxis] " + intVector5[collisionState.scanAxis]));
						Debug.Log((object)("min[scanAxis] " + collisionState.minBounds[collisionState.scanAxis]));
						Debug.Log((object)("max[scanAxis] " + collisionState.maxBounds[collisionState.scanAxis]));
						cellTraverser.DebugAll();
						Debug.LogError((object)"While loop did not exit");
						break;
					}
					num7++;
				}
				intVector5.x = cellTraverser.VoxelPos.x;
				intVector5.y = cellTraverser.VoxelPos.y;
				intVector5.z = cellTraverser.VoxelPos.z;
				bool flag2 = false;
				for (int m = 0; m < 3; m++)
				{
					if (intVector[m] != 0 && cellTraverser.StepDir[m] == 0)
					{
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					if (cellTraverser.StepDir[num4] == intVector2[num4])
					{
						int key2;
						int key = (key2 = num4);
						short num9 = intVector5[key2];
						intVector5[key] = (short)(num9 + (short)num2);
					}
					for (int n = 0; n <= num; n++)
					{
						HandleCube(ref vh, intVector5, radius, distance, ref collisionState);
						intVector5 += intVector;
					}
				}
				else
				{
					if (cellTraverser.StepDir[num3] == intVector[num3])
					{
						int key2;
						int key3 = (key2 = num3);
						short num9 = intVector5[key2];
						intVector5[key3] = (short)(num9 + (short)num);
					}
					for (int num10 = 0; num10 <= num2; num10++)
					{
						HandleCube(ref vh, intVector5, radius, distance, ref collisionState);
						intVector5 += intVector2;
					}
				}
				cellTraverser.Step();
			}
			intVector5.x = cellTraverser.VoxelPos.x;
			intVector5.y = cellTraverser.VoxelPos.y;
			intVector5.z = cellTraverser.VoxelPos.z;
			if (collisionState.firstHitDetected && Mathf.Abs(intVector5[collisionState.scanAxis] - collisionState.firstHitScanAxis) > Mathf.CeilToInt(collisionState.scaledMaxRadius))
			{
				break;
			}
		}
		return collisionState.firstHitDetected;
	}

	private static void HandleCube(ref VoxelHit vh, IntVector pos, Vector3 radius, float distance, ref CollisionState collisionState)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		if (DEBUG_MODE)
		{
			IntVector item = new IntVector(pos.x, pos.y, pos.z);
			if (debugTestedIntVector.Contains(item))
			{
				Debug.LogWarning((object)("testVector allReady tested " + pos));
			}
			debugTestedIntVector.Add(item);
		}
		bool flag = false;
		if (IsWithinBounds(pos, ref collisionState) && CubeIsWithinSphereRadius(pos, ref collisionState))
		{
			Cube cube = collisionState.cmb.GetCube(pos);
			if (cube != null && cube.HiddenSides != 63)
			{
				CubeBase.GetCorners(cube, ref cachedCornersElipsoidSpace);
				Vector3 val = new Vector3((float)pos.x, (float)pos.y, (float)pos.z);
				for (int i = 0; i < cachedCornersElipsoidSpace.Length; i++)
				{
					ref Vector3 reference = ref cachedCornersElipsoidSpace[i];
					reference += val;
					ref Vector3 reference2 = ref cachedCornersElipsoidSpace[i];
					reference2 = collisionState.localToElipsoidSpace.MultiplyPoint(cachedCornersElipsoidSpace[i]);
				}
				FaceFlags[] faceFlagsArray = CubeBase.FaceFlagsArray;
				foreach (FaceFlags faceFlags in faceFlagsArray)
				{
					if (((uint)cube.HiddenSides & (uint)faceFlags) == 0)
					{
						Face face = CubeBase.FaceFlagToFace(faceFlags);
						CubeBase.GetFace(ref cachedCornersElipsoidSpace, ref cachedFaceElipsoidSpace, CubeBase.FaceFlagToFace(faceFlags));
						flag |= HandleTriangleTest(cachedFaceElipsoidSpace[0], cachedFaceElipsoidSpace[3], cachedFaceElipsoidSpace[2], ref vh, distance, face, cube, radius, pos, collisionState);
						flag |= HandleTriangleTest(cachedFaceElipsoidSpace[2], cachedFaceElipsoidSpace[1], cachedFaceElipsoidSpace[0], ref vh, distance, face, cube, radius, pos, collisionState);
					}
				}
			}
		}
		if (flag && !collisionState.firstHitDetected)
		{
			collisionState.firstHitScanAxis = pos[collisionState.scanAxis];
			collisionState.firstHitDetected = true;
		}
	}

	private static bool HandleTriangleTest(Vector3 p1, Vector3 p2, Vector3 p3, ref VoxelHit currentVoxelHit, float distance, Face face, Cube cube, Vector3 radiusVec, IntVector pos, CollisionState collisionState)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		vhCached.distance = float.PositiveInfinity;
		bool flag = TriangleCheck.CheckTriangle(p1, p2, p3, collisionState.elipsoidSpaceOrigin, collisionState.elipsoidSpaceDirection, collisionState.elipsoidSpaceDistance, ref vhCached);
		if (flag && vhCached.distance > collisionState.elipsoidSpaceDistance)
		{
			Debug.Log((object)"this is wrong");
		}
		if (flag && vhCached.distance < currentVoxelHit.distance)
		{
			Vector3 val = collisionState.elipsoidSpaceOrigin - vhCached.point;
			if (val.sqrMagnitude >= 1f)
			{
				currentVoxelHit.distance = vhCached.distance;
				currentVoxelHit.point = elipsoidSpaceToWorld.MultiplyPoint(vhCached.point);
				currentVoxelHit.face = face;
				currentVoxelHit.normal = MathFunctions.GetNormal(elipsoidSpaceToWorld.MultiplyPoint(p1), elipsoidSpaceToWorld.MultiplyPoint(p2), elipsoidSpaceToWorld.MultiplyPoint(p3));
				currentVoxelHit.cube = cube;
				currentVoxelHit.cubePos = new IntVector(pos.x, pos.y, pos.z);
				return true;
			}
		}
		return false;
	}

	private static void SetFoundHitVariables(ref VoxelHit voxelHit, PhysicsCollisionData collisionData, int woId, InteractionFlags interactionFlag)
	{
		voxelHit.isCubeHit = true;
		voxelHit.woId = woId;
		voxelHit.collider = collisionData.collider;
		voxelHit.transform = collisionData.transform;
		voxelHit.interactionFlags = interactionFlag;
	}

	private static void MoveAxisAlignedRectBackward(Vector3[] pointsOnProjectPlane, Vector3 startOrigin, Vector3 dir)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		Plane val = new Plane(dir, startOrigin);
		float num = 0f;
		Ray val2 = new Ray(Vector3.zero, -dir);
		foreach (Vector3 val3 in pointsOnProjectPlane)
		{
			if (val.GetSide(val3))
			{
				val2.origin = val3;
				float num2 = 0f;
				if (val.Raycast(val2, ref num2) && num2 > num)
				{
					num = num2;
				}
			}
		}
		if (num > 0f)
		{
			Vector3 val4 = -dir * num;
			for (int j = 0; j < pointsOnProjectPlane.Length; j++)
			{
				ref Vector3 reference = ref pointsOnProjectPlane[j];
				reference += val4;
			}
		}
	}

	private static void DebugTest()
	{
	}

	private static bool IsWithinBounds(IntVector pos, ref CollisionState collisionState)
	{
		if (pos.x >= collisionState.minBounds.x && pos.y >= collisionState.minBounds.y && pos.z >= collisionState.minBounds.z && pos.x <= collisionState.maxBounds.x && pos.y <= collisionState.maxBounds.y && pos.z <= collisionState.maxBounds.z)
		{
			return true;
		}
		return false;
	}

	private static void MoveAxisAlignedRectOutOfBox(Vector3[] axisAlignedRect, Vector3 localNormal, Vector3 localHit, Vector3 localDir)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		collisionPlane0.SetNormalAndPosition(-localNormal, localHit);
		float num = 0f;
		Ray val = new Ray(Vector3.zero, -localDir);
		float num2 = default;
		foreach (Vector3 origin in axisAlignedRect)
		{
			val.origin = origin;
			if (collisionPlane0.Raycast(val, ref num2) && num2 > num)
			{
				num = num2;
			}
		}
		if (num != 0f)
		{
			localDir *= 0f - num;
			for (int j = 0; j < axisAlignedRect.Length; j++)
			{
				ref Vector3 reference = ref axisAlignedRect[j];
				reference += localDir;
			}
		}
	}

	private static bool CubeIsWithinFirstHitRadius(IntVector pos, float moveDistance, ref CollisionState collisionState)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (!collisionState.firstHitDetected)
		{
			return true;
		}
		float num = 1.414214f;
		float num2 = num + collisionState.scaledMaxRadius;
		Vector3 val = new Vector3((float)pos.x, (float)pos.y, (float)pos.z);
		moveDistance *= collisionState.scaledMaxRadius;
		Vector3 val2 = collisionState.localOrigin + collisionState.localDirection * moveDistance;
		Vector3 val3 = val - val2;
		if (val3.sqrMagnitude < num2 * num2)
		{
			return true;
		}
		return false;
	}

	private static bool CubeIsWithinSphereRadius(IntVector pos, ref CollisionState collisionState)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		float num = 1.414214f;
		float num2 = num + collisionState.scaledMaxRadius;
		Vector3 point = new Vector3((float)pos.x, (float)pos.y, (float)pos.z);
		float distance = 0f;
		MathFunctions.DistancePointLine(point, collisionState.localOrigin - collisionState.localDirection * 100f, collisionState.localOrigin + collisionState.localDirection * 100f, ref distance);
		if (distance <= num2)
		{
			return true;
		}
		return false;
	}

	public static Vector3 GetPlaneNormal(Vector3 localDir)
	{
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		if (Mathf.Abs(localDir.x) >= Mathf.Abs(localDir.y) && Mathf.Abs(localDir.x) >= Mathf.Abs(localDir.z))
		{
			localDir.y = 0f;
			localDir.z = 0f;
			if (localDir.x >= 0f)
			{
				localDir.x = Mathf.Ceil(localDir.x);
			}
			else
			{
				localDir.x = Mathf.Floor(localDir.x);
			}
		}
		else if (Mathf.Abs(localDir.y) >= Mathf.Abs(localDir.x) && Mathf.Abs(localDir.y) >= Mathf.Abs(localDir.z))
		{
			localDir.x = 0f;
			localDir.z = 0f;
			if (localDir.y >= 0f)
			{
				localDir.y = Mathf.Ceil(localDir.y);
			}
			else
			{
				localDir.y = Mathf.Floor(localDir.y);
			}
		}
		else
		{
			localDir.y = 0f;
			localDir.x = 0f;
			if (localDir.z >= 0f)
			{
				localDir.z = Mathf.Ceil(localDir.z);
			}
			else
			{
				localDir.z = Mathf.Floor(localDir.z);
			}
		}
		localDir *= -1f;
		return localDir;
	}

	private static bool RayCast(Ray ray, ref Vector3 hit, Plane collPlane)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		float num = default;
		if (collPlane.Raycast(ray, ref num))
		{
			Vector3 origin = ray.origin;
			Vector3 direction = ray.direction;
			hit = origin + direction.normalized * num;
			return true;
		}
		return false;
	}

	private static Vector3[] CalculateAxisAlignedRect(Vector3[] hitsClockwise, Transform t)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		if (hitsClockwise.Length != 4)
		{
			return null;
		}
		for (int i = 0; i < 4; i++)
		{
		}
		List<Vector3> list = new List<Vector3>();
		for (int j = 0; j < 4; j++)
		{
			Vector3 item = t.InverseTransformPoint(hitsClockwise[j]);
			list.Add(item);
		}
		Vector3 val2;
		Vector3 val = (val2 = list[0]);
		for (int k = 1; k < 4; k++)
		{
			for (int l = 0; l < 3; l++)
			{
				Vector3 val3 = list[k];
				if (val3[l] > val2[l])
				{
					int num = l;
					Vector3 val4 = list[k];
					val2[num] = val4[l];
				}
				Vector3 val5 = list[k];
				if (val5[l] < val[l])
				{
					int num2 = l;
					Vector3 val6 = list[k];
					val[num2] = val6[l];
				}
			}
		}
		return new Vector3[2] { val, val2 };
	}

	private static Vector3 GetMaxAngleLocalAxisVector(Ray ray, MVCubeModelBase cmb)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = new Vector3[3]
		{
			Vector3.up,
			Vector3.forward,
			Vector3.right
		};
		Vector3 result = Vector3.zero;
		Vector3 val = Vector3.zero;
		float num = 1f;
		for (int i = 0; i < 3; i++)
		{
			val = cmb.WorldRotation * array[i];
			float num2 = Mathf.Abs(Vector3.Dot(ray.direction, val.normalized));
			if (num2 < num)
			{
				result = val;
				num = num2;
			}
		}
		return result;
	}

	private static Ray[] GetBoundRays(Ray ray, MVCubeModelBase cmb)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		Vector3 maxAngleLocalAxisVector = GetMaxAngleLocalAxisVector(ray, cmb);
		Vector3 val = worldToElipsoidSpace.MultiplyVector(ray.direction);
		Vector3 val2 = worldToElipsoidSpace.MultiplyVector(maxAngleLocalAxisVector);
		Vector3 val3 = Vector3.Cross(val, val2);
		Vector3 normalized = val3.normalized;
		Vector3 val4 = Vector3.Cross(val, normalized);
		Vector3 normalized2 = val4.normalized;
		Vector3[] array = new Vector3[4]
		{
			normalized + normalized2,
			normalized - normalized2,
			-normalized + normalized2,
			-normalized - normalized2
		};
		Ray[] array2 = new Ray[4];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i].origin = elipsoidSpaceToWorld.MultiplyVector(array[i]) + ray.origin;
			array2[i].direction = ray.direction;
		}
		return array2;
	}

	public static Vector3[] GetRaysProjectedOnPlane(Ray[] boundRays, ref Plane projectPlane, Vector3 planeOrigin, Vector3 planeNormal)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = new Vector3[4]
		{
			boundRays[0].origin,
			boundRays[1].origin,
			boundRays[2].origin,
			boundRays[3].origin
		};
		bool flag = false;
		for (int i = 0; i < 4; i++)
		{
			if (!RayCast(boundRays[i], ref array[i], projectPlane))
			{
				Ray ray = new Ray(boundRays[i].origin, -boundRays[i].direction);
				if (RayCast(ray, ref array[i], projectPlane))
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
		}
		if (!flag)
		{
		}
		return array;
	}

	private static int GetScanAxis(Vector3 scanRectNormal)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 3; i++)
		{
			if (Mathf.Abs(scanRectNormal[i]) > 0.9f)
			{
				return i;
			}
		}
		Debug.LogError((object)("No scan axis found " + scanRectNormal));
		return -1;
	}

	private static void DrawAxisAlignedRect(Vector3 min, Vector3 max, int ignoreAxis, Transform t, float localIgnoreAxisValue)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		Vector2 to = default;
		Vector2 to2 = to;
		MathFunctions.Vector3ToVector2(ref min, ref to2, ignoreAxis);
		MathFunctions.Vector3ToVector2(ref max, ref to, ignoreAxis);
		List<Vector2> list = new List<Vector2>();
		list.Add(to2);
		list.Add(new Vector2(to2.x, to.y));
		list.Add(to);
		list.Add(new Vector2(to.x, to2.y));
		List<Vector2> list2 = list;
		List<Vector3> list3 = new List<Vector3>();
		for (int i = 0; i < 4; i++)
		{
			Vector3 to3 = default;
			Vector2 from = list2[i];
			MathFunctions.Vector2ToVector3(ref from, ref to3, ignoreAxis, localIgnoreAxisValue);
			list3.Add(t.TransformPoint(to3));
		}
		for (int j = 0; j < 4; j++)
		{
			Debug.DrawLine(list3[j % 4], list3[j % 4] + list3[(j + 1) % 4] - list3[j % 4]);
		}
	}
}
