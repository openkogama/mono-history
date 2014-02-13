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
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		IntVector result = new IntVector(localPos.x, localPos.y, localPos.z);
		Vector3 faceAxis = GetFaceAxis(face);
		result.x += (short)faceAxis.x;
		result.y += (short)faceAxis.y;
		result.z += (short)faceAxis.z;
		return result;
	}

	public static Face GetFaceIdentityFromLocalDir(Vector3 localDir)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = MathFunctions.AbsVector(localDir);
		if (val.x >= val.y && val.x >= val.z)
		{
			if (localDir.x < 0f)
			{
				return Face.Left;
			}
			return Face.Right;
		}
		if (val.y >= val.x && val.y >= val.z)
		{
			if (localDir.y < 0f)
			{
				return Face.Bottom;
			}
			return Face.Top;
		}
		if (val.z >= val.x && val.z >= val.y)
		{
			if (localDir.z < 0f)
			{
				return Face.Front;
			}
			return Face.Back;
		}
		Debug.LogError((object)"no face found");
		return Face.Front;
	}

	public static Vector3 GetFaceAxis(Face face)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] vertices = GetVertices(cube.Corners);
		Vector3 val = new Vector3((float)iVector.x, (float)iVector.y, (float)iVector.z);
		for (int i = 0; i < vertices.Length; i++)
		{
			ref Vector3 reference = ref vertices[i];
			reference = val + vertices[i];
		}
		return vertices;
	}

	private static void GetAverageLightValue(Face face, int vertex, Dictionary<IntVector, Cell> cells, IntVector cubePos, ref Color color, bool inside)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
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
			IntVector key = array[i] + cubePos;
			num2 = ((!cells.TryGetValue(key, out var value)) ? (num2 + 255) : (num2 + value.lightValue));
		}
		color.r = (float)num2 / 1020f;
		color.b = (float)num2 / 1020f;
		color.g = (float)num2 / 1020f;
	}

	public static void GetVisibleFaceVertices(Cube cube, ref CubeModelChunk.FaceData[] faceData, IntVector iVector, Dictionary<IntVector, Cell> cells, ref int index)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0759: Unknown result type (might be due to invalid IL or missing references)
		//IL_075e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0763: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_081b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0827: Unknown result type (might be due to invalid IL or missing references)
		//IL_082c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0831: Unknown result type (might be due to invalid IL or missing references)
		//IL_0882: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0893: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Unknown result type (might be due to invalid IL or missing references)
		//IL_090b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0917: Unknown result type (might be due to invalid IL or missing references)
		//IL_091c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0921: Unknown result type (might be due to invalid IL or missing references)
		//IL_0972: Unknown result type (might be due to invalid IL or missing references)
		//IL_097e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0983: Unknown result type (might be due to invalid IL or missing references)
		//IL_0988: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = new Vector3((float)iVector.x, (float)iVector.y, (float)iVector.z);
		CubeBase.GetCorners(cube, ref cornersBookkeeping);
		index = 0;
		if ((cube.hiddenSides & 1) == 0)
		{
			faceData[index].face = Face.Top;
			ref Vector3 reference = ref faceData[index].faceVertices[0];
			reference = val + cornersBookkeeping[0];
			GetAverageLightValue(faceData[index].face, 0, cells, iVector, ref faceData[index].colors[0], cornersBookkeeping[0].y < 0.5f);
			ref Vector3 reference2 = ref faceData[index].faceVertices[1];
			reference2 = val + cornersBookkeeping[1];
			GetAverageLightValue(faceData[index].face, 1, cells, iVector, ref faceData[index].colors[1], cornersBookkeeping[1].y < 0.5f);
			ref Vector3 reference3 = ref faceData[index].faceVertices[2];
			reference3 = val + cornersBookkeeping[2];
			GetAverageLightValue(faceData[index].face, 2, cells, iVector, ref faceData[index].colors[2], cornersBookkeeping[2].y < 0.5f);
			ref Vector3 reference4 = ref faceData[index].faceVertices[3];
			reference4 = val + cornersBookkeeping[3];
			GetAverageLightValue(faceData[index].face, 3, cells, iVector, ref faceData[index].colors[3], cornersBookkeeping[3].y < 0.5f);
			index++;
		}
		if ((cube.hiddenSides & 2) == 0)
		{
			faceData[index].face = Face.Bottom;
			ref Vector3 reference5 = ref faceData[index].faceVertices[0];
			reference5 = val + cornersBookkeeping[4];
			GetAverageLightValue(faceData[index].face, 0, cells, iVector, ref faceData[index].colors[0], cornersBookkeeping[4].y > -0.5f);
			ref Vector3 reference6 = ref faceData[index].faceVertices[1];
			reference6 = val + cornersBookkeeping[5];
			GetAverageLightValue(faceData[index].face, 1, cells, iVector, ref faceData[index].colors[1], cornersBookkeeping[5].y > -0.5f);
			ref Vector3 reference7 = ref faceData[index].faceVertices[2];
			reference7 = val + cornersBookkeeping[6];
			GetAverageLightValue(faceData[index].face, 2, cells, iVector, ref faceData[index].colors[2], cornersBookkeeping[6].y > -0.5f);
			ref Vector3 reference8 = ref faceData[index].faceVertices[3];
			reference8 = val + cornersBookkeeping[7];
			GetAverageLightValue(faceData[index].face, 3, cells, iVector, ref faceData[index].colors[3], cornersBookkeeping[7].y > -0.5f);
			index++;
		}
		if ((cube.hiddenSides & 4) == 0)
		{
			faceData[index].face = Face.Front;
			ref Vector3 reference9 = ref faceData[index].faceVertices[0];
			reference9 = val + cornersBookkeeping[7];
			GetAverageLightValue(faceData[index].face, 0, cells, iVector, ref faceData[index].colors[0], cornersBookkeeping[7].z > -0.5f);
			ref Vector3 reference10 = ref faceData[index].faceVertices[1];
			reference10 = val + cornersBookkeeping[6];
			GetAverageLightValue(faceData[index].face, 1, cells, iVector, ref faceData[index].colors[1], cornersBookkeeping[6].z > -0.5f);
			ref Vector3 reference11 = ref faceData[index].faceVertices[2];
			reference11 = val + cornersBookkeeping[1];
			GetAverageLightValue(faceData[index].face, 2, cells, iVector, ref faceData[index].colors[2], cornersBookkeeping[1].z > -0.5f);
			ref Vector3 reference12 = ref faceData[index].faceVertices[3];
			reference12 = val + cornersBookkeeping[0];
			GetAverageLightValue(faceData[index].face, 3, cells, iVector, ref faceData[index].colors[3], cornersBookkeeping[0].z > -0.5f);
			index++;
		}
		if ((cube.hiddenSides & 8) == 0)
		{
			faceData[index].face = Face.Back;
			ref Vector3 reference13 = ref faceData[index].faceVertices[0];
			reference13 = val + cornersBookkeeping[5];
			GetAverageLightValue(faceData[index].face, 0, cells, iVector, ref faceData[index].colors[0], cornersBookkeeping[5].z < 0.5f);
			ref Vector3 reference14 = ref faceData[index].faceVertices[1];
			reference14 = val + cornersBookkeeping[4];
			GetAverageLightValue(faceData[index].face, 1, cells, iVector, ref faceData[index].colors[1], cornersBookkeeping[4].z < 0.5f);
			ref Vector3 reference15 = ref faceData[index].faceVertices[2];
			reference15 = val + cornersBookkeeping[3];
			GetAverageLightValue(faceData[index].face, 2, cells, iVector, ref faceData[index].colors[2], cornersBookkeeping[3].z < 0.5f);
			ref Vector3 reference16 = ref faceData[index].faceVertices[3];
			reference16 = val + cornersBookkeeping[2];
			GetAverageLightValue(faceData[index].face, 3, cells, iVector, ref faceData[index].colors[3], cornersBookkeeping[2].z < 0.5f);
			index++;
		}
		if ((cube.hiddenSides & 0x10) == 0)
		{
			faceData[index].face = Face.Left;
			ref Vector3 reference17 = ref faceData[index].faceVertices[0];
			reference17 = val + cornersBookkeeping[4];
			GetAverageLightValue(faceData[index].face, 0, cells, iVector, ref faceData[index].colors[0], cornersBookkeeping[4].x > -0.5f);
			ref Vector3 reference18 = ref faceData[index].faceVertices[1];
			reference18 = val + cornersBookkeeping[7];
			GetAverageLightValue(faceData[index].face, 1, cells, iVector, ref faceData[index].colors[1], cornersBookkeeping[7].x > -0.5f);
			ref Vector3 reference19 = ref faceData[index].faceVertices[2];
			reference19 = val + cornersBookkeeping[0];
			GetAverageLightValue(faceData[index].face, 2, cells, iVector, ref faceData[index].colors[2], cornersBookkeeping[0].x > -0.5f);
			ref Vector3 reference20 = ref faceData[index].faceVertices[3];
			reference20 = val + cornersBookkeeping[3];
			GetAverageLightValue(faceData[index].face, 3, cells, iVector, ref faceData[index].colors[3], cornersBookkeeping[3].x > -0.5f);
			index++;
		}
		if ((cube.hiddenSides & 0x20) == 0)
		{
			faceData[index].face = Face.Right;
			ref Vector3 reference21 = ref faceData[index].faceVertices[0];
			reference21 = val + cornersBookkeeping[6];
			GetAverageLightValue(faceData[index].face, 0, cells, iVector, ref faceData[index].colors[0], cornersBookkeeping[6].x < 0.5f);
			ref Vector3 reference22 = ref faceData[index].faceVertices[1];
			reference22 = val + cornersBookkeeping[5];
			GetAverageLightValue(faceData[index].face, 1, cells, iVector, ref faceData[index].colors[1], cornersBookkeeping[5].x < 0.5f);
			ref Vector3 reference23 = ref faceData[index].faceVertices[2];
			reference23 = val + cornersBookkeeping[2];
			GetAverageLightValue(faceData[index].face, 2, cells, iVector, ref faceData[index].colors[2], cornersBookkeeping[2].x < 0.5f);
			ref Vector3 reference24 = ref faceData[index].faceVertices[3];
			reference24 = val + cornersBookkeeping[1];
			GetAverageLightValue(faceData[index].face, 3, cells, iVector, ref faceData[index].colors[3], cornersBookkeeping[1].x < 0.5f);
			index++;
		}
	}

	public static Face GetFace(Vector3[] corners, Vector3[] triangleVertices)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		foreach (int value in Enum.GetValues(typeof(Face)))
		{
			int num = 0;
			Vector3[] face2 = GetFace(corners, (Face)value);
			foreach (Vector3 val in triangleVertices)
			{
				Vector3[] array = face2;
				foreach (Vector3 val2 in array)
				{
					Vector3 val3 = val - val2;
					if ((double)val3.magnitude < 0.001)
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
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = new Vector3((float)iVector.x, (float)iVector.y, (float)iVector.z);
		Vector3[] face2 = GetFace(cube.Corners, face);
		ref Vector3 reference = ref face2[0];
		reference = gameObject.transform.TransformPoint(face2[0] + val);
		ref Vector3 reference2 = ref face2[1];
		reference2 = gameObject.transform.TransformPoint(face2[1] + val);
		ref Vector3 reference3 = ref face2[2];
		reference3 = gameObject.transform.TransformPoint(face2[2] + val);
		ref Vector3 reference4 = ref face2[3];
		reference4 = gameObject.transform.TransformPoint(face2[3] + val);
		return face2;
	}

	public static Vector3[] GetEdge(Cube cube, Face face, Edge edge)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = new Vector3((float)iVector.x, (float)iVector.y, (float)iVector.z);
		Vector3[] edge2 = GetEdge(cube, face, edge);
		ref Vector3 reference = ref edge2[0];
		reference = gameObject.transform.TransformPoint(edge2[0] + val);
		ref Vector3 reference2 = ref edge2[1];
		reference2 = gameObject.transform.TransformPoint(edge2[1] + val);
		return edge2;
	}

	private static bool IsOutOfBound(Vector3[] corners)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < corners.Length; i++)
		{
			Vector3 val = corners[i];
			for (int j = 0; j < 3; j++)
			{
				if (Mathf.Abs(val[j]) > 0.5f)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void MoveVertex(CubePickingInfo info, float value, Vector3 axis, bool edgeIndex0, bool edgeIndex1, ref CubeOutOfBoundState coob)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
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
				Debug.Log((object)"Add cube based on corner pull!");
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
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < faceVertices.Length; i++)
		{
			ref Vector3 reference = ref faceVertices[i];
			reference = faceVertices[i] + delta * axis;
		}
	}

	private static void ClampFace(ref Vector3[] faceVertices)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.5f;
		Vector3 val = new Vector3(0f - num, 0f - num, 0f - num);
		List<Vector3> list = new List<Vector3>();
		list.Add(new Vector3(corners[0].x, 1f, corners[0].y) + val);
		list.Add(new Vector3(corners[3].x, 1f, corners[3].y) + val);
		list.Add(new Vector3(corners[2].x, 1f, corners[2].y) + val);
		list.Add(new Vector3(corners[1].x, 1f, corners[1].y) + val);
		list.Add(new Vector3(corners[1].x, 0f, corners[1].y) + val);
		list.Add(new Vector3(corners[2].x, 0f, corners[2].y) + val);
		list.Add(new Vector3(corners[3].x, 0f, corners[3].y) + val);
		list.Add(new Vector3(corners[0].x, 0f, corners[0].y) + val);
		List<Vector3> cubeCorners = list;
		return CreateCubeCornersFromTopFace(cubeCorners, direction);
	}

	private static List<Vector3> CreateCubeCornersFromTopFace(List<Vector3> cubeCorners, Face direction)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < faceIndices.Length; i++)
		{
			Vector3 val = faceIndices[(i + 1) % faceIndices.Length] - faceIndices[i % faceIndices.Length];
			if (val.magnitude < 0.001f)
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsFaceValid(Vector3[] faceIndices, Face face)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = faceIndices[1] - faceIndices[0];
		Vector3 normalized = val.normalized;
		Vector3 val2 = faceIndices[2] - faceIndices[1];
		Vector3 val3 = Vector3.Cross(normalized, val2.normalized);
		Vector3 normalized2 = val3.normalized;
		Plane val4 = new Plane(normalized2, faceIndices[0]);
		for (int i = 1; i < faceIndices.Length; i++)
		{
			if (Mathf.Abs(val4.GetDistanceToPoint(faceIndices[i])) > 1E-05f)
			{
				return false;
			}
		}
		float num = Vector3.Dot(GetFaceAxis(face), normalized2);
		if (num > 0f)
		{
			return false;
		}
		return true;
	}

	public static bool IsCollapsed(Vector3[] corners)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] planeVertices = new Vector3[3];
		if (GetPlaneVertices(corners, ref planeVertices))
		{
			Plane val = new Plane(planeVertices[0], planeVertices[1], planeVertices[2]);
			foreach (Vector3 val2 in corners)
			{
				if ((double)Mathf.Abs(val.GetDistanceToPoint(val2)) > 0.01)
				{
					return false;
				}
			}
		}
		return true;
	}

	public static bool GetPlaneVertices(Vector3[] corners, ref Vector3[] planeVertices)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
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
		Vector3 val = list[1] - list[0];
		Vector3 normalized = val.normalized;
		for (int j = 2; j < list.Count; j++)
		{
			Vector3 val2 = list[j] - list[0];
			float num = Vector3.Dot(normalized, val2.normalized);
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
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		foreach (int value in Enum.GetValues(typeof(Face)))
		{
			Vector3[] face2 = GetFace(corners, (Face)value);
			Vector3 val = Vector3.zero;
			for (int i = 0; i < face2.Length; i++)
			{
				val += Vector3.Cross(face2[i % face2.Length], face2[(i + 1) % face2.Length]);
			}
			val.Normalize();
			val = -val;
			Vector3 faceAxis = GetFaceAxis((Face)value);
			float num = Vector3.Dot(val, faceAxis);
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
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
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
