using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

internal static class SharedCubeFunctions
{
	public const float LowestCubeSize = 0.0625f;

	public const float CubeSegmentSize = 0.25f;

	public const float Gridsize = 1f;

	public const float NoneGridSize = 0.0625f;

	private const float forceEdgeDistance = 0.15f;

	private static IntVector constaint = new IntVector(24, 24, 24);

	public static IntVector[][] LightTestOffsets = new IntVector[24][]
	{
		new IntVector[4]
		{
			new IntVector(-1, 1, -1),
			new IntVector(-1, 1, 0),
			new IntVector(0, 1, 0),
			new IntVector(0, 1, -1)
		},
		new IntVector[4]
		{
			new IntVector(0, 1, -1),
			new IntVector(0, 1, 0),
			new IntVector(1, 1, 0),
			new IntVector(1, 1, -1)
		},
		new IntVector[4]
		{
			new IntVector(0, 1, 0),
			new IntVector(0, 1, 1),
			new IntVector(1, 1, 1),
			new IntVector(1, 1, 0)
		},
		new IntVector[4]
		{
			new IntVector(-1, 1, 0),
			new IntVector(-1, 1, 1),
			new IntVector(0, 1, 1),
			new IntVector(0, 1, 0)
		},
		new IntVector[4]
		{
			new IntVector(-1, -1, 0),
			new IntVector(-1, -1, 1),
			new IntVector(0, -1, 1),
			new IntVector(0, -1, 0)
		},
		new IntVector[4]
		{
			new IntVector(0, -1, 0),
			new IntVector(0, -1, 1),
			new IntVector(1, -1, 1),
			new IntVector(1, -1, 0)
		},
		new IntVector[4]
		{
			new IntVector(0, -1, -1),
			new IntVector(0, -1, 0),
			new IntVector(1, -1, 0),
			new IntVector(1, -1, -1)
		},
		new IntVector[4]
		{
			new IntVector(-1, -1, -1),
			new IntVector(-1, -1, 0),
			new IntVector(0, -1, 0),
			new IntVector(0, -1, -1)
		},
		new IntVector[4]
		{
			new IntVector(-1, -1, -1),
			new IntVector(-1, 0, -1),
			new IntVector(0, 0, -1),
			new IntVector(0, -1, -1)
		},
		new IntVector[4]
		{
			new IntVector(0, -1, -1),
			new IntVector(0, 0, -1),
			new IntVector(1, 0, -1),
			new IntVector(1, -1, -1)
		},
		new IntVector[4]
		{
			new IntVector(0, 0, -1),
			new IntVector(0, 1, -1),
			new IntVector(1, 1, -1),
			new IntVector(1, 0, -1)
		},
		new IntVector[4]
		{
			new IntVector(-1, 0, -1),
			new IntVector(-1, 1, -1),
			new IntVector(0, 1, -1),
			new IntVector(0, 0, -1)
		},
		new IntVector[4]
		{
			new IntVector(0, -1, 1),
			new IntVector(0, 0, 1),
			new IntVector(1, 0, 1),
			new IntVector(1, -1, 1)
		},
		new IntVector[4]
		{
			new IntVector(-1, -1, 1),
			new IntVector(-1, 0, 1),
			new IntVector(0, 0, 1),
			new IntVector(0, -1, 1)
		},
		new IntVector[4]
		{
			new IntVector(-1, 0, 1),
			new IntVector(-1, 1, 1),
			new IntVector(0, 1, 1),
			new IntVector(0, 0, 1)
		},
		new IntVector[4]
		{
			new IntVector(0, 0, 1),
			new IntVector(0, 1, 1),
			new IntVector(1, 1, 1),
			new IntVector(1, 0, 1)
		},
		new IntVector[4]
		{
			new IntVector(-1, -1, 0),
			new IntVector(-1, 0, 0),
			new IntVector(-1, 0, 1),
			new IntVector(-1, -1, 1)
		},
		new IntVector[4]
		{
			new IntVector(-1, -1, -1),
			new IntVector(-1, 0, -1),
			new IntVector(-1, 0, 0),
			new IntVector(-1, -1, 0)
		},
		new IntVector[4]
		{
			new IntVector(-1, 0, -1),
			new IntVector(-1, 1, -1),
			new IntVector(-1, 1, 0),
			new IntVector(-1, 0, 0)
		},
		new IntVector[4]
		{
			new IntVector(-1, 0, 0),
			new IntVector(-1, 1, 0),
			new IntVector(-1, 1, 1),
			new IntVector(-1, 0, 1)
		},
		new IntVector[4]
		{
			new IntVector(1, -1, -1),
			new IntVector(1, 0, -1),
			new IntVector(1, 0, 0),
			new IntVector(1, -1, 0)
		},
		new IntVector[4]
		{
			new IntVector(1, -1, 0),
			new IntVector(1, 0, 0),
			new IntVector(1, 0, 1),
			new IntVector(1, -1, 1)
		},
		new IntVector[4]
		{
			new IntVector(1, 0, 0),
			new IntVector(1, 1, 0),
			new IntVector(1, 1, 1),
			new IntVector(1, 0, 1)
		},
		new IntVector[4]
		{
			new IntVector(1, 0, -1),
			new IntVector(1, 1, -1),
			new IntVector(1, 1, 0),
			new IntVector(1, 0, 0)
		}
	};

	public static IntVector[][] LightTestOffsetsInside = new IntVector[24][]
	{
		new IntVector[4]
		{
			new IntVector(-1, 0, -1),
			new IntVector(-1, 0, 0),
			new IntVector(0, 0, 0),
			new IntVector(0, 0, -1)
		},
		new IntVector[4]
		{
			new IntVector(0, 0, -1),
			new IntVector(0, 0, 0),
			new IntVector(1, 0, 0),
			new IntVector(1, 0, -1)
		},
		new IntVector[4]
		{
			new IntVector(0, 0, 0),
			new IntVector(0, 0, 1),
			new IntVector(1, 0, 1),
			new IntVector(1, 0, 0)
		},
		new IntVector[4]
		{
			new IntVector(-1, 0, 0),
			new IntVector(-1, 0, 1),
			new IntVector(0, 0, 1),
			new IntVector(0, 0, 0)
		},
		new IntVector[4]
		{
			new IntVector(-1, 0, 0),
			new IntVector(-1, 0, 1),
			new IntVector(0, 0, 1),
			new IntVector(0, 0, 0)
		},
		new IntVector[4]
		{
			new IntVector(0, 0, 0),
			new IntVector(0, 0, 1),
			new IntVector(1, 0, 1),
			new IntVector(1, 0, -1)
		},
		new IntVector[4]
		{
			new IntVector(0, 0, -1),
			new IntVector(0, 0, 0),
			new IntVector(1, 0, 0),
			new IntVector(1, 0, -1)
		},
		new IntVector[4]
		{
			new IntVector(-1, 0, -1),
			new IntVector(-1, 0, 0),
			new IntVector(0, 0, 0),
			new IntVector(0, 0, -1)
		},
		new IntVector[4]
		{
			new IntVector(-1, -1, 0),
			new IntVector(-1, 0, 0),
			new IntVector(0, 0, 0),
			new IntVector(0, -1, 0)
		},
		new IntVector[4]
		{
			new IntVector(0, -1, 0),
			new IntVector(0, 0, 0),
			new IntVector(1, 0, 0),
			new IntVector(1, -1, 0)
		},
		new IntVector[4]
		{
			new IntVector(0, 0, 0),
			new IntVector(0, 1, 0),
			new IntVector(1, 1, 0),
			new IntVector(1, 0, 0)
		},
		new IntVector[4]
		{
			new IntVector(-1, 0, 0),
			new IntVector(-1, 1, 0),
			new IntVector(0, 1, 0),
			new IntVector(0, 0, 0)
		},
		new IntVector[4]
		{
			new IntVector(0, -1, 0),
			new IntVector(0, 0, 0),
			new IntVector(1, 0, 0),
			new IntVector(1, -1, 0)
		},
		new IntVector[4]
		{
			new IntVector(-1, -1, 0),
			new IntVector(-1, 0, 0),
			new IntVector(0, 0, 0),
			new IntVector(0, -1, 0)
		},
		new IntVector[4]
		{
			new IntVector(-1, 0, 0),
			new IntVector(-1, 1, 0),
			new IntVector(0, 1, 0),
			new IntVector(0, 0, 0)
		},
		new IntVector[4]
		{
			new IntVector(0, 0, 0),
			new IntVector(0, 1, 0),
			new IntVector(1, 1, 0),
			new IntVector(1, 0, 0)
		},
		new IntVector[4]
		{
			new IntVector(0, -1, 0),
			new IntVector(0, 0, 0),
			new IntVector(0, 0, 1),
			new IntVector(0, -1, 1)
		},
		new IntVector[4]
		{
			new IntVector(0, -1, -1),
			new IntVector(0, 0, -1),
			new IntVector(0, 0, 0),
			new IntVector(0, -1, 0)
		},
		new IntVector[4]
		{
			new IntVector(0, 0, -1),
			new IntVector(0, 1, -1),
			new IntVector(0, 1, 0),
			new IntVector(0, 0, 0)
		},
		new IntVector[4]
		{
			new IntVector(0, 0, 0),
			new IntVector(0, 1, 0),
			new IntVector(0, 1, 1),
			new IntVector(0, 0, 1)
		},
		new IntVector[4]
		{
			new IntVector(0, -1, -1),
			new IntVector(0, 0, -1),
			new IntVector(0, 0, 0),
			new IntVector(0, -1, 0)
		},
		new IntVector[4]
		{
			new IntVector(0, -1, 0),
			new IntVector(0, 0, 0),
			new IntVector(0, 0, 1),
			new IntVector(0, -1, 1)
		},
		new IntVector[4]
		{
			new IntVector(0, 0, 0),
			new IntVector(0, 1, 0),
			new IntVector(0, 1, 1),
			new IntVector(0, 0, 1)
		},
		new IntVector[4]
		{
			new IntVector(0, 0, -1),
			new IntVector(0, 1, -1),
			new IntVector(0, 1, 0),
			new IntVector(0, 0, 0)
		}
	};

	public static IntVector CubeConstraint => constaint;

	public static Vector3 CubeConstraintVector3
	{
		get
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3((float)constaint.x, (float)constaint.y, (float)constaint.z);
		}
	}

	public static void AddCubeMeshCubeLines(Mesh mesh, Vector3[] corners, float diagonalWidth)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 4; i++)
		{
			AddCubeLine(mesh, corners[i], corners[(i + 1) % 4], diagonalWidth);
		}
		for (int j = 4; j < 8; j++)
		{
			AddCubeLine(mesh, corners[j], corners[(j + 1) % 4 + 4], diagonalWidth);
		}
		for (int k = 0; k < 4; k++)
		{
			AddCubeLine(mesh, corners[k], corners[7 - k], diagonalWidth);
		}
	}

	public static void AddCubeLine(Mesh mesh, Vector3 p0, Vector3 p1, float diagonalWidth)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		float num = diagonalWidth / 2f;
		Vector3 val = p1 - p0;
		Vector3 normalized = val.normalized;
		Vector3 val2 = p0 - normalized * num;
		Vector3 val3 = p1 + normalized * num;
		Vector3 val4 = normalized;
		val4.y = 0f;
		float num2 = 0f - Vector3.Angle(normalized, val4);
		if (normalized.y < 0f)
		{
			num2 = 0f - num2;
		}
		float num3 = MathFunctions.SignedAngle(Vector3.forward, val4, Vector3.up) * 57.29578f;
		Quaternion val5 = Quaternion.Euler(num2, 0f, 0f);
		Quaternion val6 = Quaternion.Euler(0f, num3, 0f);
		Quaternion val7 = val6 * val5;
		Vector3[] array = new Vector3[4]
		{
			(val7 * Vector3.down + val7 * Vector3.left) * num,
			(val7 * Vector3.down + val7 * Vector3.right) * num,
			(val7 * Vector3.up + val7 * Vector3.right) * num,
			(val7 * Vector3.up + val7 * Vector3.left) * num
		};
		Vector3[] array2 = new Vector3[8];
		for (int i = 0; i < 4; i++)
		{
			ref Vector3 reference = ref array2[i];
			reference = array[i] + val2;
		}
		for (int j = 0; j < 4; j++)
		{
			ref Vector3 reference2 = ref array2[j + 4];
			reference2 = array[3 - j] + val3;
		}
		AddCubeMesh(mesh, array2, insideOut: false);
	}

	public static void AddCubeMesh(Mesh mesh, Vector3[] corners, bool insideOut)
	{
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		List<int> list = new List<int>(mesh.triangles);
		List<Vector2> list2 = new List<Vector2>(mesh.uv);
		List<Vector3> list3 = new List<Vector3>(mesh.vertices);
		int count = list3.Count;
		list3.AddRange(GetVertices(corners));
		for (int i = 0; i < 6; i++)
		{
			if (!insideOut)
			{
				list.Add(i * 4 + count);
				list.Add(i * 4 + 3 + count);
				list.Add(i * 4 + 2 + count);
				list.Add(i * 4 + 2 + count);
				list.Add(i * 4 + 1 + count);
				list.Add(i * 4 + count);
			}
			else
			{
				list.Add(i * 4 + 2 + count);
				list.Add(i * 4 + 3 + count);
				list.Add(i * 4 + count);
				list.Add(i * 4 + count);
				list.Add(i * 4 + 1 + count);
				list.Add(i * 4 + 2 + count);
			}
			list2.Add(new Vector2(0f, 0f));
			list2.Add(new Vector2(1f, 0f));
			list2.Add(new Vector2(1f, 1f));
			list2.Add(new Vector2(0f, 1f));
		}
		mesh.vertices = list3.ToArray();
		mesh.uv = list2.ToArray();
		mesh.triangles = list.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}

	public static Vector3[] GetCorners()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3[8]
		{
			new Vector3(-0.5f, 0.5f, -0.5f),
			new Vector3(0.5f, 0.5f, -0.5f),
			new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(-0.5f, 0.5f, 0.5f),
			new Vector3(-0.5f, -0.5f, 0.5f),
			new Vector3(0.5f, -0.5f, 0.5f),
			new Vector3(0.5f, -0.5f, -0.5f),
			new Vector3(-0.5f, -0.5f, -0.5f)
		};
	}

	public static Vector3[] GetCorners(Bounds bounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return GetCorners(bounds.min, bounds.max);
	}

	public static Vector3[] GetCorners(Vector3 min, Vector3 max)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3[8]
		{
			new Vector3(min.x, max.y, min.z),
			new Vector3(max.x, max.y, min.z),
			new Vector3(max.x, max.y, max.z),
			new Vector3(min.x, max.y, max.z),
			new Vector3(min.x, min.y, max.z),
			new Vector3(max.x, min.y, max.z),
			new Vector3(max.x, min.y, min.z),
			new Vector3(min.x, min.y, min.z)
		};
	}

	public static Vector3[] GetVertices()
	{
		return GetVertices(GetCorners());
	}

	public static Vector3[] GetVertices(Vector3[] corners)
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

	public static Vector3 GetClosestGridPoint(Vector3 worldPosition, Quaternion rotation, float gridSize, Vector3 scale)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 val = Matrix4x4.TRS(rotation * scale * 0.5f, rotation, Vector3.one * gridSize);
		Matrix4x4 inverse = val.inverse;
		Vector3 vector = inverse.MultiplyPoint(worldPosition);
		vector = MathFunctions.RoundVector(vector, 0);
		return val.MultiplyPoint(vector);
	}

	public static IntVector WorldToLocal(GameObject gameObject, Vector3 point, bool floor = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Vector3 vector = gameObject.transform.InverseTransformPoint(point);
		vector = ((!floor) ? MathFunctions.RoundVector(vector, 0) : MathFunctions.FloorVector(vector));
		return new IntVector((short)vector.x, (short)vector.y, (short)vector.z);
	}

	public static Vector3 WorldPosToValidGridPos(GameObject gameObject, Vector3 worldPos, int cubeSegments)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (cubeSegments < 0)
		{
			Debug.LogError((object)"CubeSegments is at least 1");
		}
		float num = (float)Math.Round(1f / (float)cubeSegments, 2);
		Vector3 val = gameObject.transform.InverseTransformPoint(worldPos);
		Vector3 val2 = new Vector3(val.x, val.y, val.z);
		val = MathFunctions.FloorVector(val) - Vector3.one * 0.5f;
		for (int i = 0; i < 3; i++)
		{
			float num2 = val2[i] - val[i];
			int num3 = Mathf.FloorToInt(num2 / num);
			val[i] += (float)num3 * num;
		}
		return gameObject.transform.TransformPoint(val);
	}

	public static Vector3 LocalToWorld(GameObject gameObject, IntVector iVector)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		return gameObject.transform.TransformPoint(new Vector3((float)iVector.x, (float)iVector.y, (float)iVector.z));
	}

	public static Dictionary<IntVector, Cube> CreateFromBytePackage(BytePacker bp)
	{
		Dictionary<IntVector, Cube> dictionary = new Dictionary<IntVector, Cube>();
		int num = bp.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			IntVector key = new IntVector(bp.ReadInt16(), bp.ReadInt16(), bp.ReadInt16());
			dictionary[key] = new Cube(bp, bp.ReadByte());
		}
		return dictionary;
	}

	public static Bounds? GetAxisAlignedBoundsRecursively(Transform transform)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected Obj, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		MeshRenderer component = ((Component)transform).GetComponent<MeshRenderer>();
		Bounds? result = null;
		if ((Object)(object)component != (Object)null)
		{
			result = ((Renderer)component).bounds;
		}
		foreach (Transform item in transform)
		{
			Transform transform2 = item;
			Bounds? axisAlignedBoundsRecursively = GetAxisAlignedBoundsRecursively(transform2);
			if (!result.HasValue)
			{
				result = axisAlignedBoundsRecursively;
			}
			else if (axisAlignedBoundsRecursively.HasValue)
			{
				Bounds value = default;
				Bounds value2 = result.Value;
				value.min = value2.min;
				Bounds value3 = result.Value;
				value.max = value3.max;
				Bounds value4 = result.Value;
				Vector3 min = value4.min;
				Bounds value5 = axisAlignedBoundsRecursively.Value;
				value.min = MathFunctions.GetMinVector(min, value5.min);
				Bounds value6 = result.Value;
				Vector3 max = value6.max;
				Bounds value7 = axisAlignedBoundsRecursively.Value;
				value.max = MathFunctions.GetMaxVector(max, value7.max);
				result = value;
			}
		}
		return result;
	}

	private static Vector3[] GetTriangleVertices(int triangleIndex, GameObject gameObject)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = new Vector3[3];
		MeshFilter component = gameObject.GetComponent<MeshFilter>();
		ref Vector3 reference = ref array[0];
		reference = component.sharedMesh.vertices[component.sharedMesh.triangles[triangleIndex * 3]];
		ref Vector3 reference2 = ref array[1];
		reference2 = component.sharedMesh.vertices[component.sharedMesh.triangles[triangleIndex * 3 + 1]];
		ref Vector3 reference3 = ref array[2];
		reference3 = component.sharedMesh.vertices[component.sharedMesh.triangles[triangleIndex * 3 + 2]];
		return array;
	}

	public static float ScaleFactor(GameObject gameObject)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		for (int i = 0; i < 3; i++)
		{
			float num2 = num;
			Vector3 localScale = gameObject.transform.localScale;
			num = num2 + localScale[i];
		}
		return num / 3f;
	}

	private static float ScaleFactor(GameObject gameObject, Face face)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		switch (face)
		{
		case Face.Front:
		case Face.Back:
			return gameObject.transform.localScale.z;
		case Face.Top:
		case Face.Bottom:
			return gameObject.transform.localScale.y;
		case Face.Left:
		case Face.Right:
			return gameObject.transform.localScale.x;
		default:
			Debug.LogError((object)"No face");
			Debug.Break();
			return 0f;
		}
	}

	public static CubeOutOfBoundState MoveEdge(MVCubeModelBase cmb, CubePickingInfo info, Vector3 mousePositionDelta, ref float delta, ref float deltaAccum, float mouseSensitivity, ref bool edgeMoved, bool edgeIndex0, bool edgeIndex1)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		if (cmb.GetCube(info.iLocalPos) == null)
		{
			return CubeOutOfBoundState.WithinBounds;
		}
		CubeOutOfBoundState outOfBoundState = CubeOutOfBoundState.NoChange;
		float num = ScaleFactor(cmb.GameObject, info.pickedFace) * 0.25f;
		float num2 = (cmb.Scale.x + cmb.Scale.y + cmb.Scale.z) / 3f;
		float num3 = num / num2;
		Vector3 val = Vector4.op_Implicit(cmb.Transform.localToWorldMatrix * Vector4.op_Implicit(Cube.GetFaceAxis(info.pickedFace)));
		Vector3 val2 = info.point + val * deltaAccum;
		Debug.DrawLine(val2, val2 + val, Color.magenta);
		Debug.DrawLine(val2, val2 + Vector3.up, Color.magenta);
		Vector3 val3 = Camera.main.WorldToScreenPoint(val2 + val) - Camera.main.WorldToScreenPoint(val2);
		if (val3.magnitude > 0f)
		{
			float num4 = Vector3.Dot(val3.normalized, mousePositionDelta) / val3.magnitude;
			delta += num4 * 1.3f;
		}
		if (Mathf.Abs(delta) >= num3)
		{
			float num5 = delta % num3;
			delta -= num5;
			deltaAccum += delta;
			if (info.pickedEdge != Edge.None)
			{
				if (edgeIndex0 || edgeIndex1)
				{
					float value = delta;
					Vector4 val4 = cmb.Transform.worldToLocalMatrix * Vector4.op_Implicit(val.normalized);
					Cube.MoveVertex(info, value, Vector4.op_Implicit(val4.normalized), edgeIndex0, edgeIndex1, ref outOfBoundState);
				}
				else
				{
					float value2 = delta;
					Vector4 val5 = cmb.Transform.worldToLocalMatrix * Vector4.op_Implicit(val.normalized);
					Cube.MoveEdge(info, value2, Vector4.op_Implicit(val5.normalized), ref outOfBoundState);
				}
			}
			else
			{
				float delta2 = delta;
				Vector4 val6 = cmb.Transform.worldToLocalMatrix * Vector4.op_Implicit(val.normalized);
				Cube.MoveFace(info, delta2, Vector4.op_Implicit(val6.normalized), ref outOfBoundState);
			}
			delta = 0f;
			edgeMoved = true;
		}
		else
		{
			edgeMoved = false;
		}
		return outOfBoundState;
	}

	private static void GetVertices(CubePickingInfo info, GameObject gameObject)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (info.pickedEdge == Edge.None)
		{
			return;
		}
		Vector3[] edgeVerticesWorld = Cube.GetEdgeVerticesWorld(gameObject, info.cube, info.pickedFace, info.pickedEdge, info.iLocalPos);
		Vector3[] edge = Cube.GetEdge(info.cube, info.pickedFace, info.pickedEdge);
		Vector3 val = edge[0] - edge[1];
		float magnitude = val.magnitude;
		float num = 0.15f * ScaleFactor(gameObject) * magnitude;
		for (int i = 0; i < edgeVerticesWorld.Length; i++)
		{
			Vector3 val2 = edgeVerticesWorld[i] - info.point;
			if (val2.magnitude < num)
			{
				if (i == 0)
				{
					info.pickedEdgeIndex0 = true;
				}
				if (i == 1)
				{
					info.pickedEdgeIndex1 = true;
				}
			}
		}
	}

	public static bool GetPickingInfo(MVCubeModelBase cr, ref CubePickingInfo info)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		VoxelHit voxelHit = default;
		if (CollisionDetection.MVHit(ray, cr, out voxelHit))
		{
			info.cube = Cube.Clone(voxelHit.cube);
			info.iLocalPos = voxelHit.cubePos;
			info.pickedFace = voxelHit.face;
			info.point = voxelHit.point;
			info.normal = voxelHit.normal;
			info.pickedEdge = Cube.GetEdge(cr.GameObject, info.cube, info.pickedFace, voxelHit.point, info.iLocalPos);
			GetVertices(info, cr.GameObject);
			return true;
		}
		return false;
	}

	public static Bounds? GetAxisAlignedBoundsRecursively(List<MVWorldObjectClient> wos)
	{
		List<Transform> list = new List<Transform>();
		foreach (MVWorldObjectClient wo in wos)
		{
			list.Add(wo.Transform);
		}
		return GetAxisAlignedBoundsRecursively(list);
	}

	public static Bounds? GetAxisAlignedBoundsRecursively(List<Transform> transforms)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		Bounds? result = null;
		foreach (Transform transform in transforms)
		{
			Bounds? axisAlignedBoundsRecursively = GetAxisAlignedBoundsRecursively(transform);
			if (!result.HasValue)
			{
				result = axisAlignedBoundsRecursively;
			}
			else if (axisAlignedBoundsRecursively.HasValue)
			{
				Bounds value = default;
				Bounds value2 = result.Value;
				value.min = value2.min;
				Bounds value3 = result.Value;
				value.max = value3.max;
				Bounds value4 = result.Value;
				Vector3 min = value4.min;
				Bounds value5 = axisAlignedBoundsRecursively.Value;
				value.min = MathFunctions.GetMinVector(min, value5.min);
				Bounds value6 = result.Value;
				Vector3 max = value6.max;
				Bounds value7 = axisAlignedBoundsRecursively.Value;
				value.max = MathFunctions.GetMaxVector(max, value7.max);
				result = value;
			}
		}
		return result;
	}

	public static Vector3 GetWorldCenter(List<Transform> transforms)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Bounds? axisAlignedBoundsRecursively = GetAxisAlignedBoundsRecursively(transforms);
		if (axisAlignedBoundsRecursively.HasValue)
		{
			Bounds value = axisAlignedBoundsRecursively.Value;
			return value.center;
		}
		return Vector3.zero;
	}

	public static Vector3 GetWorldCenter(Transform transform)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Bounds? axisAlignedBoundsRecursively = GetAxisAlignedBoundsRecursively(transform);
		if (axisAlignedBoundsRecursively.HasValue)
		{
			Bounds value = axisAlignedBoundsRecursively.Value;
			return value.center;
		}
		return Vector3.zero;
	}

	public static void SetLayerRecursively(Transform t, bool select)
	{
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Expected Obj, but got Unknown
		if (select)
		{
			if (((Component)t).gameObject.layer == LayerMask.NameToLayer("Default"))
			{
				((Component)t).gameObject.layer = LayerMask.NameToLayer("CamRotateTarget");
			}
			else if (((Component)t).gameObject.layer == LayerMask.NameToLayer("Logic"))
			{
				((Component)t).gameObject.layer = LayerMask.NameToLayer("LogicSelected");
			}
			else if (((Component)t).gameObject.layer == LayerMask.NameToLayer("Player"))
			{
				((Component)t).gameObject.layer = LayerMask.NameToLayer("PlayerSelected");
			}
		}
		else if (((Component)t).gameObject.layer == LayerMask.NameToLayer("CamRotateTarget"))
		{
			((Component)t).gameObject.layer = LayerMask.NameToLayer("Default");
		}
		else if (((Component)t).gameObject.layer == LayerMask.NameToLayer("LogicSelected"))
		{
			((Component)t).gameObject.layer = LayerMask.NameToLayer("Logic");
		}
		else if (((Component)t).gameObject.layer == LayerMask.NameToLayer("PlayerSelected"))
		{
			((Component)t).gameObject.layer = LayerMask.NameToLayer("Player");
		}
		foreach (Transform item in t)
		{
			Transform t2 = item;
			SetLayerRecursively(t2, select);
		}
	}

	public static IntVector CubePosToChunk(IntVector cubePos, int chunkSize)
	{
		int num = chunkSize / 2;
		IntVector result = new IntVector(cubePos.x, cubePos.y, cubePos.z);
		result.x = (short)Mathf.FloorToInt(((float)result.x + (float)num) / (float)chunkSize);
		result.y = (short)Mathf.FloorToInt(((float)result.y + (float)num) / (float)chunkSize);
		result.z = (short)Mathf.FloorToInt(((float)result.z + (float)num) / (float)chunkSize);
		return result;
	}
}
