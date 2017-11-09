using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class Cube : CubeBase
{
	private static Vector3[] cornersBookkeeping = new Vector3[8];

	private byte hiddenSides;

	public byte HiddenSides
	{
		get
		{
			return hiddenSides;
		}
		set
		{
			hiddenSides = value;
		}
	}

	public Cube(byte[] byteCorners, byte[] faceMaterials)
		: base(byteCorners, faceMaterials)
	{
	}

	public Cube(BytePacker bp, byte byteFlags)
		: base(bp, byteFlags)
	{
	}

	public Cube Clone()
	{
		return Clone(this);
	}

	public static Cube Clone(Cube original)
	{
		if (original == null)
		{
			return null;
		}
		Cube cube = new Cube((byte[])original.byteCorners.Clone(), (byte[])original.faceMaterials.Clone());
		CubeBase.SetCubeFlags(cube);
		return cube;
	}

	public static byte[] CreateMaterialArray(byte material)
	{
		return new byte[6] { material, material, material, material, material, material };
	}

	public static Vector3[] GetCorners(Cube cube, Face face)
	{
		Vector3[] face2 = GetFace(cube.Corners, face);
		return GetCorners(face2, face).ToArray();
	}

	public static void SetMaterial(Cube cube, Face face, byte materialId)
	{
		cube.faceMaterials[(int)face] = materialId;
	}

	public static Vector3[] GetVertices(Cube cube)
	{
		return GetVertices(cube.Corners);
	}

	public static IntVector GetCubePosAboveFace(IntVector localPos, Face face)
	{
		IntVector result = new IntVector(localPos.x, localPos.y, localPos.z);
		Vector3 faceAxis = GetFaceAxis(face);
		result.x += (short)faceAxis.x;
		result.y += (short)faceAxis.y;
		result.z += (short)faceAxis.z;
		return result;
	}

	public static Face GetFaceIdentityFromLocalDir(Vector3 localDir)
	{
		Vector3 vector = MathFunctions.AbsVector(localDir);
		if (vector.x >= vector.y && vector.x >= vector.z)
		{
			if (localDir.x < 0f)
			{
				return Face.Left;
			}
			return Face.Right;
		}
		if (vector.y >= vector.x && vector.y >= vector.z)
		{
			if (localDir.y < 0f)
			{
				return Face.Bottom;
			}
			return Face.Top;
		}
		if (vector.z >= vector.x && vector.z >= vector.y)
		{
			if (localDir.z < 0f)
			{
				return Face.Front;
			}
			return Face.Back;
		}
		Debug.LogError("no face found");
		return Face.Front;
	}

	public static Vector3 GetFaceAxis(Face face)
	{
		return face switch
		{
			Face.Top => Vector3.up, 
			Face.Bottom => Vector3.down, 
			Face.Left => Vector3.left, 
			Face.Right => Vector3.right, 
			Face.Front => Vector3.back, 
			Face.Back => Vector3.forward, 
			_ => Vector3.zero, 
		};
	}

	public static List<Vector3> GetCorners(List<Vector2> clockwiseCorners, Face direction)
	{
		return SquareCornersToCubeCorners(clockwiseCorners, direction);
	}

	public static void SetFace(Cube cube, Face face, Vector3[] faceVertices)
	{
		cornersBookkeeping = cube.Corners;
		SetFace(ref cornersBookkeeping, face, faceVertices);
		cube.Corners = cornersBookkeeping;
	}

	public static bool IsFaceBoxSideAligened(Cube cube, Face face)
	{
		Vector3[] face2 = GetFace(RotateFaceToTop(cube, face), face);
		float num = 0.5f;
		for (int i = 0; i < 4; i++)
		{
			if (num != face2[i].y)
			{
				return false;
			}
		}
		return true;
	}

	public static void UnIndentFace(Cube cube, Face face)
	{
		Vector3[] face2 = GetFace(RotateFaceToTop(cube, face), face);
		float y = 0.5f;
		for (int i = 0; i < 4; i++)
		{
			face2[i].y = y;
		}
		Quaternion fromTopRotation = GetFromTopRotation(face);
		for (int j = 0; j < face2.Length; j++)
		{
			Vector3 vector = fromTopRotation * face2[j];
			vector = MathFunctions.RoundVector(vector, 3);
			face2[j] = vector;
		}
		SetFace(cube, face, face2);
	}

	public static Vector3[] GetVerticesWorldAxisAligned(Cube cube, IntVector iVector)
	{
		Vector3[] vertices = GetVertices(cube.Corners);
		Vector3 vector = new Vector3(iVector.x, iVector.y, iVector.z);
		for (int i = 0; i < vertices.Length; i++)
		{
			ref Vector3 reference = ref vertices[i];
			reference = vector + vertices[i];
		}
		return vertices;
	}

	private static void GetAverageLightValue(Face face, int vertex, Dictionary<IntVector, Cell> cells, IntVector cubePos, ref Color color, bool inside)
	{
		if (!CubeModelChunk.UseAOShadows)
		{
			color = Color.white;
			return;
		}
		int num = (int)face * 4 + vertex;
		IntVector[] array = ((!inside) ? SharedCubeFunctions.LightTestOffsets[num] : SharedCubeFunctions.LightTestOffsetsInside[num]);
		int num2 = 0;
		for (int i = 0; i < 4; i++)
		{
			num2 = ((!cells.TryGetValue(array[i] + cubePos, out var value)) ? (num2 + 255) : (num2 + value.lightValue));
		}
		color.r = (float)num2 / 1020f;
		color.b = (float)num2 / 1020f;
		color.g = (float)num2 / 1020f;
	}

	public static void GetVisibleFaceVertices(Cube cube, ref CubeModelChunk.FaceData[] faceData, IntVector iVector, Dictionary<IntVector, Cell> cells, ref int index)
	{
		Vector3 vector = new Vector3(iVector.x, iVector.y, iVector.z);
		CubeBase.GetCorners(cube, ref cornersBookkeeping);
		index = 0;
		if ((cube.hiddenSides & 1) == 0)
		{
			faceData[index].face = Face.Top;
			ref Vector3 reference = ref faceData[index].faceVertices[0];
			reference = vector + cornersBookkeeping[0];
			GetAverageLightValue(faceData[index].face, 0, cells, iVector, ref faceData[index].colors[0], cornersBookkeeping[0].y < 0.5f);
			ref Vector3 reference2 = ref faceData[index].faceVertices[1];
			reference2 = vector + cornersBookkeeping[1];
			GetAverageLightValue(faceData[index].face, 1, cells, iVector, ref faceData[index].colors[1], cornersBookkeeping[1].y < 0.5f);
			ref Vector3 reference3 = ref faceData[index].faceVertices[2];
			reference3 = vector + cornersBookkeeping[2];
			GetAverageLightValue(faceData[index].face, 2, cells, iVector, ref faceData[index].colors[2], cornersBookkeeping[2].y < 0.5f);
			ref Vector3 reference4 = ref faceData[index].faceVertices[3];
			reference4 = vector + cornersBookkeeping[3];
			GetAverageLightValue(faceData[index].face, 3, cells, iVector, ref faceData[index].colors[3], cornersBookkeeping[3].y < 0.5f);
			index++;
		}
		if ((cube.hiddenSides & 2) == 0)
		{
			faceData[index].face = Face.Bottom;
			ref Vector3 reference5 = ref faceData[index].faceVertices[0];
			reference5 = vector + cornersBookkeeping[4];
			GetAverageLightValue(faceData[index].face, 0, cells, iVector, ref faceData[index].colors[0], cornersBookkeeping[4].y > -0.5f);
			ref Vector3 reference6 = ref faceData[index].faceVertices[1];
			reference6 = vector + cornersBookkeeping[5];
			GetAverageLightValue(faceData[index].face, 1, cells, iVector, ref faceData[index].colors[1], cornersBookkeeping[5].y > -0.5f);
			ref Vector3 reference7 = ref faceData[index].faceVertices[2];
			reference7 = vector + cornersBookkeeping[6];
			GetAverageLightValue(faceData[index].face, 2, cells, iVector, ref faceData[index].colors[2], cornersBookkeeping[6].y > -0.5f);
			ref Vector3 reference8 = ref faceData[index].faceVertices[3];
			reference8 = vector + cornersBookkeeping[7];
			GetAverageLightValue(faceData[index].face, 3, cells, iVector, ref faceData[index].colors[3], cornersBookkeeping[7].y > -0.5f);
			index++;
		}
		if ((cube.hiddenSides & 4) == 0)
		{
			faceData[index].face = Face.Front;
			ref Vector3 reference9 = ref faceData[index].faceVertices[0];
			reference9 = vector + cornersBookkeeping[7];
			GetAverageLightValue(faceData[index].face, 0, cells, iVector, ref faceData[index].colors[0], cornersBookkeeping[7].z > -0.5f);
			ref Vector3 reference10 = ref faceData[index].faceVertices[1];
			reference10 = vector + cornersBookkeeping[6];
			GetAverageLightValue(faceData[index].face, 1, cells, iVector, ref faceData[index].colors[1], cornersBookkeeping[6].z > -0.5f);
			ref Vector3 reference11 = ref faceData[index].faceVertices[2];
			reference11 = vector + cornersBookkeeping[1];
			GetAverageLightValue(faceData[index].face, 2, cells, iVector, ref faceData[index].colors[2], cornersBookkeeping[1].z > -0.5f);
			ref Vector3 reference12 = ref faceData[index].faceVertices[3];
			reference12 = vector + cornersBookkeeping[0];
			GetAverageLightValue(faceData[index].face, 3, cells, iVector, ref faceData[index].colors[3], cornersBookkeeping[0].z > -0.5f);
			index++;
		}
		if ((cube.hiddenSides & 8) == 0)
		{
			faceData[index].face = Face.Back;
			ref Vector3 reference13 = ref faceData[index].faceVertices[0];
			reference13 = vector + cornersBookkeeping[5];
			GetAverageLightValue(faceData[index].face, 0, cells, iVector, ref faceData[index].colors[0], cornersBookkeeping[5].z < 0.5f);
			ref Vector3 reference14 = ref faceData[index].faceVertices[1];
			reference14 = vector + cornersBookkeeping[4];
			GetAverageLightValue(faceData[index].face, 1, cells, iVector, ref faceData[index].colors[1], cornersBookkeeping[4].z < 0.5f);
			ref Vector3 reference15 = ref faceData[index].faceVertices[2];
			reference15 = vector + cornersBookkeeping[3];
			GetAverageLightValue(faceData[index].face, 2, cells, iVector, ref faceData[index].colors[2], cornersBookkeeping[3].z < 0.5f);
			ref Vector3 reference16 = ref faceData[index].faceVertices[3];
			reference16 = vector + cornersBookkeeping[2];
			GetAverageLightValue(faceData[index].face, 3, cells, iVector, ref faceData[index].colors[3], cornersBookkeeping[2].z < 0.5f);
			index++;
		}
		if ((cube.hiddenSides & 0x10) == 0)
		{
			faceData[index].face = Face.Left;
			ref Vector3 reference17 = ref faceData[index].faceVertices[0];
			reference17 = vector + cornersBookkeeping[4];
			GetAverageLightValue(faceData[index].face, 0, cells, iVector, ref faceData[index].colors[0], cornersBookkeeping[4].x > -0.5f);
			ref Vector3 reference18 = ref faceData[index].faceVertices[1];
			reference18 = vector + cornersBookkeeping[7];
			GetAverageLightValue(faceData[index].face, 1, cells, iVector, ref faceData[index].colors[1], cornersBookkeeping[7].x > -0.5f);
			ref Vector3 reference19 = ref faceData[index].faceVertices[2];
			reference19 = vector + cornersBookkeeping[0];
			GetAverageLightValue(faceData[index].face, 2, cells, iVector, ref faceData[index].colors[2], cornersBookkeeping[0].x > -0.5f);
			ref Vector3 reference20 = ref faceData[index].faceVertices[3];
			reference20 = vector + cornersBookkeeping[3];
			GetAverageLightValue(faceData[index].face, 3, cells, iVector, ref faceData[index].colors[3], cornersBookkeeping[3].x > -0.5f);
			index++;
		}
		if ((cube.hiddenSides & 0x20) == 0)
		{
			faceData[index].face = Face.Right;
			ref Vector3 reference21 = ref faceData[index].faceVertices[0];
			reference21 = vector + cornersBookkeeping[6];
			GetAverageLightValue(faceData[index].face, 0, cells, iVector, ref faceData[index].colors[0], cornersBookkeeping[6].x < 0.5f);
			ref Vector3 reference22 = ref faceData[index].faceVertices[1];
			reference22 = vector + cornersBookkeeping[5];
			GetAverageLightValue(faceData[index].face, 1, cells, iVector, ref faceData[index].colors[1], cornersBookkeeping[5].x < 0.5f);
			ref Vector3 reference23 = ref faceData[index].faceVertices[2];
			reference23 = vector + cornersBookkeeping[2];
			GetAverageLightValue(faceData[index].face, 2, cells, iVector, ref faceData[index].colors[2], cornersBookkeeping[2].x < 0.5f);
			ref Vector3 reference24 = ref faceData[index].faceVertices[3];
			reference24 = vector + cornersBookkeeping[1];
			GetAverageLightValue(faceData[index].face, 3, cells, iVector, ref faceData[index].colors[3], cornersBookkeeping[1].x < 0.5f);
			index++;
		}
	}

	public static Face GetFace(Vector3[] corners, Vector3[] triangleVertices)
	{
		foreach (int value in Enum.GetValues(typeof(Face)))
		{
			int num = 0;
			Vector3[] face2 = GetFace(corners, (Face)value);
			foreach (Vector3 vector in triangleVertices)
			{
				Vector3[] array = face2;
				foreach (Vector3 vector2 in array)
				{
					if ((double)(vector - vector2).magnitude < 0.001)
					{
						num++;
						break;
					}
				}
			}
			if (num == 3)
			{
				return (Face)value;
			}
		}
		return Face.Top;
	}

	public static Vector3[] GetFace(Vector3[] corners, Face face)
	{
		Vector3[] faceVertices = new Vector3[4];
		CubeBase.GetFace(ref corners, ref faceVertices, face);
		return faceVertices;
	}

	public static Vector3[] GetFaceVerticesWorld(GameObject gameObject, Cube cube, Face face, IntVector iVector)
	{
		Vector3 vector = new Vector3(iVector.x, iVector.y, iVector.z);
		Vector3[] face2 = GetFace(cube.Corners, face);
		ref Vector3 reference = ref face2[0];
		reference = gameObject.transform.TransformPoint(face2[0] + vector);
		ref Vector3 reference2 = ref face2[1];
		reference2 = gameObject.transform.TransformPoint(face2[1] + vector);
		ref Vector3 reference3 = ref face2[2];
		reference3 = gameObject.transform.TransformPoint(face2[2] + vector);
		ref Vector3 reference4 = ref face2[3];
		reference4 = gameObject.transform.TransformPoint(face2[3] + vector);
		return face2;
	}

	public static Vector3[] GetEdge(Cube cube, Face face, Edge edge)
	{
		Vector3[] face2 = GetFace(cube.Corners, face);
		Vector3[] array = new Vector3[2];
		switch (edge)
		{
		case Edge.Front:
		{
			ref Vector3 reference7 = ref array[0];
			reference7 = face2[0];
			ref Vector3 reference8 = ref array[1];
			reference8 = face2[1];
			break;
		}
		case Edge.Back:
		{
			ref Vector3 reference5 = ref array[0];
			reference5 = face2[2];
			ref Vector3 reference6 = ref array[1];
			reference6 = face2[3];
			break;
		}
		case Edge.Left:
		{
			ref Vector3 reference3 = ref array[0];
			reference3 = face2[3];
			ref Vector3 reference4 = ref array[1];
			reference4 = face2[0];
			break;
		}
		case Edge.Right:
		{
			ref Vector3 reference = ref array[0];
			reference = face2[1];
			ref Vector3 reference2 = ref array[1];
			reference2 = face2[2];
			break;
		}
		}
		return array;
	}

	public static void SetEdge(Cube cube, Face face, Edge edge, Vector3[] edgeVertices)
	{
		cornersBookkeeping = cube.Corners;
		SetEdge(ref cornersBookkeeping, face, edge, edgeVertices);
		cube.Corners = cornersBookkeeping;
	}

	public static Edge GetEdge(GameObject gameObject, Cube cube, Face face, Vector3 pos, IntVector iVector)
	{
		float num = 1000f;
		Edge result = Edge.None;
		foreach (int value in Enum.GetValues(typeof(Edge)))
		{
			Vector3[] edgeVerticesWorld = GetEdgeVerticesWorld(gameObject, cube, face, (Edge)value, iVector);
			float distance = 1000f;
			MathFunctions.DistancePointLine(pos, edgeVerticesWorld[0], edgeVerticesWorld[1], ref distance);
			if (num > distance)
			{
				result = (Edge)value;
				num = distance;
			}
		}
		return result;
	}

	public static Vector3[] GetEdgeVerticesWorld(GameObject gameObject, Cube cube, Face face, Edge edge, IntVector iVector)
	{
		Vector3 vector = new Vector3(iVector.x, iVector.y, iVector.z);
		Vector3[] edge2 = GetEdge(cube, face, edge);
		ref Vector3 reference = ref edge2[0];
		reference = gameObject.transform.TransformPoint(edge2[0] + vector);
		ref Vector3 reference2 = ref edge2[1];
		reference2 = gameObject.transform.TransformPoint(edge2[1] + vector);
		return edge2;
	}

	private static bool IsOutOfBound(Vector3[] corners)
	{
		for (int i = 0; i < corners.Length; i++)
		{
			Vector3 vector = corners[i];
			for (int j = 0; j < 3; j++)
			{
				if (Mathf.Abs(vector[j]) > 0.5f)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void MoveVertex(CubePickingInfo info, float value, Vector3 axis, bool edgeIndex0, bool edgeIndex1, ref CubeOutOfBoundState coob)
	{
		Vector3[] edge = GetEdge(info.cube, info.pickedFace, info.pickedEdge);
		Vector3[] edge2 = GetEdge(info.cube, info.pickedFace, info.pickedEdge);
		if (edgeIndex0)
		{
			ref Vector3 reference = ref edge[0];
			reference = edge[0] + value * axis;
		}
		if (edgeIndex1)
		{
			ref Vector3 reference2 = ref edge[1];
			reference2 = edge[1] + value * axis;
		}
		float min = -0.5f;
		float max = 0.5f;
		if (IsOutOfBound(edge))
		{
			if (IsFaceBoxSideAligened(info.cube, info.pickedFace))
			{
				Debug.Log("Add cube based on corner pull!");
				coob = CubeOutOfBoundState.OutOfBoundsAddVertex;
			}
			return;
		}
		MathFunctions.ClampVector(ref edge[0], min, max);
		MathFunctions.ClampVector(ref edge[1], min, max);
		ref Vector3 reference3 = ref edge[0];
		reference3 = MathFunctions.RoundVector(edge[0], 3);
		ref Vector3 reference4 = ref edge[1];
		reference4 = MathFunctions.RoundVector(edge[1], 3);
		Vector3[] corners = (Vector3[])info.cube.Corners.Clone();
		SetEdge(ref corners, info.pickedFace, info.pickedEdge, edge);
		if (IsLegal(corners))
		{
			SetEdge(info.cube, info.pickedFace, info.pickedEdge, edge);
			if (edge2[0] != edge[0] || edge2[1] != edge[1])
			{
				coob = CubeOutOfBoundState.WithinBounds;
			}
		}
	}

	public static void MoveEdge(CubePickingInfo info, float value, Vector3 axis, ref CubeOutOfBoundState coob)
	{
		Vector3[] edge = GetEdge(info.cube, info.pickedFace, info.pickedEdge);
		ref Vector3 reference = ref edge[0];
		reference = edge[0] + value * axis;
		ref Vector3 reference2 = ref edge[1];
		reference2 = edge[1] + value * axis;
		float min = -0.5f;
		float max = 0.5f;
		if (IsOutOfBound(edge))
		{
			if (IsFaceBoxSideAligened(info.cube, info.pickedFace))
			{
				coob = CubeOutOfBoundState.OutOfBoundsAddEdge;
			}
			return;
		}
		MathFunctions.ClampVector(ref edge[0], min, max);
		MathFunctions.ClampVector(ref edge[1], min, max);
		ref Vector3 reference3 = ref edge[0];
		reference3 = MathFunctions.RoundVector(edge[0], 3);
		ref Vector3 reference4 = ref edge[1];
		reference4 = MathFunctions.RoundVector(edge[1], 3);
		Vector3[] corners = (Vector3[])info.cube.Corners.Clone();
		SetEdge(ref corners, info.pickedFace, info.pickedEdge, edge);
		if (IsLegal(corners))
		{
			SetEdge(info.cube, info.pickedFace, info.pickedEdge, edge);
			if (coob == CubeOutOfBoundState.NoChange)
			{
				coob = CubeOutOfBoundState.WithinBounds;
			}
		}
	}

	private static bool FaceIsOutOfCubeBoundery(Vector3[] faceVertices)
	{
		float num = -0.5f;
		float num2 = 0.5f;
		for (int i = 0; i < faceVertices.Length; i++)
		{
			ref Vector3 reference = ref faceVertices[i];
			reference = MathFunctions.RoundVector(faceVertices[i], 3);
			int num3 = 0;
			for (int j = 0; j < 3; j++)
			{
				if (faceVertices[i][j] >= num && faceVertices[i][j] <= num2)
				{
					num3++;
				}
			}
			if (num3 == 3)
			{
				return false;
			}
		}
		return true;
	}

	private static void AddDeltaToFace(ref Vector3[] faceVertices, float delta, Vector3 axis)
	{
		for (int i = 0; i < faceVertices.Length; i++)
		{
			ref Vector3 reference = ref faceVertices[i];
			reference = faceVertices[i] + delta * axis;
		}
	}

	private static void ClampFace(ref Vector3[] faceVertices)
	{
		float min = -0.5f;
		float max = 0.5f;
		for (int i = 0; i < faceVertices.Length; i++)
		{
			MathFunctions.ClampVector(ref faceVertices[i], min, max);
			ref Vector3 reference = ref faceVertices[i];
			reference = MathFunctions.RoundVector(faceVertices[i], 3);
		}
	}

	public static void MoveFace(CubePickingInfo info, float delta, Vector3 axis, ref CubeOutOfBoundState outOfBoundState)
	{
		Vector3[] faceVertices = GetFace(info.cube.Corners, info.pickedFace);
		AddDeltaToFace(ref faceVertices, delta, axis);
		bool flag = FaceIsOutOfCubeBoundery(faceVertices);
		ClampFace(ref faceVertices);
		Vector3[] corners = (Vector3[])info.cube.Corners.Clone();
		SetFace(ref corners, info.pickedFace, faceVertices);
		if (IsLegal(corners))
		{
			SetFace(info.cube, info.pickedFace, faceVertices);
		}
		if (flag)
		{
			if (IsCollapsed(info.cube.Corners))
			{
				outOfBoundState = CubeOutOfBoundState.OutOfBoundsRemove;
			}
			else
			{
				outOfBoundState = CubeOutOfBoundState.OutOfBoundsAdd;
			}
		}
		else
		{
			outOfBoundState = CubeOutOfBoundState.WithinBounds;
		}
	}

	private static List<Vector3> GetCorners(Vector3[] counterClockwiseFace, Face direction)
	{
		float num = 0.5f;
		Quaternion toTopRotation = GetToTopRotation(direction);
		for (int i = 0; i < counterClockwiseFace.Length; i++)
		{
			ref Vector3 reference = ref counterClockwiseFace[i];
			reference = toTopRotation * counterClockwiseFace[i];
			ref Vector3 reference2 = ref counterClockwiseFace[i];
			reference2 = new Vector3(counterClockwiseFace[i].x, 0f, counterClockwiseFace[i].z);
		}
		List<Vector3> list = new List<Vector3>();
		list.Add(new Vector3(counterClockwiseFace[0].x, num, counterClockwiseFace[0].z));
		list.Add(new Vector3(counterClockwiseFace[1].x, num, counterClockwiseFace[1].z));
		list.Add(new Vector3(counterClockwiseFace[2].x, num, counterClockwiseFace[2].z));
		list.Add(new Vector3(counterClockwiseFace[3].x, num, counterClockwiseFace[3].z));
		list.Add(new Vector3(counterClockwiseFace[3].x, 0f - num, counterClockwiseFace[3].z));
		list.Add(new Vector3(counterClockwiseFace[2].x, 0f - num, counterClockwiseFace[2].z));
		list.Add(new Vector3(counterClockwiseFace[1].x, 0f - num, counterClockwiseFace[1].z));
		list.Add(new Vector3(counterClockwiseFace[0].x, 0f - num, counterClockwiseFace[0].z));
		List<Vector3> cubeCorners = list;
		return CreateCubeCornersFromTopFace(cubeCorners, direction);
	}

	private static Vector3[] GetVertices(Vector3[] corners)
	{
		List<Vector3> list = new List<Vector3>(corners);
		list.Add(list[7]);
		list.Add(list[6]);
		list.Add(list[1]);
		list.Add(list[0]);
		list.Add(list[5]);
		list.Add(list[4]);
		list.Add(list[3]);
		list.Add(list[2]);
		list.Add(list[4]);
		list.Add(list[7]);
		list.Add(list[0]);
		list.Add(list[3]);
		list.Add(list[6]);
		list.Add(list[5]);
		list.Add(list[2]);
		list.Add(list[1]);
		return list.ToArray();
	}

	private static void SetFace(ref Vector3[] corners, Face face, Vector3[] faceVertices)
	{
		switch (face)
		{
		case Face.Top:
		{
			ref Vector3 reference21 = ref corners[0];
			reference21 = faceVertices[0];
			ref Vector3 reference22 = ref corners[1];
			reference22 = faceVertices[1];
			ref Vector3 reference23 = ref corners[2];
			reference23 = faceVertices[2];
			ref Vector3 reference24 = ref corners[3];
			reference24 = faceVertices[3];
			break;
		}
		case Face.Bottom:
		{
			ref Vector3 reference17 = ref corners[4];
			reference17 = faceVertices[0];
			ref Vector3 reference18 = ref corners[5];
			reference18 = faceVertices[1];
			ref Vector3 reference19 = ref corners[6];
			reference19 = faceVertices[2];
			ref Vector3 reference20 = ref corners[7];
			reference20 = faceVertices[3];
			break;
		}
		case Face.Back:
		{
			ref Vector3 reference13 = ref corners[5];
			reference13 = faceVertices[0];
			ref Vector3 reference14 = ref corners[4];
			reference14 = faceVertices[1];
			ref Vector3 reference15 = ref corners[3];
			reference15 = faceVertices[2];
			ref Vector3 reference16 = ref corners[2];
			reference16 = faceVertices[3];
			break;
		}
		case Face.Front:
		{
			ref Vector3 reference9 = ref corners[7];
			reference9 = faceVertices[0];
			ref Vector3 reference10 = ref corners[6];
			reference10 = faceVertices[1];
			ref Vector3 reference11 = ref corners[1];
			reference11 = faceVertices[2];
			ref Vector3 reference12 = ref corners[0];
			reference12 = faceVertices[3];
			break;
		}
		case Face.Left:
		{
			ref Vector3 reference5 = ref corners[4];
			reference5 = faceVertices[0];
			ref Vector3 reference6 = ref corners[7];
			reference6 = faceVertices[1];
			ref Vector3 reference7 = ref corners[0];
			reference7 = faceVertices[2];
			ref Vector3 reference8 = ref corners[3];
			reference8 = faceVertices[3];
			break;
		}
		case Face.Right:
		{
			ref Vector3 reference = ref corners[6];
			reference = faceVertices[0];
			ref Vector3 reference2 = ref corners[5];
			reference2 = faceVertices[1];
			ref Vector3 reference3 = ref corners[2];
			reference3 = faceVertices[2];
			ref Vector3 reference4 = ref corners[1];
			reference4 = faceVertices[3];
			break;
		}
		}
	}

	private static List<Vector3> SquareCornersToCubeCorners(List<Vector2> corners, Face direction)
	{
		float num = 0.5f;
		Vector3 vector = new Vector3(0f - num, 0f - num, 0f - num);
		List<Vector3> list = new List<Vector3>();
		list.Add(new Vector3(corners[0].x, 1f, corners[0].y) + vector);
		list.Add(new Vector3(corners[3].x, 1f, corners[3].y) + vector);
		list.Add(new Vector3(corners[2].x, 1f, corners[2].y) + vector);
		list.Add(new Vector3(corners[1].x, 1f, corners[1].y) + vector);
		list.Add(new Vector3(corners[1].x, 0f, corners[1].y) + vector);
		list.Add(new Vector3(corners[2].x, 0f, corners[2].y) + vector);
		list.Add(new Vector3(corners[3].x, 0f, corners[3].y) + vector);
		list.Add(new Vector3(corners[0].x, 0f, corners[0].y) + vector);
		List<Vector3> cubeCorners = list;
		return CreateCubeCornersFromTopFace(cubeCorners, direction);
	}

	private static List<Vector3> CreateCubeCornersFromTopFace(List<Vector3> cubeCorners, Face direction)
	{
		List<Vector3> list = new List<Vector3>(8);
		List<Vector3> list2 = new List<Vector3>(8);
		for (int i = 0; i < cubeCorners.Count; i++)
		{
			list.Add(cubeCorners[i]);
		}
		for (int j = 0; j < cubeCorners.Count; j++)
		{
			list2.Add(cubeCorners[j]);
		}
		switch (direction)
		{
		case Face.Bottom:
			list[7] = cubeCorners[3];
			list[6] = cubeCorners[2];
			list[4] = cubeCorners[0];
			list[5] = cubeCorners[1];
			list[3] = cubeCorners[7];
			list[2] = cubeCorners[6];
			list[0] = cubeCorners[4];
			list[1] = cubeCorners[5];
			cubeCorners = list;
			break;
		case Face.Back:
			list2[0] = cubeCorners[3];
			list2[1] = cubeCorners[2];
			list2[3] = cubeCorners[4];
			list2[2] = cubeCorners[5];
			list2[7] = cubeCorners[0];
			list2[6] = cubeCorners[1];
			list2[4] = cubeCorners[7];
			list2[5] = cubeCorners[6];
			list[0] = list2[2];
			list[1] = list2[3];
			list[2] = list2[0];
			list[3] = list2[1];
			list[4] = list2[6];
			list[5] = list2[7];
			list[6] = list2[4];
			list[7] = list2[5];
			cubeCorners = list;
			break;
		case Face.Front:
			list[0] = cubeCorners[3];
			list[1] = cubeCorners[2];
			list[3] = cubeCorners[4];
			list[2] = cubeCorners[5];
			list[7] = cubeCorners[0];
			list[6] = cubeCorners[1];
			list[4] = cubeCorners[7];
			list[5] = cubeCorners[6];
			cubeCorners = list;
			break;
		case Face.Left:
			list2[0] = cubeCorners[3];
			list2[1] = cubeCorners[2];
			list2[3] = cubeCorners[4];
			list2[2] = cubeCorners[5];
			list2[7] = cubeCorners[0];
			list2[6] = cubeCorners[1];
			list2[4] = cubeCorners[7];
			list2[5] = cubeCorners[6];
			list[0] = list2[1];
			list[1] = list2[2];
			list[2] = list2[3];
			list[3] = list2[0];
			list[4] = list2[7];
			list[5] = list2[4];
			list[6] = list2[5];
			list[7] = list2[6];
			cubeCorners = list;
			break;
		case Face.Right:
			list2[0] = cubeCorners[3];
			list2[1] = cubeCorners[2];
			list2[3] = cubeCorners[4];
			list2[2] = cubeCorners[5];
			list2[7] = cubeCorners[0];
			list2[6] = cubeCorners[1];
			list2[4] = cubeCorners[7];
			list2[5] = cubeCorners[6];
			list[0] = list2[3];
			list[1] = list2[0];
			list[2] = list2[1];
			list[3] = list2[2];
			list[4] = list2[5];
			list[5] = list2[6];
			list[6] = list2[7];
			list[7] = list2[4];
			cubeCorners = list;
			break;
		}
		Quaternion fromTopRotation = GetFromTopRotation(direction);
		for (int k = 0; k < cubeCorners.Count; k++)
		{
			Vector3 vector = fromTopRotation * cubeCorners[k];
			cubeCorners[k] = MathFunctions.RoundVector(vector, 3);
		}
		return cubeCorners;
	}

	private static Quaternion GetFromTopRotation(Face direction)
	{
		Quaternion result = Quaternion.identity;
		Quaternion identity = Quaternion.identity;
		Quaternion identity2 = Quaternion.identity;
		Quaternion identity3 = Quaternion.identity;
		identity.SetFromToRotation(Vector3.forward, Vector3.left);
		identity3.SetFromToRotation(Vector3.up, Vector3.back);
		identity2.SetFromToRotation(Vector3.up, Vector3.right);
		switch (direction)
		{
		case Face.Bottom:
			result = identity3 * identity3;
			break;
		case Face.Back:
			result = identity * identity * identity3;
			break;
		case Face.Front:
			result = identity3;
			break;
		case Face.Left:
			result = Quaternion.Inverse(identity) * identity3;
			break;
		case Face.Right:
			result = identity * identity3;
			break;
		}
		return result;
	}

	private static Vector3[] RotateFaceToTop(Cube cube, Face direction)
	{
		cornersBookkeeping = cube.Corners;
		Vector3[] array = new Vector3[8]
		{
			cornersBookkeeping[0],
			cornersBookkeeping[1],
			cornersBookkeeping[2],
			cornersBookkeeping[3],
			cornersBookkeeping[4],
			cornersBookkeeping[5],
			cornersBookkeeping[6],
			cornersBookkeeping[7]
		};
		Quaternion toTopRotation = GetToTopRotation(direction);
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 vector = toTopRotation * array[i];
			ref Vector3 reference = ref array[i];
			reference = MathFunctions.RoundVector(vector, 3);
		}
		return array;
	}

	private static Quaternion GetToTopRotation(Face direction)
	{
		Quaternion result = Quaternion.identity;
		Quaternion identity = Quaternion.identity;
		Quaternion identity2 = Quaternion.identity;
		Quaternion identity3 = Quaternion.identity;
		identity.SetFromToRotation(Vector3.forward, Vector3.left);
		identity3.SetFromToRotation(Vector3.up, Vector3.back);
		identity2.SetFromToRotation(Vector3.up, Vector3.right);
		switch (direction)
		{
		case Face.Bottom:
			result = Quaternion.Inverse(identity3) * Quaternion.Inverse(identity3);
			break;
		case Face.Back:
			result = Quaternion.Inverse(identity3) * Quaternion.Inverse(identity) * Quaternion.Inverse(identity);
			break;
		case Face.Front:
			result = Quaternion.Inverse(identity3);
			break;
		case Face.Left:
			result = Quaternion.Inverse(identity3) * identity;
			break;
		case Face.Right:
			result = Quaternion.Inverse(identity3) * Quaternion.Inverse(identity);
			break;
		}
		return result;
	}

	private static void SetEdge(ref Vector3[] corners, Face face, Edge edge, Vector3[] edgeVertices)
	{
		Vector3[] face2 = GetFace(corners, face);
		switch (edge)
		{
		case Edge.Front:
		{
			ref Vector3 reference7 = ref face2[0];
			reference7 = edgeVertices[0];
			ref Vector3 reference8 = ref face2[1];
			reference8 = edgeVertices[1];
			break;
		}
		case Edge.Back:
		{
			ref Vector3 reference5 = ref face2[2];
			reference5 = edgeVertices[0];
			ref Vector3 reference6 = ref face2[3];
			reference6 = edgeVertices[1];
			break;
		}
		case Edge.Left:
		{
			ref Vector3 reference3 = ref face2[3];
			reference3 = edgeVertices[0];
			ref Vector3 reference4 = ref face2[0];
			reference4 = edgeVertices[1];
			break;
		}
		case Edge.Right:
		{
			ref Vector3 reference = ref face2[1];
			reference = edgeVertices[0];
			ref Vector3 reference2 = ref face2[2];
			reference2 = edgeVertices[1];
			break;
		}
		}
		SetFace(ref corners, face, face2);
	}

	private static bool IsFaceCollapsed(Vector3[] faceIndices)
	{
		for (int i = 0; i < faceIndices.Length; i++)
		{
			if ((faceIndices[(i + 1) % faceIndices.Length] - faceIndices[i % faceIndices.Length]).magnitude < 0.001f)
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsFaceValid(Vector3[] faceIndices, Face face)
	{
		Vector3 normalized = Vector3.Cross((faceIndices[1] - faceIndices[0]).normalized, (faceIndices[2] - faceIndices[1]).normalized).normalized;
		Plane plane = new Plane(normalized, faceIndices[0]);
		for (int i = 1; i < faceIndices.Length; i++)
		{
			if (Mathf.Abs(plane.GetDistanceToPoint(faceIndices[i])) > 1E-05f)
			{
				return false;
			}
		}
		float num = Vector3.Dot(GetFaceAxis(face), normalized);
		if (num > 0f)
		{
			return false;
		}
		return true;
	}

	public static bool IsCollapsed(Vector3[] corners)
	{
		Vector3[] planeVertices = new Vector3[3];
		if (GetPlaneVertices(corners, ref planeVertices))
		{
			Plane plane = new Plane(planeVertices[0], planeVertices[1], planeVertices[2]);
			foreach (Vector3 inPt in corners)
			{
				if ((double)Mathf.Abs(plane.GetDistanceToPoint(inPt)) > 0.01)
				{
					return false;
				}
			}
		}
		return true;
	}

	public static bool GetPlaneVertices(Vector3[] corners, ref Vector3[] planeVertices)
	{
		HashSet<Vector3> hashSet = new HashSet<Vector3>();
		for (int i = 0; i < corners.Length; i++)
		{
			hashSet.Add(corners[i]);
		}
		if (hashSet.Count < 3)
		{
			return false;
		}
		List<Vector3> list = new List<Vector3>(hashSet);
		List<Vector3> list2 = new List<Vector3>();
		list2.Add(list[0]);
		list2.Add(list[1]);
		Vector3 normalized = (list[1] - list[0]).normalized;
		for (int j = 2; j < list.Count; j++)
		{
			float num = Vector3.Dot(normalized, (list[j] - list[0]).normalized);
			if (num < 0.99f && (double)num > -0.99)
			{
				list2.Add(list[j]);
				planeVertices = list2.ToArray();
				return true;
			}
		}
		return false;
	}

	public static bool IsLegal(Vector3[] corners)
	{
		foreach (int value in Enum.GetValues(typeof(Face)))
		{
			Vector3[] face2 = GetFace(corners, (Face)value);
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < face2.Length; i++)
			{
				zero += Vector3.Cross(face2[i % face2.Length], face2[(i + 1) % face2.Length]);
			}
			zero.Normalize();
			zero = -zero;
			Vector3 faceAxis = GetFaceAxis((Face)value);
			float num = Vector3.Dot(zero, faceAxis);
			if ((double)num < -0.0001)
			{
				return false;
			}
		}
		return true;
	}

	private static bool IsCornersValid(Vector3[] corners)
	{
		for (int i = 0; i < 6; i++)
		{
			if (!IsFaceValid(GetFace(corners, (Face)i), (Face)i))
			{
				return false;
			}
		}
		return true;
	}

	private static void GetTriangle(int triangleNr, Vector3[] triangleVertices, Vector3[] corners)
	{
		switch (triangleNr)
		{
		case 0:
		{
			ref Vector3 reference34 = ref triangleVertices[0];
			reference34 = corners[0];
			ref Vector3 reference35 = ref triangleVertices[1];
			reference35 = corners[1];
			ref Vector3 reference36 = ref triangleVertices[2];
			reference36 = corners[2];
			break;
		}
		case 1:
		{
			ref Vector3 reference31 = ref triangleVertices[0];
			reference31 = corners[0];
			ref Vector3 reference32 = ref triangleVertices[1];
			reference32 = corners[2];
			ref Vector3 reference33 = ref triangleVertices[2];
			reference33 = corners[3];
			break;
		}
		case 2:
		{
			ref Vector3 reference28 = ref triangleVertices[0];
			reference28 = corners[4];
			ref Vector3 reference29 = ref triangleVertices[1];
			reference29 = corners[5];
			ref Vector3 reference30 = ref triangleVertices[2];
			reference30 = corners[6];
			break;
		}
		case 3:
		{
			ref Vector3 reference25 = ref triangleVertices[0];
			reference25 = corners[4];
			ref Vector3 reference26 = ref triangleVertices[1];
			reference26 = corners[6];
			ref Vector3 reference27 = ref triangleVertices[2];
			reference27 = corners[7];
			break;
		}
		case 4:
		{
			ref Vector3 reference22 = ref triangleVertices[0];
			reference22 = corners[0];
			ref Vector3 reference23 = ref triangleVertices[1];
			reference23 = corners[1];
			ref Vector3 reference24 = ref triangleVertices[2];
			reference24 = corners[2];
			break;
		}
		case 5:
		{
			ref Vector3 reference19 = ref triangleVertices[0];
			reference19 = corners[0];
			ref Vector3 reference20 = ref triangleVertices[1];
			reference20 = corners[1];
			ref Vector3 reference21 = ref triangleVertices[2];
			reference21 = corners[2];
			break;
		}
		case 6:
		{
			ref Vector3 reference16 = ref triangleVertices[0];
			reference16 = corners[0];
			ref Vector3 reference17 = ref triangleVertices[1];
			reference17 = corners[1];
			ref Vector3 reference18 = ref triangleVertices[2];
			reference18 = corners[2];
			break;
		}
		case 7:
		{
			ref Vector3 reference13 = ref triangleVertices[0];
			reference13 = corners[0];
			ref Vector3 reference14 = ref triangleVertices[1];
			reference14 = corners[1];
			ref Vector3 reference15 = ref triangleVertices[2];
			reference15 = corners[2];
			break;
		}
		case 8:
		{
			ref Vector3 reference10 = ref triangleVertices[0];
			reference10 = corners[0];
			ref Vector3 reference11 = ref triangleVertices[1];
			reference11 = corners[1];
			ref Vector3 reference12 = ref triangleVertices[2];
			reference12 = corners[2];
			break;
		}
		case 9:
		{
			ref Vector3 reference7 = ref triangleVertices[0];
			reference7 = corners[0];
			ref Vector3 reference8 = ref triangleVertices[1];
			reference8 = corners[1];
			ref Vector3 reference9 = ref triangleVertices[2];
			reference9 = corners[2];
			break;
		}
		case 10:
		{
			ref Vector3 reference4 = ref triangleVertices[0];
			reference4 = corners[0];
			ref Vector3 reference5 = ref triangleVertices[1];
			reference5 = corners[1];
			ref Vector3 reference6 = ref triangleVertices[2];
			reference6 = corners[2];
			break;
		}
		case 11:
		{
			ref Vector3 reference = ref triangleVertices[0];
			reference = corners[0];
			ref Vector3 reference2 = ref triangleVertices[1];
			reference2 = corners[1];
			ref Vector3 reference3 = ref triangleVertices[2];
			reference3 = corners[2];
			break;
		}
		}
	}
}
