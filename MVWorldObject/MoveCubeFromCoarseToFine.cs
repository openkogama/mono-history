using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public static class MoveCubeFromCoarseToFine
{
	private static IntVector[] intCubeCorners = new IntVector[8]
	{
		new IntVector(0, 0, 0),
		new IntVector(1, 0, 0),
		new IntVector(1, 0, 1),
		new IntVector(0, 0, 1),
		new IntVector(0, 1, 0),
		new IntVector(1, 1, 0),
		new IntVector(1, 1, 1),
		new IntVector(0, 1, 1)
	};

	public static void MoveCube(ICubeModel from, ICubeModel to, IntVector fromPos)
	{
		CubeBase cubeBase = from.GetCubeBase(fromPos);
		if (cubeBase.UnIndentedSides == 63)
		{
			AddUnindentedCube(from, to, fromPos);
		}
		else
		{
			AddIndentedCube(from, to, fromPos);
		}
	}

	private static void AddUnindentedCube(ICubeModel from, ICubeModel to, IntVector fromPos)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		byte[] array = (byte[])from.GetCubeBase(fromPos).FaceMaterials.Clone();
		Vector3 scale = from.Scale;
		float num = scale[0];
		Vector3 scale2 = to.Scale;
		int num2 = (int)(num / scale2[0]);
		IntVector intVector = fromPos * num2;
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				for (int k = 0; k < num2; k++)
				{
					to.AddCubeNetworkUpdate(new IntVector((short)(intVector.x + i), (short)(intVector.y + j), (short)(intVector.z + k)), new CubeBase(CubeBase.IdentityByteCorners, (byte[])array.Clone()));
				}
			}
		}
	}

	private static void AddIndentedCube(ICubeModel from, ICubeModel to, IntVector fromPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Vector3 scale = from.Scale;
		float num = scale[0];
		Vector3 scale2 = to.Scale;
		int num2 = (int)(num / scale2[0]);
		List<ValidPos> validPoses = CreateValidPosGrid(num2);
		List<Plane> testPlanes = GetTestPlanes(from, fromPos);
		SetValidPoints(testPlanes, validPoses);
		List<IntVector> validCubes = GetValidCubes(validPoses, num2);
		IntVector intVector = fromPos * num2;
		byte[] array = (byte[])from.GetCubeBase(fromPos).FaceMaterials.Clone();
		foreach (IntVector item in validCubes)
		{
			to.AddCubeNetworkUpdate(intVector + item, new CubeBase(CubeBase.IdentityByteCorners, (byte[])array.Clone()));
		}
	}

	private static List<Plane> GetTestPlanes(ICubeModel from, IntVector fromPos)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		CubeBase cubeBase = from.GetCubeBase(fromPos);
		Vector3[] corners = cubeBase.Corners;
		Vector3[] faceVertices = new Vector3[4];
		List<Plane> list = new List<Plane>();
		FaceFlags[] faceFlagsArray = CubeBase.FaceFlagsArray;
		foreach (FaceFlags faceFlags in faceFlagsArray)
		{
			if (((uint)cubeBase.UnIndentedSides & (uint)faceFlags) != 0)
			{
				continue;
			}
			Face face = CubeBase.FaceFlagToFace(faceFlags);
			CubeBase.GetFace(ref corners, ref faceVertices, face);
			if (!IsFaceIndented(face, ref faceVertices))
			{
				continue;
			}
			Plane item = new Plane(faceVertices[0], faceVertices[3], faceVertices[2]);
			Plane item2 = new Plane(faceVertices[2], faceVertices[1], faceVertices[0]);
			Vector3 val = item.normal - item2.normal;
			if (val.sqrMagnitude == 0f)
			{
				Vector3 normal = item.normal;
				if (normal.sqrMagnitude != 0f)
				{
					list.Add(item);
				}
				continue;
			}
			Vector3 normal2 = item.normal;
			if (normal2.sqrMagnitude != 0f)
			{
				list.Add(item);
			}
			Vector3 normal3 = item2.normal;
			if (normal3.sqrMagnitude != 0f)
			{
				list.Add(item2);
			}
		}
		return list;
	}

	private static List<ValidPos> CreateValidPosGrid(int scaleFactor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.one / (float)scaleFactor;
		Vector3 val2 = -Vector3.one * 0.5f;
		int num = scaleFactor + 1;
		List<ValidPos> list = new List<ValidPos>();
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				for (int k = 0; k < num; k++)
				{
					Vector3 pos = val2 + new IntVector((short)i, (short)j, (short)k) * val;
					list.Add(new ValidPos(valid: true, pos));
				}
			}
		}
		return list;
	}

	private static List<IntVector> GetValidCubes(List<ValidPos> validPoses, int scaleFactor)
	{
		List<IntVector> list = new List<IntVector>();
		for (int i = 0; i < scaleFactor; i++)
		{
			for (int j = 0; j < scaleFactor; j++)
			{
				for (int k = 0; k < scaleFactor; k++)
				{
					IntVector intVector = new IntVector(i, j, k);
					bool flag = true;
					IntVector[] array = intCubeCorners;
					foreach (IntVector intVector2 in array)
					{
						int index = IntVectorToValidPosIndex(intVector + intVector2, scaleFactor + 1);
						flag = validPoses[index].valid;
						if (!flag)
						{
							break;
						}
					}
					if (flag)
					{
						list.Add(intVector);
					}
				}
			}
		}
		return list;
	}

	private static int IntVectorToValidPosIndex(IntVector intVector, int validPosSize)
	{
		return intVector.x * validPosSize * validPosSize + intVector.y * validPosSize + intVector.z;
	}

	private static void SetValidPoints(List<Plane> testPlanes, List<ValidPos> validPoses)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		foreach (Plane testPlane in testPlanes)
		{
			Plane current = testPlane;
			foreach (ValidPos validPose in validPoses)
			{
				if (validPose.valid && current.GetDistanceToPoint(validPose.pos) >= 0.0001f)
				{
					validPose.valid = false;
				}
			}
		}
	}

	private static bool IsFaceIndented(Face face, ref Vector3[] faceCorners)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		float num2 = 0.5f;
		switch (face)
		{
		case Face.Top:
			num = 1;
			break;
		case Face.Bottom:
			num = 1;
			num2 = 0f - num2;
			break;
		case Face.Left:
			num = 0;
			num2 = 0f - num2;
			break;
		case Face.Right:
			num = 0;
			break;
		case Face.Front:
			num = 2;
			num2 = 0f - num2;
			break;
		case Face.Back:
			num = 2;
			break;
		}
		Vector3[] array = faceCorners;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 val = array[i];
			if (!Mathf.Approximately(val[num], num2))
			{
				return true;
			}
		}
		return false;
	}
}
