using System;
using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

public static class MVRaycast
{
	private const float HALF_VOXEL_SIZE = 0.5f;

	private const float LINE_FACET_TEST_DISTANCE = 300f;

	private static Bounds cubeBounds = new Bounds(Vector3.zero, Vector3.one);

	private static Ray intersectRay = default;

	private static HashSet<int> foundWos = new HashSet<int>();

	static MVRaycast()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
	}

	public static bool MVHit(Ray ray, MVWorldObjectClient wo, out VoxelHit voxelHit, float distance = float.PositiveInfinity)
	{
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		voxelHit = default;
		List<RaycastHit> list = new List<RaycastHit>();
		if (wo is MVCubeModelBase)
		{
			List<GameObject> chunks = ((MVCubeModelBase)wo).Chunks;
			List<Collider> list2 = new List<Collider>();
			RaycastHit item = default;
			foreach (GameObject item2 in chunks)
			{
				if (((Component)item2.transform).collider.Raycast(ray, ref item, distance))
				{
					list.Add(item);
					continue;
				}
				Bounds bounds = ((Component)item2.transform).collider.bounds;
				if (bounds.Contains(ray.origin))
				{
					list2.Add(((Component)item2.transform).collider);
				}
			}
			PhysicsCollisionData[] physicsCollisionData = SharedCollisionFunctions.GetPhysicsCollisionData(list2.ToArray(), list.ToArray(), ray.origin);
			for (int i = 0; i < physicsCollisionData.Length; i++)
			{
				if (HitDetectOnWo(ray, i, wo, physicsCollisionData, handleObjectsInsideBoxCollider: false, out voxelHit, null, distance))
				{
					return true;
				}
			}
		}
		else
		{
			if ((Object)(object)((Component)wo.GameObject.transform).collider == (Object)null)
			{
				Debug.LogWarning((object)("No collider on wo of type " + wo.GetType()));
				Debug.LogWarning((object)"Maybe a recursive check of the children is needed");
				return false;
			}
			Debug.LogWarning((object)"Remember to test positive infinity!");
			RaycastHit hit = default;
			if (((Component)wo.GameObject.transform).collider.Raycast(ray, ref hit, distance))
			{
				SharedCollisionFunctions.SetToNoneVoxelHit(ref voxelHit, new PhysicsCollisionData(hit), wo.Id);
				return true;
			}
		}
		return false;
	}

	public static List<VoxelHit> MVHitAll(Ray ray, float distance = float.PositiveInfinity, int layerMask = -5, HashSet<int> ignoreWoIds = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return MVHit(ray, all: true, distance, layerMask, ignoreWoIds);
	}

	public static bool MVHit(Ray ray, out VoxelHit voxelHit, float distance = float.PositiveInfinity, int layerMask = -5, HashSet<int> ignoreWoIds = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		voxelHit = default;
		List<VoxelHit> list = MVHit(ray, all: false, distance, layerMask, ignoreWoIds);
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

	private static List<VoxelHit> MVHit(Ray ray, bool all, float distance, int layerMask, HashSet<int> ignoreWoIds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		Vector3 direction = ray.direction;
		if (direction.sqrMagnitude == 0f)
		{
			return new List<VoxelHit>();
		}
		if (all)
		{
			foundWos.Clear();
		}
		RaycastHit[] source = Physics.RaycastAll(ray, distance, layerMask);
		Collider[] overlapResult = Physics.OverlapSphere(ray.origin, 0f);
		PhysicsCollisionData[] physicsCollisionData = SharedCollisionFunctions.GetPhysicsCollisionData(overlapResult, source.ToArray(), ray.origin);
		List<VoxelHit> list = new List<VoxelHit>();
		for (int i = 0; i < physicsCollisionData.Length; i++)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(physicsCollisionData[i].transform);
			if (!SharedCollisionFunctions.IgnoreCollision(mVObject, ignoreWoIds) && (!all || !foundWos.Contains(mVObject.Id)) && HitDetectOnWo(ray, i, mVObject, physicsCollisionData, !all, out var voxelHit, ignoreWoIds, distance))
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

	private static bool HitDetectOnWo(Ray ray, int i, MVWorldObjectClient wo, PhysicsCollisionData[] collisionData, bool handleObjectsInsideBoxCollider, out VoxelHit voxelHit, HashSet<int> ignoreWoIds, float distance = float.PositiveInfinity)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		voxelHit = default;
		if (wo is MVCubeModelBase)
		{
			if (GetCellOnRay(ray, ref voxelHit, ((Component)collisionData[i].transform).gameObject, (MVCubeModelBase)wo, collisionData[i].point, distance, wo.Scale))
			{
				SetFoundHitVariables(ref voxelHit, collisionData[i], (MVCubeModelBase)wo);
				if (handleObjectsInsideBoxCollider)
				{
					HandleObjectsInsideBoxCollider(ray, i, collisionData, ref voxelHit, ignoreWoIds, distance);
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

	private static void HandleObjectsInsideBoxCollider(Ray ray, int indexOfFirstHit, PhysicsCollisionData[] collisionData, ref VoxelHit voxelHit, HashSet<int> ignoreWoIds, float distance)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		for (int i = indexOfFirstHit + 1; i < collisionData.Length; i++)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(collisionData[i].transform);
			if (SharedCollisionFunctions.IgnoreCollision(mVObject, ignoreWoIds))
			{
				continue;
			}
			if (mVObject is MVCubeModelBase)
			{
				Vector3 val = ray.origin - voxelHit.point;
				float sqrMagnitude = val.sqrMagnitude;
				Vector3 val2 = ray.origin - collisionData[i].point;
				if (sqrMagnitude < val2.sqrMagnitude)
				{
					break;
				}
				Bounds bounds = collisionData[indexOfFirstHit].collider.bounds;
				if (!bounds.Contains(collisionData[i].point))
				{
					break;
				}
				VoxelHit vHit = default;
				if (GetCellOnRay(ray, ref vHit, ((Component)collisionData[i].transform).gameObject, (MVCubeModelBase)mVObject, collisionData[i].point, distance, mVObject.Scale))
				{
					Vector3 val3 = ray.origin - voxelHit.point;
					float sqrMagnitude2 = val3.sqrMagnitude;
					Vector3 val4 = ray.origin - vHit.point;
					if (sqrMagnitude2 > val4.sqrMagnitude)
					{
						SetFoundHitVariables(ref vHit, collisionData[i], (MVCubeModelBase)mVObject);
						SharedCollisionFunctions.SetToVoxelHit(ref voxelHit, ref vHit);
					}
				}
			}
			else if (!collisionData[i].isInsideCollider)
			{
				Vector3 val5 = ray.origin - voxelHit.point;
				float sqrMagnitude3 = val5.sqrMagnitude;
				Vector3 val6 = ray.origin - collisionData[i].point;
				if (sqrMagnitude3 > val6.sqrMagnitude)
				{
					SharedCollisionFunctions.SetToNoneVoxelHit(ref voxelHit, collisionData[i], mVObject.Id);
					break;
				}
			}
		}
	}

	private static void SetFoundHitVariables(ref VoxelHit voxelHit, PhysicsCollisionData collisionData, MVCubeModelBase cubeModelBase)
	{
		voxelHit.isCubeHit = true;
		voxelHit.woId = cubeModelBase.Id;
		voxelHit.collider = cubeModelBase.GameObject.collider;
		voxelHit.transform = cubeModelBase.GameObject.transform;
		voxelHit.interactionFlags = cubeModelBase.InteractionFlags;
	}

	private static bool IsWithinDistance(float distance, Vector3 localOrigin, IntVector voxelPos)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (distance == float.PositiveInfinity)
		{
			return true;
		}
		float num = distance * distance;
		Vector3 val = 0.5f * Vector3.one;
		float num2 = num + val.sqrMagnitude;
		Vector3 val2 = 0.5f * Vector3.one;
		float num3 = num2 + distance * val2.sqrMagnitude;
		Vector3 val3 = CubeMathFunctions.LocalIntVectorToLocalPos(voxelPos);
		Vector3 val4 = val3 - localOrigin;
		return val4.sqrMagnitude <= num3;
	}

	private static bool GetCellOnRay(Ray ray, ref VoxelHit vHit, GameObject chunk, MVCubeModelBase cmb, Vector3 hitPoint, float distance, Vector3 scale)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		intersectRay.direction = chunk.transform.InverseTransformDirection(ray.direction);
		intersectRay.origin = chunk.transform.InverseTransformPoint(ray.origin);
		float num = distance;
		if (distance != float.PositiveInfinity)
		{
			Vector3 val = MathFunctions.DivideVector(distance * intersectRay.direction, scale);
			num = val.magnitude;
		}
		Vector3 val2 = ray.origin - hitPoint;
		Vector3 val3 = MathFunctions.DivideVector(val2.magnitude * intersectRay.direction, scale);
		float magnitude = val3.magnitude;
		Vector3 val4 = intersectRay.origin + intersectRay.direction * magnitude;
		IntVector target = CubeMathFunctions.LocalPosToLocalIntVector(val4);
		IntVector min = default;
		IntVector max = default;
		SharedCollisionFunctions.GetVoxelBounds(ref min, ref max, chunk.GetComponent<MeshFilter>().sharedMesh.bounds);
		MathFunctions.ClampIntVector(ref target, min, max);
		int num2 = Math.Sign(intersectRay.direction.x);
		int num3 = Math.Sign(intersectRay.direction.y);
		int num4 = Math.Sign(intersectRay.direction.z);
		Vector3 val5 = new Vector3((float)((int)target.x + ((num2 > 0) ? 1 : 0)), (float)((int)target.y + ((num3 > 0) ? 1 : 0)), (float)((int)target.z + ((num4 > 0) ? 1 : 0)));
		Vector3 val6 = new Vector3((val5.x - val4.x - 0.5f) / intersectRay.direction.x, (val5.y - val4.y - 0.5f) / intersectRay.direction.y, (val5.z - val4.z - 0.5f) / intersectRay.direction.z);
		if (float.IsNaN(val6.x) || float.IsNegativeInfinity(val6.x))
		{
			val6.x = float.PositiveInfinity;
		}
		if (float.IsNaN(val6.y) || float.IsNegativeInfinity(val6.y))
		{
			val6.y = float.PositiveInfinity;
		}
		if (float.IsNaN(val6.z) || float.IsNegativeInfinity(val6.z))
		{
			val6.z = float.PositiveInfinity;
		}
		Vector3 val7 = new Vector3((float)num2 / intersectRay.direction.x, (float)num3 / intersectRay.direction.y, (float)num4 / intersectRay.direction.z);
		if (float.IsNaN(val7.x))
		{
			val7.x = float.PositiveInfinity;
		}
		if (float.IsNaN(val7.y))
		{
			val7.y = float.PositiveInfinity;
		}
		if (float.IsNaN(val7.z))
		{
			val7.z = float.PositiveInfinity;
		}
		while (true)
		{
			if (!IsWithinDistance(num, intersectRay.origin, target))
			{
				return false;
			}
			Cube cube = cmb.GetCube(target);
			if (cube != null && GetHitPoint(ray, cube, ref vHit, target, val4, scale, num) && Vector3.Dot(vHit.point - ray.origin, ray.direction) > 0f)
			{
				vHit.normal = chunk.transform.TransformDirection(vHit.normal);
				vHit.cubePos = target;
				vHit.cube = cube;
				return true;
			}
			if (val6.x < val6.y && val6.x < val6.z)
			{
				target.x += (short)num2;
				if (target.x < min.x || target.x > max.x)
				{
					return false;
				}
				val6.x += val7.x;
			}
			else if (val6.y < val6.z)
			{
				target.y += (short)num3;
				if (target.y < min.y || target.y > max.y)
				{
					return false;
				}
				val6.y += val7.y;
			}
			else
			{
				target.z += (short)num4;
				if (target.z < min.z || target.z > max.z)
				{
					break;
				}
				val6.z += val7.z;
			}
		}
		return false;
	}

	private static bool GetHitPoint(Ray ray, Cube cube, ref VoxelHit vHit, IntVector voxelPos, Vector3 localBoundsHitPoint, Vector3 scale, float scaledDistance)
	{
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		if (cube.HiddenSides != 63)
		{
			if (cube.UnIndentedSides == 63)
			{
				cubeBounds.center = new Vector3((float)voxelPos.x, (float)voxelPos.y, (float)voxelPos.z);
				float num = 0f;
				if (cubeBounds.IntersectRay(intersectRay, ref num))
				{
					Vector3 localDir = intersectRay.origin + intersectRay.direction * num - cubeBounds.center;
					if (num > scaledDistance)
					{
						return false;
					}
					Vector3 val = MathFunctions.MultiplyVector(num * intersectRay.direction, scale);
					vHit.distance = val.magnitude;
					vHit.point = ray.origin + ray.direction * vHit.distance;
					vHit.face = Cube.GetFaceIdentityFromLocalDir(localDir);
					vHit.normal = Cube.GetFaceAxis(vHit.face);
					return true;
				}
			}
			else
			{
				Vector3[] corners = cube.Corners;
				Vector3 val2 = new Vector3((float)voxelPos.x, (float)voxelPos.y, (float)voxelPos.z);
				Vector3 p = default;
				for (int i = 0; i < corners.Length; i++)
				{
					ref Vector3 reference = ref corners[i];
					reference += val2;
				}
				foreach (byte value in Enum.GetValues(typeof(FaceFlags)))
				{
					if ((cube.HiddenSides & value) != 0)
					{
						continue;
					}
					Face face = CubeBase.FaceFlagToFace((FaceFlags)value);
					Vector3[] face2 = Cube.GetFace(corners, CubeBase.FaceFlagToFace((FaceFlags)value));
					if (MathFunctions.LineFacetCollision(localBoundsHitPoint - intersectRay.direction, localBoundsHitPoint + intersectRay.direction * 300f, face2[0], face2[3], face2[2], intersectRay.direction, ref p, ref vHit.normal) || MathFunctions.LineFacetCollision(localBoundsHitPoint - intersectRay.direction, intersectRay.origin + intersectRay.direction * 300f, face2[2], face2[1], face2[0], intersectRay.direction, ref p, ref vHit.normal))
					{
						Vector3 val3 = intersectRay.origin - p;
						float magnitude = val3.magnitude;
						if (magnitude > scaledDistance)
						{
							return false;
						}
						Vector3 val4 = MathFunctions.MultiplyVector(magnitude * intersectRay.direction, scale);
						vHit.distance = val4.magnitude;
						vHit.point = ray.origin + ray.direction * vHit.distance;
						vHit.face = face;
						return true;
					}
				}
			}
		}
		return false;
	}
}
