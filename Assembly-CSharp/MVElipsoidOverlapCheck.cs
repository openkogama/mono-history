using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public static class MVElipsoidOverlapCheck
{
	private const float SQRT_3 = 1.7320508f;

	private static Matrix4x4 worldToElipsoidSpace = default;

	private static Matrix4x4 elipsoidSpaceToWorld = default;

	private static Matrix4x4 localToElipsoidSpace = default;

	private static Vector3 localElipsoidPosition = default;

	private static Matrix4x4 worldToRadiusExtendedElipsoidSpace = default;

	private static Matrix4x4 radiusExtendedElipsoidSpaceToWorld = default;

	private static Matrix4x4 localToRadiusExtendedElipsoidSpace = default;

	private static bool reducedElipsoidSpaceExists = false;

	private static Matrix4x4 worldToRadiusReducedElipsoidSpace = default;

	private static Matrix4x4 radiusReducedElipsoidSpaceToWorld = default;

	private static Matrix4x4 localToRadiusReducedElipsoidSpace = default;

	private static Matrix4x4 localToWorld = default;

	private static Matrix4x4 worldToLocal = default;

	private static Vector3[] cachedCorners = new Vector3[8];

	private static Vector3[] cachedFace = new Vector3[4];

	private static ElipsoidOverlapCheckType checkType = ElipsoidOverlapCheckType.Bool;

	private static List<IntVector> cachedIntVectors = new List<IntVector>();

	public static bool ElipsoidOverlapCheckBool(Vector3 radius, Vector3 position, Quaternion rotation, int layerMask = -5, HashSet<int> ignoreWoIds = null)
	{
		checkType = ElipsoidOverlapCheckType.Bool;
		List<MVOverlapResult> list = ElipsoidOverlapCheck(radius, position, rotation, ignoreWoIds, layerMask);
		if (list.Count > 0 && list[0].localCubePos != null && list[0].localCubePos.Length > 0)
		{
			return true;
		}
		return false;
	}

	public static List<MVOverlapResult> ElipsoidOverlapCheckSector(Vector3 radius, Vector3 position, Quaternion rotation, int layerMask = -5, HashSet<int> ignoreWoIds = null)
	{
		checkType = ElipsoidOverlapCheckType.Sectors;
		return ElipsoidOverlapCheck(radius, position, rotation, ignoreWoIds, layerMask);
	}

	public static bool ElipsoidOverlapCheckBool(Vector3 position, Transform transform, Bounds localBounds, int layerMask = -5, HashSet<int> ignoreWoIds = null)
	{
		checkType = ElipsoidOverlapCheckType.Bool;
		List<MVOverlapResult> list = ElipsoidOverlapCheck(position, transform, localBounds, ignoreWoIds, layerMask);
		if (list.Count > 0 && list[0].localCubePos != null && list[0].localCubePos.Length > 0)
		{
			return true;
		}
		return false;
	}

	public static List<MVOverlapResult> ElipsoidOverlapCheckSector(Vector3 position, Transform transform, Bounds localBounds, int layerMask = -5, HashSet<int> ignoreWoIds = null)
	{
		checkType = ElipsoidOverlapCheckType.Sectors;
		return ElipsoidOverlapCheck(position, transform, localBounds, ignoreWoIds, layerMask);
	}

	private static List<MVOverlapResult> ElipsoidOverlapCheck(Vector3 position, Transform transform, Bounds localBounds, HashSet<int> ignoreWoIds, int layerMask = -5)
	{
		Vector3 vector = transform.TransformPoint(localBounds.center);
		Vector3 radius = MathFunctions.MultiplyVector(localBounds.size / 2f, transform.localScale);
		Vector3 vector2 = vector - transform.position;
		position += vector2;
		return ElipsoidOverlapCheck(radius, position, transform.rotation, ignoreWoIds, layerMask);
	}

	private static List<MVOverlapResult> ElipsoidOverlapCheck(Vector3 radius, Vector3 position, Quaternion rotation, HashSet<int> ignoreWoIds, int layerMask = -5)
	{
		List<MVOverlapResult> list = new List<MVOverlapResult>();
		float num = 0f;
		for (int i = 0; i < 3; i++)
		{
			if (radius[i] > num)
			{
				num = radius[i];
			}
		}
		Collider[] array = Physics.OverlapSphere(position, num, layerMask);
		for (int j = 0; j < array.Length; j++)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(array[j].transform);
			if (!SharedCollisionFunctions.IgnoreCollision(mVObject, ignoreWoIds) && ElipsoidOverlapCheckOnWo(radius, position, rotation, array[j].transform.gameObject, mVObject, out var elipsoidOverlapResult))
			{
				elipsoidOverlapResult.woId = mVObject.Id;
				list.Add(elipsoidOverlapResult);
				if (checkType == ElipsoidOverlapCheckType.Bool)
				{
					return list;
				}
			}
		}
		return list;
	}

	private static bool ElipsoidOverlapCheckOnWo(Vector3 radius, Vector3 position, Quaternion rotation, GameObject chunk, MVWorldObjectClient wo, out MVOverlapResult elipsoidOverlapResult)
	{
		elipsoidOverlapResult = default;
		if (wo is ICubeModelCollider)
		{
			ICubeModelCollider cmb = (ICubeModelCollider)wo;
			localToWorld = chunk.transform.localToWorldMatrix;
			worldToLocal = chunk.transform.worldToLocalMatrix;
			elipsoidSpaceToWorld = Matrix4x4.TRS(position, rotation, radius);
			worldToElipsoidSpace = elipsoidSpaceToWorld.inverse;
			localToElipsoidSpace = worldToElipsoidSpace * localToWorld;
			localElipsoidPosition = worldToLocal.MultiplyPoint(position);
			Vector3 vector = MathFunctions.MultiplyVector(Vector3.one * 0.8660254f, wo.Scale);
			radiusExtendedElipsoidSpaceToWorld = Matrix4x4.TRS(position, rotation, radius + vector);
			worldToRadiusExtendedElipsoidSpace = radiusExtendedElipsoidSpaceToWorld.inverse;
			localToRadiusExtendedElipsoidSpace = worldToRadiusExtendedElipsoidSpace * localToWorld;
			Vector3 vector2 = radius - vector;
			if (vector2.x > 0f && vector2.y > 0f && vector2.z > 0f)
			{
				radiusReducedElipsoidSpaceToWorld = Matrix4x4.TRS(position, rotation, radius - vector);
				worldToRadiusReducedElipsoidSpace = radiusReducedElipsoidSpaceToWorld.inverse;
				localToRadiusReducedElipsoidSpace = worldToRadiusReducedElipsoidSpace * localToWorld;
				reducedElipsoidSpaceExists = true;
			}
			else
			{
				reducedElipsoidSpaceExists = false;
			}
			Vector3[] tangentNormalsLocalSpace = GetTangentNormalsLocalSpace();
			Bounds boundsFromAxisAlignedVectors = GetBoundsFromAxisAlignedVectors(tangentNormalsLocalSpace);
			boundsFromAxisAlignedVectors.center = worldToLocal.MultiplyPoint(position);
			return ScanElipsoidBounds(boundsFromAxisAlignedVectors, chunk, cmb, ref elipsoidOverlapResult);
		}
		return false;
	}

	private static bool ScanElipsoidBounds(Bounds localElipsoidBounds, GameObject chunk, ICubeModelCollider cmb, ref MVOverlapResult elipsoidOverlapResult)
	{
		cachedIntVectors.Clear();
		IntVector target = CubeMathFunctions.LocalPosToLocalIntVector(localElipsoidBounds.min);
		IntVector target2 = CubeMathFunctions.LocalPosToLocalIntVector(localElipsoidBounds.max);
		IntVector min = default;
		IntVector max = default;
		SharedCollisionFunctions.GetVoxelBounds(ref min, ref max, chunk.GetComponent<MeshFilter>().sharedMesh.bounds);
		MathFunctions.ClampIntVector(ref target, min, max);
		MathFunctions.ClampIntVector(ref target2, min, max);
		bool flag = false;
		for (int i = target.x; i <= target2.x; i++)
		{
			for (int j = target.y; j <= target2.y; j++)
			{
				for (int k = target.z; k <= target2.z; k++)
				{
					IntVector intVector = new IntVector((short)i, (short)j, (short)k);
					if (HandleCube(intVector, cmb, ref elipsoidOverlapResult))
					{
						flag = true;
						cachedIntVectors.Add(intVector);
						if (checkType == ElipsoidOverlapCheckType.Bool)
						{
							elipsoidOverlapResult.localCubePos = cachedIntVectors.ToArray();
							return flag;
						}
					}
				}
			}
		}
		if (flag)
		{
			elipsoidOverlapResult.localCubePos = cachedIntVectors.ToArray();
		}
		return flag;
	}

	private static bool HandleCube(IntVector cubePos, ICubeModelCollider cmb, ref MVOverlapResult elipsoidOverlapResult)
	{
		Cube cube = cmb.GetCube(cubePos);
		Vector3 vector = new Vector3(cubePos.x, cubePos.y, cubePos.z);
		if (cube != null && localToRadiusExtendedElipsoidSpace.MultiplyPoint(vector).sqrMagnitude <= 1f)
		{
			if (reducedElipsoidSpaceExists)
			{
				if (localToRadiusReducedElipsoidSpace.MultiplyPoint(vector).sqrMagnitude <= 1f)
				{
					return true;
				}
				return DoDetailedCheck(cube, vector);
			}
			if (IsCenterPointWithinCube(cube, vector))
			{
				return true;
			}
			return DoDetailedCheck(cube, vector);
		}
		return false;
	}

	private static bool IsCenterPointWithinCube(Cube cube, Vector3 localPos)
	{
		int num = 0;
		CubeBase.GetCorners(cube, ref cachedCorners);
		for (int i = 0; i < cachedCorners.Length; i++)
		{
			cachedCorners[i] += localPos;
		}
		FaceFlags[] faceFlagsArray = CubeBase.FaceFlagsArray;
		foreach (FaceFlags faceFlag in faceFlagsArray)
		{
			CubeBase.GetFace(ref cachedCorners, ref cachedFace, CubeBase.FaceFlagToFace(faceFlag));
			Vector3 p = default;
			if (MathFunctions.LineFacet(localElipsoidPosition, localElipsoidPosition + Vector3.right * 4f, cachedFace[0], cachedFace[3], cachedFace[2], ref p))
			{
				num++;
			}
			if (MathFunctions.LineFacet(localElipsoidPosition, localElipsoidPosition + Vector3.right * 4f, cachedFace[2], cachedFace[1], cachedFace[0], ref p))
			{
				num++;
			}
		}
		if (num % 2 == 0)
		{
			return false;
		}
		return true;
	}

	private static bool DoDetailedCheck(Cube cube, Vector3 localPos)
	{
		CubeBase.GetCorners(cube, ref cachedCorners);
		for (int i = 0; i < cachedCorners.Length; i++)
		{
			cachedCorners[i] += localPos;
			ref Vector3 reference = ref cachedCorners[i];
			reference = localToElipsoidSpace.MultiplyPoint(cachedCorners[i]);
			if (cachedCorners[i].sqrMagnitude <= 1f)
			{
				return true;
			}
		}
		FaceFlags[] faceFlagsArray = CubeBase.FaceFlagsArray;
		foreach (FaceFlags faceFlag in faceFlagsArray)
		{
			CubeBase.GetFace(ref cachedCorners, ref cachedFace, CubeBase.FaceFlagToFace(faceFlag));
			if (!HandleTriangleTest(cachedFace[0], cachedFace[3], cachedFace[2], Vector3.zero, 1f))
			{
				return true;
			}
			if (!HandleTriangleTest(cachedFace[2], cachedFace[1], cachedFace[0], Vector3.zero, 1f))
			{
				return true;
			}
		}
		return false;
	}

	private static bool HandleTriangleTest(Vector3 A, Vector3 B, Vector3 C, Vector3 P, float r)
	{
		if ((A - B).sqrMagnitude < 0.01f || (A - C).sqrMagnitude < 0.01f || (B - C).sqrMagnitude < 0.01f)
		{
			return true;
		}
		A -= P;
		B -= P;
		C -= P;
		float num = r * r;
		Vector3 normalized = Vector3.Cross(B - A, C - A).normalized;
		float num2 = Vector3.Dot(A, normalized);
		float num3 = Vector3.Dot(normalized, normalized);
		bool flag = num2 * num2 > num * num3;
		float num4 = Vector3.Dot(A, A);
		float num5 = Vector3.Dot(A, B);
		float num6 = Vector3.Dot(A, C);
		float num7 = Vector3.Dot(B, B);
		float num8 = Vector3.Dot(B, C);
		float num9 = Vector3.Dot(C, C);
		bool flag2 = (num4 > num) & (num5 > num4) & (num6 > num4);
		bool flag3 = (num7 > num) & (num5 > num7) & (num8 > num7);
		bool flag4 = (num9 > num) & (num6 > num9) & (num8 > num9);
		Vector3 vector = B - A;
		Vector3 vector2 = C - B;
		Vector3 vector3 = A - C;
		float num10 = num5 - num4;
		float num11 = num8 - num7;
		float num12 = num6 - num9;
		float num13 = Vector3.Dot(vector, vector);
		float num14 = Vector3.Dot(vector2, vector2);
		float num15 = Vector3.Dot(vector3, vector3);
		Vector3 vector4 = A * num13 - num10 * vector;
		Vector3 vector5 = B * num14 - num11 * vector2;
		Vector3 vector6 = C * num15 - num12 * vector3;
		Vector3 rhs = C * num13 - vector4;
		Vector3 rhs2 = A * num14 - vector5;
		Vector3 rhs3 = B * num15 - vector6;
		bool flag5 = (Vector3.Dot(vector4, vector4) > num * num13 * num13) & (Vector3.Dot(vector4, rhs) > 0f);
		bool flag6 = (Vector3.Dot(vector5, vector5) > num * num14 * num14) & (Vector3.Dot(vector5, rhs2) > 0f);
		bool flag7 = (Vector3.Dot(vector6, vector6) > num * num15 * num15) & (Vector3.Dot(vector6, rhs3) > 0f);
		return flag || flag2 || flag3 || flag4 || flag5 || flag6 || flag7;
	}

	private static Vector3[] GetTangentNormalsLocalSpace()
	{
		Vector3 vector = localToWorld.MultiplyVector(Vector3.right);
		Vector3 tangent = localToWorld.MultiplyVector(Vector3.up);
		Vector3 tangent2 = localToWorld.MultiplyVector(Vector3.forward);
		return new Vector3[3]
		{
			GetTangentNormal(vector, tangent2),
			GetTangentNormal(tangent, tangent2),
			GetTangentNormal(tangent, vector)
		};
	}

	private static Bounds GetBoundsFromAxisAlignedVectors(Vector3[] vectors)
	{
		for (int i = 0; i < vectors.Length; i++)
		{
			ref Vector3 reference = ref vectors[i];
			reference = worldToLocal.MultiplyVector(vectors[i]);
		}
		return GetBoundsFromVectors(vectors);
	}

	private static Bounds GetBoundsFromVectors(Vector3[] vectors)
	{
		Bounds result = default;
		Vector3 vector = default;
		for (int i = 0; i < vectors.Length; i++)
		{
			Vector3 vector2 = vectors[i];
			for (int j = 0; j < 3; j++)
			{
				if (result.extents[j] < Mathf.Abs(vector2[j]))
				{
					vector = result.extents;
					vector[j] = Mathf.Abs(vector2[j]);
					result.extents = vector;
				}
			}
		}
		return result;
	}

	private static Vector3 GetTangentNormal(Vector3 tangent0, Vector3 tangent1)
	{
		Vector3 normalized = worldToElipsoidSpace.MultiplyVector(tangent0).normalized;
		Vector3 normalized2 = worldToElipsoidSpace.MultiplyVector(tangent1).normalized;
		Vector3 normalized3 = Vector3.Cross(normalized, normalized2).normalized;
		return elipsoidSpaceToWorld.MultiplyVector(normalized3);
	}
}
