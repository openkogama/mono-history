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

	private static Vector3[] cornerPointsLocalSpaceCalculateAxisAlignedRect = new Vector3[4];

	private static Vector3[] minMaxCalculateAxisAlignedRect = new Vector3[2];

	private static Vector3[] testVectorGetMaxAngleLocalAxisVector = new Vector3[3]
	{
		Vector3.up,
		Vector3.forward,
		Vector3.right
	};

	private static Ray[] raysGetBoundRays = new Ray[4];

	private static Vector3[] circleCornersGetBoundRays = new Vector3[4];

	private static readonly Vector3[] pointsOnPlane = new Vector3[4];

	public static bool MVElipsoidCast(Ray ray, Transform transform, Bounds localBounds, float distance, out VoxelHit voxelHit, HashSet<int> ignoreWoIds = null, int layerMask = -5)
	{
		voxelHit = default;
		List<VoxelHit> list = MVElipsoidCast(ray, transform, localBounds, all: false, distance, ignoreWoIds, layerMask);
		if (list.Count == 0)
		{
			return false;
		}
		if (list.Count > 1)
		{
			Debug.LogError("Hit counter greater than 1!");
			return false;
		}
		voxelHit = list[0];
		return true;
	}

	public static List<VoxelHit> MVElipsoidCastAll(Ray ray, Transform transform, Bounds localBounds, float distance, HashSet<int> ignoreWoIds = null, int layerMask = -5)
	{
		return MVElipsoidCast(ray, transform, localBounds, all: true, distance, ignoreWoIds, layerMask);
	}

	public static bool MVElipsoidCast(Ray ray, Vector3 radius, Quaternion rotation, float distance, out VoxelHit voxelHit, HashSet<int> ignoreWoIds = null, int layerMask = -5)
	{
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
			Debug.LogError("Hit counter greater than 1!");
			return false;
		}
		voxelHit = list[0];
		return true;
	}

	public static List<VoxelHit> MVElipsoidCastAll(Ray ray, Vector3 radius, Quaternion rotation, float distance, HashSet<int> ignoreWoIds = null, int layerMask = -5)
	{
		return MVElipsoidCast(ray, radius, rotation, distance, all: true, ignoreWoIds, layerMask);
	}

	private static List<VoxelHit> MVElipsoidCast(Ray ray, Transform transform, Bounds localBounds, bool all, float distance, HashSet<int> ignoreWoIds, int layerMask = -5)
	{
		Vector3 vector = transform.TransformPoint(localBounds.center);
		Vector3 radius = MathFunctions.MultiplyVector(localBounds.size / 2f, transform.localScale);
		Vector3 vector2 = vector - transform.position;
		ray.origin += vector2;
		return MVElipsoidCast(ray, radius, transform.rotation, distance, all, ignoreWoIds, layerMask);
	}

	private static List<VoxelHit> MVElipsoidCast(Ray ray, Vector3 radius, Quaternion rotation, float distance, bool all, HashSet<int> ignoreWoIds, int layerMask = -5)
	{
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
		PhysicsCollisionDatasWrapper physicsCollisionData = SharedCollisionFunctions.GetPhysicsCollisionData(overlapResult, hits, ray.origin);
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

	private static bool SphereHitDetectOnWo(Ray ray, Vector3 radius, Quaternion rotation, float maxRadius, float distance, int i, MVWorldObjectClient wo, PhysicsCollisionDatasWrapper collisionData, bool handleObjectsInsideBoxCollider, out VoxelHit voxelHit, HashSet<int> ignoreWoIds)
	{
		voxelHit = default;
		if (wo is ICubeModelCollider)
		{
			elipsoidSpaceToWorld = Matrix4x4.TRS(Vector3.zero, rotation, radius);
			worldToElipsoidSpace = elipsoidSpaceToWorld.inverse;
			GetBoundRays(ray, (ICubeModelCollider)wo);
			if (LayerScan(ref voxelHit, radius, maxRadius, collisionData[i].transform.gameObject, distance, (ICubeModelCollider)wo, collisionData[i], ray, raysGetBoundRays))
			{
				if (handleObjectsInsideBoxCollider)
				{
					HandleObjectsInsideBoxCollider(ray, radius, rotation, maxRadius, distance, i, collisionData, ref voxelHit, raysGetBoundRays, ignoreWoIds);
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

	private static void HandleObjectsInsideBoxCollider(Ray ray, Vector3 radius, Quaternion rotation, float maxRadius, float distance, int indexOfFirstHit, PhysicsCollisionDatasWrapper collisionData, ref VoxelHit voxelHit, Ray[] boundRays, HashSet<int> ignoreWoIds)
	{
		for (int i = indexOfFirstHit + 1; i < collisionData.Length; i++)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(collisionData[i].transform);
			if (SharedCollisionFunctions.IgnoreCollision(mVObject, ignoreWoIds))
			{
				continue;
			}
			if (mVObject is ICubeModelCollider)
			{
				if (voxelHit.distance <= collisionData[i].distance)
				{
					break;
				}
				VoxelHit vh = default;
				if (LayerScan(ref vh, radius, maxRadius, collisionData[i].transform.gameObject, distance, (ICubeModelCollider)mVObject, collisionData[i], ray, boundRays) && voxelHit.distance > vh.distance)
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

	private static bool LayerScan(ref VoxelHit vh, Vector3 radius, float maxRadius, GameObject chunk, float distance, ICubeModelCollider wo, PhysicsCollisionData collisionData, Ray ray, Ray[] boundRays)
	{
		CollisionState collisionState = new CollisionState
		{
			firstHitScanAxis = -1,
			firstHitDetected = false,
			minBounds = default,
			maxBounds = default
		};
		BoxCollider component = chunk.GetComponent<BoxCollider>();
		SharedCollisionFunctions.GetVoxelBounds(localSpaceBounds: new Bounds(component.center, component.size), min: ref collisionState.minBounds, max: ref collisionState.maxBounds);
		collisionState.localHitPoint = collisionData.transform.InverseTransformPoint(collisionData.point);
		collisionState.localNormal = collisionData.transform.InverseTransformDirection(collisionData.normal);
		collisionState.localDirection = collisionData.transform.InverseTransformDirection(ray.direction);
		collisionState.localToElipsoidSpace = worldToElipsoidSpace * chunk.transform.localToWorldMatrix;
		collisionState.cmb = wo;
		Vector3 vec = Vector3.one.normalized * maxRadius;
		collisionState.scaledMaxRadius = MathFunctions.DivideVector(vec, collisionState.cmb.Scale).magnitude;
		collisionState.origin = ray.origin;
		collisionState.direction = ray.direction;
		float num = 0f;
		float num2 = distance;
		if (!collisionData.isInsideCollider)
		{
			collisionState.origin = ray.origin + ray.direction * (collisionData.distance - 1E-05f);
			num = (ray.origin - collisionState.origin).magnitude;
			num2 -= num;
		}
		collisionState.elipsoidSpaceOrigin = worldToElipsoidSpace.MultiplyPoint(collisionState.origin);
		collisionState.elipsoidSpaceDirection = worldToElipsoidSpace.MultiplyVector(collisionState.direction).normalized;
		collisionState.elipsoidSpaceDistance = worldToElipsoidSpace.MultiplyVector(collisionState.direction * num2).magnitude;
		collisionState.localOrigin = collisionData.transform.InverseTransformPoint(collisionState.origin);
		Vector3 planeNormal = GetPlaneNormal(collisionState.localDirection);
		if (DEBUG_DRAW)
		{
			Debug.DrawLine(collisionState.origin, collisionState.origin + planeNormal, Color.red, 1f);
		}
		if (planeNormal == Vector3.zero)
		{
			Debug.Log("scanRect cant be zero");
		}
		collisionState.scanAxis = GetScanAxis(planeNormal);
		collisionPlane.SetNormalAndPosition(collisionData.transform.TransformDirection(planeNormal), collisionData.point);
		GetRaysProjectedOnPlane(boundRays, ref collisionPlane, collisionData.point, collisionData.transform.TransformDirection(planeNormal));
		if (DEBUG_DRAW)
		{
			Debug.DrawLine(ray.origin, boundRays[0].origin, Color.red);
			Debug.DrawLine(ray.origin, boundRays[1].origin, Color.red);
			Debug.DrawLine(ray.origin, boundRays[2].origin, Color.red);
			Debug.DrawLine(ray.origin, boundRays[3].origin, Color.red);
			Debug.DrawLine(ray.origin, ray.origin + Vector3.up, Color.black);
			Debug.DrawLine(pointsOnPlane[0], pointsOnPlane[2]);
			Debug.DrawLine(pointsOnPlane[1], pointsOnPlane[3]);
			Debug.DrawLine(collisionData.point, collisionData.point + Vector3.left, Color.red);
			Debug.DrawLine(collisionData.point, collisionData.point + collisionData.transform.TransformDirection(planeNormal), Color.red);
		}
		if (collisionData.isInsideCollider)
		{
			MoveAxisAlignedRectBackward(pointsOnPlane, ray.origin, ray.direction);
		}
		CalculateAxisAlignedRect(pointsOnPlane, collisionData.transform);
		if (!collisionData.isInsideCollider)
		{
			MoveAxisAlignedRectOutOfBox(minMaxCalculateAxisAlignedRect, collisionState.localNormal, collisionState.localHitPoint, collisionState.localDirection);
		}
		if (DEBUG_DRAW)
		{
			DrawAxisAlignedRect(minMaxCalculateAxisAlignedRect[0], minMaxCalculateAxisAlignedRect[1], collisionState.scanAxis, chunk.transform, minMaxCalculateAxisAlignedRect[0][collisionState.scanAxis]);
		}
		if (LayerScan(radius, minMaxCalculateAxisAlignedRect, ref vh, num2, ref collisionState))
		{
			float magnitude = elipsoidSpaceToWorld.MultiplyVector(collisionState.elipsoidSpaceDirection * vh.distance).magnitude;
			vh.distance = magnitude;
			vh.distance += num;
			SetFoundHitVariables(ref vh, collisionData, wo.Id, wo.InteractionFlags);
			return true;
		}
		return false;
	}

	private static bool LayerScan(Vector3 radius, Vector3[] alignedRect, ref VoxelHit vh, float distance, ref CollisionState collisionState)
	{
		vh.distance = float.PositiveInfinity;
		Vector3 vector = alignedRect[0];
		Vector3 vector2 = alignedRect[0];
		Vector3 vector3 = alignedRect[0];
		bool flag = false;
		for (int i = 0; i < 3; i++)
		{
			if (i != collisionState.scanAxis && !flag)
			{
				vector2[i] = alignedRect[1][i];
				flag = true;
			}
			else if (i != collisionState.scanAxis && flag)
			{
				vector3[i] = alignedRect[1][i];
			}
		}
		Vector3 normalized = (vector2 - vector).normalized;
		Vector3 normalized2 = (vector3 - vector).normalized;
		IntVector intVector = CubeMathFunctions.LocalPosToLocalIntVector(normalized);
		IntVector intVector2 = CubeMathFunctions.LocalPosToLocalIntVector(normalized2);
		cellTraverser.Init(vector, collisionState);
		IntVector intVector3 = CubeMathFunctions.LocalPosToLocalIntVector(vector2);
		IntVector intVector4 = CubeMathFunctions.LocalPosToLocalIntVector(vector3);
		intVector3[collisionState.scanAxis] = (short)Mathf.Clamp(intVector3[collisionState.scanAxis], collisionState.minBounds[collisionState.scanAxis], collisionState.maxBounds[collisionState.scanAxis]);
		intVector4[collisionState.scanAxis] = (short)Mathf.Clamp(intVector3[collisionState.scanAxis], collisionState.minBounds[collisionState.scanAxis], collisionState.maxBounds[collisionState.scanAxis]);
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
				Debug.Log("scanAxis" + collisionState.scanAxis);
				Debug.Log("testVector " + intVector5);
				Debug.Log("testVector[scanAxis] " + intVector5[collisionState.scanAxis]);
				Debug.Log("min[scanAxis] " + collisionState.minBounds[collisionState.scanAxis]);
				Debug.Log("max[scanAxis] " + collisionState.maxBounds[collisionState.scanAxis]);
				cellTraverser.DebugAll();
				Debug.LogError("While loop did not exit");
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
						Debug.Log("scanAxis " + collisionState.scanAxis);
						Debug.Log("testVector " + intVector5);
						Debug.Log("testVector[scanAxis] " + intVector5[collisionState.scanAxis]);
						Debug.Log("min[scanAxis] " + collisionState.minBounds[collisionState.scanAxis]);
						Debug.Log("max[scanAxis] " + collisionState.maxBounds[collisionState.scanAxis]);
						cellTraverser.DebugAll();
						Debug.LogError("While loop did not exit");
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
		if (DEBUG_MODE)
		{
			IntVector item = new IntVector(pos.x, pos.y, pos.z);
			if (debugTestedIntVector.Contains(item))
			{
				Debug.LogWarning("testVector allReady tested " + pos);
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
				Vector3 vector = new Vector3(pos.x, pos.y, pos.z);
				for (int i = 0; i < cachedCornersElipsoidSpace.Length; i++)
				{
					cachedCornersElipsoidSpace[i] += vector;
					ref Vector3 reference = ref cachedCornersElipsoidSpace[i];
					reference = collisionState.localToElipsoidSpace.MultiplyPoint(cachedCornersElipsoidSpace[i]);
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
		vhCached.distance = float.PositiveInfinity;
		bool flag = TriangleCheck.CheckTriangle(p1, p2, p3, collisionState.elipsoidSpaceOrigin, collisionState.elipsoidSpaceDirection, collisionState.elipsoidSpaceDistance, ref vhCached);
		if (flag && vhCached.distance > collisionState.elipsoidSpaceDistance)
		{
			Debug.Log("this is wrong");
			Debug.LogFormat("Points: {0}, {1}, {2}", p1, p2, p3);
			Debug.LogFormat("Origin, Direction, distance {0}, {1}, {2}.", collisionState.elipsoidSpaceOrigin, collisionState.elipsoidSpaceDirection, distance);
		}
		if (flag && vhCached.distance < currentVoxelHit.distance && (collisionState.elipsoidSpaceOrigin - vhCached.point).sqrMagnitude >= 1f)
		{
			currentVoxelHit.distance = vhCached.distance;
			currentVoxelHit.point = elipsoidSpaceToWorld.MultiplyPoint(vhCached.point);
			currentVoxelHit.face = face;
			currentVoxelHit.normal = MathFunctions.GetNormal(elipsoidSpaceToWorld.MultiplyPoint(p1), elipsoidSpaceToWorld.MultiplyPoint(p2), elipsoidSpaceToWorld.MultiplyPoint(p3));
			currentVoxelHit.cube = cube;
			currentVoxelHit.cubePos = new IntVector(pos.x, pos.y, pos.z);
			return true;
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
		Plane plane = new Plane(dir, startOrigin);
		float num = 0f;
		Ray ray = new Ray(Vector3.zero, -dir);
		foreach (Vector3 vector in pointsOnProjectPlane)
		{
			if (plane.GetSide(vector))
			{
				ray.origin = vector;
				float enter = 0f;
				if (plane.Raycast(ray, out enter) && enter > num)
				{
					num = enter;
				}
			}
		}
		if (num > 0f)
		{
			Vector3 vector2 = -dir * num;
			for (int j = 0; j < pointsOnProjectPlane.Length; j++)
			{
				pointsOnProjectPlane[j] += vector2;
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
		collisionPlane0.SetNormalAndPosition(-localNormal, localHit);
		float num = 0f;
		Ray ray = new Ray(Vector3.zero, -localDir);
		foreach (Vector3 origin in axisAlignedRect)
		{
			ray.origin = origin;
			if (collisionPlane0.Raycast(ray, out var enter) && enter > num)
			{
				num = enter;
			}
		}
		if (num != 0f)
		{
			localDir *= 0f - num;
			for (int j = 0; j < axisAlignedRect.Length; j++)
			{
				axisAlignedRect[j] += localDir;
			}
		}
	}

	private static bool CubeIsWithinFirstHitRadius(IntVector pos, float moveDistance, ref CollisionState collisionState)
	{
		if (!collisionState.firstHitDetected)
		{
			return true;
		}
		float num = 1.414214f;
		float num2 = num + collisionState.scaledMaxRadius;
		Vector3 vector = new Vector3(pos.x, pos.y, pos.z);
		moveDistance *= collisionState.scaledMaxRadius;
		Vector3 vector2 = collisionState.localOrigin + collisionState.localDirection * moveDistance;
		if ((vector - vector2).sqrMagnitude < num2 * num2)
		{
			return true;
		}
		return false;
	}

	private static bool CubeIsWithinSphereRadius(IntVector pos, ref CollisionState collisionState)
	{
		float num = 1.414214f;
		float num2 = num + collisionState.scaledMaxRadius;
		Vector3 point = new Vector3(pos.x, pos.y, pos.z);
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
		if (collPlane.Raycast(ray, out var enter))
		{
			hit = ray.origin + ray.direction.normalized * enter;
			return true;
		}
		return false;
	}

	private static void CalculateAxisAlignedRect(Vector3[] hitsClockwise, Transform t)
	{
		for (int i = 0; i < 4; i++)
		{
		}
		for (int j = 0; j < 4; j++)
		{
			Vector3 vector = t.InverseTransformPoint(hitsClockwise[j]);
			cornerPointsLocalSpaceCalculateAxisAlignedRect[j] = vector;
		}
		Vector3 vector3;
		Vector3 vector2 = (vector3 = cornerPointsLocalSpaceCalculateAxisAlignedRect[0]);
		for (int k = 1; k < 4; k++)
		{
			for (int l = 0; l < 3; l++)
			{
				if (cornerPointsLocalSpaceCalculateAxisAlignedRect[k][l] > vector3[l])
				{
					vector3[l] = cornerPointsLocalSpaceCalculateAxisAlignedRect[k][l];
				}
				if (cornerPointsLocalSpaceCalculateAxisAlignedRect[k][l] < vector2[l])
				{
					vector2[l] = cornerPointsLocalSpaceCalculateAxisAlignedRect[k][l];
				}
			}
		}
		minMaxCalculateAxisAlignedRect[0] = vector2;
		minMaxCalculateAxisAlignedRect[1] = vector3;
	}

	private static Vector3 GetMaxAngleLocalAxisVector(Ray ray, ICubeModelCollider cmb)
	{
		Vector3 result = Vector3.zero;
		Vector3 zero = Vector3.zero;
		float num = 1f;
		for (int i = 0; i < 3; i++)
		{
			zero = cmb.WorldRotation * testVectorGetMaxAngleLocalAxisVector[i];
			float num2 = Mathf.Abs(Vector3.Dot(ray.direction, zero.normalized));
			if (num2 < num)
			{
				result = zero;
				num = num2;
			}
		}
		return result;
	}

	private static void GetBoundRays(Ray ray, ICubeModelCollider cmb)
	{
		Vector3 maxAngleLocalAxisVector = GetMaxAngleLocalAxisVector(ray, cmb);
		Vector3 lhs = worldToElipsoidSpace.MultiplyVector(ray.direction);
		Vector3 rhs = worldToElipsoidSpace.MultiplyVector(maxAngleLocalAxisVector);
		Vector3 normalized = Vector3.Cross(lhs, rhs).normalized;
		Vector3 normalized2 = Vector3.Cross(lhs, normalized).normalized;
		ref Vector3 reference = ref circleCornersGetBoundRays[0];
		reference = normalized + normalized2;
		ref Vector3 reference2 = ref circleCornersGetBoundRays[1];
		reference2 = normalized - normalized2;
		ref Vector3 reference3 = ref circleCornersGetBoundRays[2];
		reference3 = -normalized + normalized2;
		ref Vector3 reference4 = ref circleCornersGetBoundRays[3];
		reference4 = -normalized - normalized2;
		for (int i = 0; i < circleCornersGetBoundRays.Length; i++)
		{
			raysGetBoundRays[i].origin = elipsoidSpaceToWorld.MultiplyVector(circleCornersGetBoundRays[i]) + ray.origin;
			raysGetBoundRays[i].direction = ray.direction;
		}
	}

	public static void GetRaysProjectedOnPlane(Ray[] boundRays, ref Plane projectPlane, Vector3 planeOrigin, Vector3 planeNormal)
	{
		ref Vector3 reference = ref pointsOnPlane[0];
		reference = boundRays[0].origin;
		ref Vector3 reference2 = ref pointsOnPlane[1];
		reference2 = boundRays[1].origin;
		ref Vector3 reference3 = ref pointsOnPlane[2];
		reference3 = boundRays[2].origin;
		ref Vector3 reference4 = ref pointsOnPlane[3];
		reference4 = boundRays[3].origin;
		bool flag = false;
		for (int i = 0; i < 4; i++)
		{
			if (!RayCast(boundRays[i], ref pointsOnPlane[i], projectPlane))
			{
				Ray ray = new Ray(boundRays[i].origin, -boundRays[i].direction);
				if (RayCast(ray, ref pointsOnPlane[i], projectPlane))
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
		}
		if (flag)
		{
		}
	}

	private static int GetScanAxis(Vector3 scanRectNormal)
	{
		for (int i = 0; i < 3; i++)
		{
			if (Mathf.Abs(scanRectNormal[i]) > 0.9f)
			{
				return i;
			}
		}
		Debug.LogError("No scan axis found " + scanRectNormal);
		return -1;
	}

	private static void DrawAxisAlignedRect(Vector3 min, Vector3 max, int ignoreAxis, Transform t, float localIgnoreAxisValue)
	{
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
