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

	static MVElipsoidOverlapCheck()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
	}

	public static bool ElipsoidOverlapCheckBool(Vector3 radius, Vector3 position, Quaternion rotation, int layerMask = -5, HashSet<int> ignoreWoIds = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		checkType = ElipsoidOverlapCheckType.Sectors;
		return ElipsoidOverlapCheck(radius, position, rotation, ignoreWoIds, layerMask);
	}

	public static bool ElipsoidOverlapCheckBool(Vector3 position, Transform transform, Bounds localBounds, int layerMask = -5, HashSet<int> ignoreWoIds = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		checkType = ElipsoidOverlapCheckType.Sectors;
		return ElipsoidOverlapCheck(position, transform, localBounds, ignoreWoIds, layerMask);
	}

	private static List<MVOverlapResult> ElipsoidOverlapCheck(Vector3 position, Transform transform, Bounds localBounds, HashSet<int> ignoreWoIds, int layerMask = -5)
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
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = transform.TransformPoint(localBounds.center);
		Vector3 radius = MathFunctions.MultiplyVector(localBounds.size / 2f, transform.localScale);
		Vector3 val2 = val - transform.position;
		position += val2;
		return ElipsoidOverlapCheck(radius, position, transform.rotation, ignoreWoIds, layerMask);
	}

	private static List<MVOverlapResult> ElipsoidOverlapCheck(Vector3 radius, Vector3 position, Quaternion rotation, HashSet<int> ignoreWoIds, int layerMask = -5)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
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
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(((Component)array[j]).transform);
			if (!SharedCollisionFunctions.IgnoreCollision(mVObject, ignoreWoIds) && ElipsoidOverlapCheckOnWo(radius, position, rotation, ((Component)((Component)array[j]).transform).gameObject, mVObject, out var elipsoidOverlapResult))
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
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		elipsoidOverlapResult = default;
		if (wo is MVCubeModelBase)
		{
			MVCubeModelBase cmb = (MVCubeModelBase)wo;
			localToWorld = chunk.transform.localToWorldMatrix;
			worldToLocal = chunk.transform.worldToLocalMatrix;
			elipsoidSpaceToWorld = Matrix4x4.TRS(position, rotation, radius);
			worldToElipsoidSpace = elipsoidSpaceToWorld.inverse;
			localToElipsoidSpace = worldToElipsoidSpace * localToWorld;
			localElipsoidPosition = worldToLocal.MultiplyPoint(position);
			Vector3 val = MathFunctions.MultiplyVector(Vector3.one * 0.8660254f, wo.Scale);
			radiusExtendedElipsoidSpaceToWorld = Matrix4x4.TRS(position, rotation, radius + val);
			worldToRadiusExtendedElipsoidSpace = radiusExtendedElipsoidSpaceToWorld.inverse;
			localToRadiusExtendedElipsoidSpace = worldToRadiusExtendedElipsoidSpace * localToWorld;
			Vector3 val2 = radius - val;
			if (val2.x > 0f && val2.y > 0f && val2.z > 0f)
			{
				radiusReducedElipsoidSpaceToWorld = Matrix4x4.TRS(position, rotation, radius - val);
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

	private static bool ScanElipsoidBounds(Bounds localElipsoidBounds, GameObject chunk, MVCubeModelBase cmb, ref MVOverlapResult elipsoidOverlapResult)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
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

	private static bool HandleCube(IntVector cubePos, MVCubeModelBase cmb, ref MVOverlapResult elipsoidOverlapResult)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		Cube cube = cmb.GetCube(cubePos);
		Vector3 val = new Vector3((float)cubePos.x, (float)cubePos.y, (float)cubePos.z);
		if (cube != null)
		{
			Vector3 val2 = localToRadiusExtendedElipsoidSpace.MultiplyPoint(val);
			if (val2.sqrMagnitude <= 1f)
			{
				if (reducedElipsoidSpaceExists)
				{
					Vector3 val3 = localToRadiusReducedElipsoidSpace.MultiplyPoint(val);
					if (val3.sqrMagnitude <= 1f)
					{
						return true;
					}
					return DoDetailedCheck(cube, val);
				}
				if (IsCenterPointWithinCube(cube, val))
				{
					return true;
				}
				return DoDetailedCheck(cube, val);
			}
		}
		return false;
	}

	private static bool IsCenterPointWithinCube(Cube cube, Vector3 localPos)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		CubeBase.GetCorners(cube, ref cachedCorners);
		for (int i = 0; i < cachedCorners.Length; i++)
		{
			ref Vector3 reference = ref cachedCorners[i];
			reference += localPos;
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
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		CubeBase.GetCorners(cube, ref cachedCorners);
		for (int i = 0; i < cachedCorners.Length; i++)
		{
			ref Vector3 reference = ref cachedCorners[i];
			reference += localPos;
			ref Vector3 reference2 = ref cachedCorners[i];
			reference2 = localToElipsoidSpace.MultiplyPoint(cachedCorners[i]);
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = A - B;
		if (!(val.sqrMagnitude < 0.01f))
		{
			Vector3 val2 = A - C;
			if (!(val2.sqrMagnitude < 0.01f))
			{
				Vector3 val3 = B - C;
				if (!(val3.sqrMagnitude < 0.01f))
				{
					A -= P;
					B -= P;
					C -= P;
					float num = r * r;
					Vector3 val4 = Vector3.Cross(B - A, C - A);
					Vector3 normalized = val4.normalized;
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
					Vector3 val5 = B - A;
					Vector3 val6 = C - B;
					Vector3 val7 = A - C;
					float num10 = num5 - num4;
					float num11 = num8 - num7;
					float num12 = num6 - num9;
					float num13 = Vector3.Dot(val5, val5);
					float num14 = Vector3.Dot(val6, val6);
					float num15 = Vector3.Dot(val7, val7);
					Vector3 val8 = A * num13 - num10 * val5;
					Vector3 val9 = B * num14 - num11 * val6;
					Vector3 val10 = C * num15 - num12 * val7;
					Vector3 val11 = C * num13 - val8;
					Vector3 val12 = A * num14 - val9;
					Vector3 val13 = B * num15 - val10;
					bool flag5 = (Vector3.Dot(val8, val8) > num * num13 * num13) & (Vector3.Dot(val8, val11) > 0f);
					bool flag6 = (Vector3.Dot(val9, val9) > num * num14 * num14) & (Vector3.Dot(val9, val12) > 0f);
					bool flag7 = (Vector3.Dot(val10, val10) > num * num15 * num15) & (Vector3.Dot(val10, val13) > 0f);
					return flag || flag2 || flag3 || flag4 || flag5 || flag6 || flag7;
				}
			}
		}
		return true;
	}

	private static Vector3[] GetTangentNormalsLocalSpace()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = localToWorld.MultiplyVector(Vector3.right);
		Vector3 tangent = localToWorld.MultiplyVector(Vector3.up);
		Vector3 tangent2 = localToWorld.MultiplyVector(Vector3.forward);
		return new Vector3[3]
		{
			GetTangentNormal(val, tangent2),
			GetTangentNormal(tangent, tangent2),
			GetTangentNormal(tangent, val)
		};
	}

	private static Bounds GetBoundsFromAxisAlignedVectors(Vector3[] vectors)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < vectors.Length; i++)
		{
			ref Vector3 reference = ref vectors[i];
			reference = worldToLocal.MultiplyVector(vectors[i]);
		}
		return GetBoundsFromVectors(vectors);
	}

	private static Bounds GetBoundsFromVectors(Vector3[] vectors)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Bounds result = default;
		Vector3 extents = default;
		for (int i = 0; i < vectors.Length; i++)
		{
			Vector3 val = vectors[i];
			for (int j = 0; j < 3; j++)
			{
				Vector3 extents2 = result.extents;
				if (extents2[j] < Mathf.Abs(val[j]))
				{
					extents = result.extents;
					extents[j] = Mathf.Abs(val[j]);
					result.extents = extents;
				}
			}
		}
		return result;
	}

	private static Vector3 GetTangentNormal(Vector3 tangent0, Vector3 tangent1)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = worldToElipsoidSpace.MultiplyVector(tangent0);
		Vector3 normalized = val.normalized;
		Vector3 val2 = worldToElipsoidSpace.MultiplyVector(tangent1);
		Vector3 normalized2 = val2.normalized;
		Vector3 val3 = Vector3.Cross(normalized, normalized2);
		Vector3 normalized3 = val3.normalized;
		return elipsoidSpaceToWorld.MultiplyVector(normalized3);
	}
}
