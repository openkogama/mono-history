using System;
using System.Collections;
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

	public static bool MVHit(Ray ray, MVWorldObjectClient wo, out VoxelHit voxelHit, float distance = float.PositiveInfinity)
	{
		voxelHit = default;
		List<RaycastHit> list = new List<RaycastHit>();
		if (wo is ICubeModelCollider)
		{
			ChunkInstances chunkInstances = ((ICubeModelCollider)wo).ChunkInstances;
			List<Collider> list2 = new List<Collider>();
			foreach (KeyValuePair<IntVector, GameObject> item in (IEnumerable)chunkInstances)
			{
				if (item.Value.transform.GetComponent<Collider>().Raycast(ray, out var hitInfo, distance))
				{
					list.Add(hitInfo);
				}
				else if (item.Value.transform.GetComponent<Collider>().bounds.Contains(ray.origin))
				{
					list2.Add(item.Value.transform.GetComponent<Collider>());
				}
			}
			PhysicsCollisionDatasWrapper physicsCollisionData = SharedCollisionFunctions.GetPhysicsCollisionData(list2.ToArray(), list.ToArray(), ray.origin);
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
			if (wo.GameObject.transform.GetComponent<Collider>() == null)
			{
				Debug.LogWarning("No collider on wo of type " + wo.GetType());
				Debug.LogWarning("Maybe a recursive check of the children is needed");
				return false;
			}
			Debug.LogWarning("Remember to test positive infinity!");
			if (wo.GameObject.transform.GetComponent<Collider>().Raycast(ray, out var hitInfo2, distance))
			{
				PhysicsCollisionData physicsCollisionData2 = new PhysicsCollisionData();
				physicsCollisionData2.Set(hitInfo2);
				SharedCollisionFunctions.SetToNoneVoxelHit(ref voxelHit, physicsCollisionData2, wo.Id);
				return true;
			}
		}
		return false;
	}

	public static List<VoxelHit> MVHitAll(Ray ray, float distance = float.PositiveInfinity, int layerMask = -5, HashSet<int> ignoreWoIds = null)
	{
		return MVHit(ray, all: true, distance, layerMask, ignoreWoIds);
	}

	public static bool MVHit(Ray ray, out VoxelHit voxelHit, float distance = float.PositiveInfinity, int layerMask = -5, HashSet<int> ignoreWoIds = null)
	{
		voxelHit = default;
		List<VoxelHit> list = MVHit(ray, all: false, distance, layerMask, ignoreWoIds);
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

	private static List<VoxelHit> MVHit(Ray ray, bool all, float distance, int layerMask, HashSet<int> ignoreWoIds)
	{
		if (ray.direction.sqrMagnitude == 0f)
		{
			return new List<VoxelHit>();
		}
		if (all)
		{
			foundWos.Clear();
		}
		RaycastHit[] source = Physics.RaycastAll(ray, distance, layerMask);
		Collider[] overlapResult = Physics.OverlapSphere(ray.origin, 0f, layerMask);
		PhysicsCollisionDatasWrapper physicsCollisionData = SharedCollisionFunctions.GetPhysicsCollisionData(overlapResult, source.ToArray(), ray.origin);
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

	private static bool HitDetectOnWo(Ray ray, int i, MVWorldObjectClient wo, PhysicsCollisionDatasWrapper collisionData, bool handleObjectsInsideBoxCollider, out VoxelHit voxelHit, HashSet<int> ignoreWoIds, float distance = float.PositiveInfinity)
	{
		voxelHit = default;
		if (wo is ICubeModelCollider)
		{
			if (GetCellOnRay(ray, ref voxelHit, collisionData[i].transform.gameObject, (ICubeModelCollider)wo, collisionData[i].point, distance, wo.Scale))
			{
				SetFoundHitVariables(ref voxelHit, collisionData[i], (ICubeModelCollider)wo);
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

	private static void HandleObjectsInsideBoxCollider(Ray ray, int indexOfFirstHit, PhysicsCollisionDatasWrapper collisionData, ref VoxelHit voxelHit, HashSet<int> ignoreWoIds, float distance)
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
				if ((ray.origin - voxelHit.point).sqrMagnitude < (ray.origin - collisionData[i].point).sqrMagnitude || !collisionData[indexOfFirstHit].collider.bounds.Contains(collisionData[i].point))
				{
					break;
				}
				VoxelHit vHit = default;
				if (GetCellOnRay(ray, ref vHit, collisionData[i].transform.gameObject, (ICubeModelCollider)mVObject, collisionData[i].point, distance, mVObject.Scale) && (ray.origin - voxelHit.point).sqrMagnitude > (ray.origin - vHit.point).sqrMagnitude)
				{
					SetFoundHitVariables(ref vHit, collisionData[i], (ICubeModelCollider)mVObject);
					SharedCollisionFunctions.SetToVoxelHit(ref voxelHit, ref vHit);
				}
			}
			else if (!collisionData[i].isInsideCollider && (ray.origin - voxelHit.point).sqrMagnitude > (ray.origin - collisionData[i].point).sqrMagnitude)
			{
				SharedCollisionFunctions.SetToNoneVoxelHit(ref voxelHit, collisionData[i], mVObject.Id);
				break;
			}
		}
	}

	private static void SetFoundHitVariables(ref VoxelHit voxelHit, PhysicsCollisionData collisionData, ICubeModelCollider cubeModelBase)
	{
		voxelHit.isCubeHit = true;
		voxelHit.woId = cubeModelBase.Id;
		voxelHit.collider = cubeModelBase.GameObject.GetComponent<Collider>();
		voxelHit.transform = cubeModelBase.Transform;
		voxelHit.interactionFlags = cubeModelBase.InteractionFlags;
	}

	private static bool IsWithinDistance(float distance, Vector3 localOrigin, IntVector voxelPos)
	{
		if (distance == float.PositiveInfinity)
		{
			return true;
		}
		float num = distance * distance + (0.5f * Vector3.one).sqrMagnitude + distance * (0.5f * Vector3.one).sqrMagnitude;
		Vector3 vector = CubeMathFunctions.LocalIntVectorToLocalPos(voxelPos);
		return (vector - localOrigin).sqrMagnitude <= num;
	}

	private static bool GetCellOnRay(Ray ray, ref VoxelHit vHit, GameObject chunk, ICubeModelCollider cmb, Vector3 hitPoint, float distance, Vector3 scale)
	{
		intersectRay.direction = chunk.transform.InverseTransformDirection(ray.direction);
		intersectRay.origin = chunk.transform.InverseTransformPoint(ray.origin);
		float num = distance;
		if (distance != float.PositiveInfinity)
		{
			num = MathFunctions.DivideVector(distance * intersectRay.direction, scale).magnitude;
		}
		float magnitude = MathFunctions.DivideVector((ray.origin - hitPoint).magnitude * intersectRay.direction, scale).magnitude;
		Vector3 vector = intersectRay.origin + intersectRay.direction * magnitude;
		IntVector target = CubeMathFunctions.LocalPosToLocalIntVector(vector);
		IntVector min = default;
		IntVector max = default;
		BoxCollider component = chunk.GetComponent<BoxCollider>();
		Bounds localSpaceBounds = new Bounds(component.center, component.size);
		SharedCollisionFunctions.GetVoxelBounds(ref min, ref max, localSpaceBounds);
		MathFunctions.ClampIntVector(ref target, min, max);
		int num2 = Math.Sign(intersectRay.direction.x);
		int num3 = Math.Sign(intersectRay.direction.y);
		int num4 = Math.Sign(intersectRay.direction.z);
		Vector3 vector2 = new Vector3((int)target.x + ((num2 > 0) ? 1 : 0), (int)target.y + ((num3 > 0) ? 1 : 0), (int)target.z + ((num4 > 0) ? 1 : 0));
		Vector3 vector3 = new Vector3((vector2.x - vector.x - 0.5f) / intersectRay.direction.x, (vector2.y - vector.y - 0.5f) / intersectRay.direction.y, (vector2.z - vector.z - 0.5f) / intersectRay.direction.z);
		if (float.IsNaN(vector3.x) || float.IsNegativeInfinity(vector3.x))
		{
			vector3.x = float.PositiveInfinity;
		}
		if (float.IsNaN(vector3.y) || float.IsNegativeInfinity(vector3.y))
		{
			vector3.y = float.PositiveInfinity;
		}
		if (float.IsNaN(vector3.z) || float.IsNegativeInfinity(vector3.z))
		{
			vector3.z = float.PositiveInfinity;
		}
		Vector3 vector4 = new Vector3((float)num2 / intersectRay.direction.x, (float)num3 / intersectRay.direction.y, (float)num4 / intersectRay.direction.z);
		if (float.IsNaN(vector4.x))
		{
			vector4.x = float.PositiveInfinity;
		}
		if (float.IsNaN(vector4.y))
		{
			vector4.y = float.PositiveInfinity;
		}
		if (float.IsNaN(vector4.z))
		{
			vector4.z = float.PositiveInfinity;
		}
		while (true)
		{
			if (!IsWithinDistance(num, intersectRay.origin, target))
			{
				return false;
			}
			Cube cube = cmb.GetCube(target);
			if (cube != null && GetHitPoint(ray, cube, ref vHit, target, vector, scale, num) && Vector3.Dot(vHit.point - ray.origin, ray.direction) > 0f)
			{
				vHit.normal = chunk.transform.TransformDirection(vHit.normal);
				vHit.cubePos = target;
				vHit.cube = cube;
				return true;
			}
			if (vector3.x < vector3.y && vector3.x < vector3.z)
			{
				target.x += (short)num2;
				if (target.x < min.x || target.x > max.x)
				{
					return false;
				}
				vector3.x += vector4.x;
			}
			else if (vector3.y < vector3.z)
			{
				target.y += (short)num3;
				if (target.y < min.y || target.y > max.y)
				{
					return false;
				}
				vector3.y += vector4.y;
			}
			else
			{
				target.z += (short)num4;
				if (target.z < min.z || target.z > max.z)
				{
					break;
				}
				vector3.z += vector4.z;
			}
		}
		return false;
	}

	private static bool GetHitPoint(Ray ray, Cube cube, ref VoxelHit vHit, IntVector voxelPos, Vector3 localBoundsHitPoint, Vector3 scale, float scaledDistance)
	{
		if (cube.HiddenSides != 63)
		{
			if (cube.UnIndentedSides == 63)
			{
				cubeBounds.center = new Vector3(voxelPos.x, voxelPos.y, voxelPos.z);
				float distance = 0f;
				if (cubeBounds.Contains(intersectRay.origin))
				{
					return false;
				}
				if (cubeBounds.IntersectRay(intersectRay, out distance))
				{
					Vector3 localDir = intersectRay.origin + intersectRay.direction * distance - cubeBounds.center;
					if (distance > scaledDistance)
					{
						return false;
					}
					Face faceIdentityFromLocalDir = Cube.GetFaceIdentityFromLocalDir(localDir);
					FaceFlags faceFlags = CubeBase.FaceToFaceFlag(faceIdentityFromLocalDir);
					if (((uint)cube.HiddenSides & (uint)faceFlags) != 0)
					{
						return false;
					}
					vHit.distance = MathFunctions.MultiplyVector(distance * intersectRay.direction, scale).magnitude;
					vHit.point = ray.origin + ray.direction * vHit.distance;
					vHit.face = faceIdentityFromLocalDir;
					vHit.normal = Cube.GetFaceAxis(vHit.face);
					return true;
				}
			}
			else
			{
				Vector3[] corners = cube.Corners;
				Vector3 vector = new Vector3(voxelPos.x, voxelPos.y, voxelPos.z);
				Vector3 p = default;
				for (int i = 0; i < corners.Length; i++)
				{
					corners[i] += vector;
				}
				foreach (byte value in Enum.GetValues(typeof(FaceFlags)))
				{
					if ((cube.HiddenSides & value) != 0)
					{
						continue;
					}
					Face face = CubeBase.FaceFlagToFace((FaceFlags)value);
					Vector3[] face2 = Cube.GetFace(corners, CubeBase.FaceFlagToFace((FaceFlags)value));
					if (MathFunctions.LineFacetCollision(intersectRay.origin, localBoundsHitPoint + intersectRay.direction * 300f, face2[0], face2[3], face2[2], intersectRay.direction, ref p, ref vHit.normal) || MathFunctions.LineFacetCollision(intersectRay.origin, intersectRay.origin + intersectRay.direction * 300f, face2[2], face2[1], face2[0], intersectRay.direction, ref p, ref vHit.normal))
					{
						float magnitude = (intersectRay.origin - p).magnitude;
						if (magnitude > scaledDistance)
						{
							return false;
						}
						vHit.distance = MathFunctions.MultiplyVector(magnitude * intersectRay.direction, scale).magnitude;
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
