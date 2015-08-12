using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public static class MoveCubeFromCoarseToFine
{
	private class ValidPos
	{
		public bool valid;

		public Vector3 pos;

		public ValidPos(bool valid, Vector3 pos)
		{
			this.valid = valid;
			this.pos = pos;
		}
	}

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
		byte[] array = (byte[])from.GetCubeBase(fromPos).FaceMaterials.Clone();
		int num = (int)(from.Scale[0] / to.Scale[0]);
		IntVector intVector = fromPos * num;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				for (int k = 0; k < num; k++)
				{
					to.AddCubeNetworkUpdate(new IntVector((short)(intVector.x + i), (short)(intVector.y + j), (short)(intVector.z + k)), new CubeBase(CubeBase.IdentityByteCorners, (byte[])array.Clone()));
				}
			}
		}
	}

	private static void AddIndentedCube(ICubeModel from, ICubeModel to, IntVector fromPos)
	{
		int num = (int)(from.Scale[0] / to.Scale[0]);
		List<ValidPos> validPoses = CreateValidPosGrid(num);
		List<Plane> testPlanes = GetTestPlanes(from, fromPos);
		SetValidPoints(testPlanes, validPoses);
		List<IntVector> validCubes = GetValidCubes(validPoses, num);
		IntVector intVector = fromPos * num;
		byte[] array = (byte[])from.GetCubeBase(fromPos).FaceMaterials.Clone();
		foreach (IntVector item in validCubes)
		{
			to.AddCubeNetworkUpdate(intVector + item, new CubeBase(CubeBase.IdentityByteCorners, (byte[])array.Clone()));
		}
	}

	private static List<Plane> GetTestPlanes(ICubeModel from, IntVector fromPos)
	{
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
			if ((item.normal - item2.normal).sqrMagnitude == 0f)
			{
				if (item.normal.sqrMagnitude != 0f)
				{
					list.Add(item);
				}
				continue;
			}
			if (item.normal.sqrMagnitude != 0f)
			{
				list.Add(item);
			}
			if (item2.normal.sqrMagnitude != 0f)
			{
				list.Add(item2);
			}
		}
		return list;
	}

	private static List<ValidPos> CreateValidPosGrid(int scaleFactor)
	{
		Vector3 vector = Vector3.one / scaleFactor;
		Vector3 vector2 = -Vector3.one * 0.5f;
		int num = scaleFactor + 1;
		List<ValidPos> list = new List<ValidPos>();
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				for (int k = 0; k < num; k++)
				{
					Vector3 pos = vector2 + new IntVector((short)i, (short)j, (short)k) * vector;
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
		foreach (Plane testPlane in testPlanes)
		{
			foreach (ValidPos validPose in validPoses)
			{
				if (validPose.valid && testPlane.GetDistanceToPoint(validPose.pos) >= 0.0001f)
				{
					validPose.valid = false;
				}
			}
		}
	}

	private static bool IsFaceIndented(Face face, ref Vector3[] faceCorners)
	{
		int index = 0;
		float num = 0.5f;
		switch (face)
		{
		case Face.Top:
			index = 1;
			break;
		case Face.Bottom:
			index = 1;
			num = 0f - num;
			break;
		case Face.Left:
			index = 0;
			num = 0f - num;
			break;
		case Face.Right:
			index = 0;
			break;
		case Face.Front:
			index = 2;
			num = 0f - num;
			break;
		case Face.Back:
			index = 2;
			break;
		}
		Vector3[] array = faceCorners;
		foreach (Vector3 vector in array)
		{
			if (!Mathf.Approximately(vector[index], num))
			{
				return true;
			}
		}
		return false;
	}
}
