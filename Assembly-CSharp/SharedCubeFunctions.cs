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

	public static Vector3 CubeConstraintVector3 => new Vector3(constaint.x, constaint.y, constaint.z);

	public static void AddCubeMeshCubeLines(Mesh mesh, Vector3[] corners, float diagonalWidth)
	{
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
		float num = diagonalWidth / 2f;
		Vector3 normalized = (p1 - p0).normalized;
		Vector3 vector = p0 - normalized * num;
		Vector3 vector2 = p1 + normalized * num;
		Vector3 vector3 = normalized;
		vector3.y = 0f;
		float num2 = 0f - Vector3.Angle(normalized, vector3);
		if (normalized.y < 0f)
		{
			num2 = 0f - num2;
		}
		float y = MathFunctions.SignedAngle(Vector3.forward, vector3, Vector3.up) * 57.29578f;
		Quaternion quaternion = Quaternion.Euler(num2, 0f, 0f);
		Quaternion quaternion2 = Quaternion.Euler(0f, y, 0f);
		Quaternion quaternion3 = quaternion2 * quaternion;
		Vector3[] array = new Vector3[4]
		{
			(quaternion3 * Vector3.down + quaternion3 * Vector3.left) * num,
			(quaternion3 * Vector3.down + quaternion3 * Vector3.right) * num,
			(quaternion3 * Vector3.up + quaternion3 * Vector3.right) * num,
			(quaternion3 * Vector3.up + quaternion3 * Vector3.left) * num
		};
		Vector3[] array2 = new Vector3[8];
		for (int i = 0; i < 4; i++)
		{
			ref Vector3 reference = ref array2[i];
			reference = array[i] + vector;
		}
		for (int j = 0; j < 4; j++)
		{
			ref Vector3 reference2 = ref array2[j + 4];
			reference2 = array[3 - j] + vector2;
		}
		AddCubeMesh(mesh, array2, insideOut: false);
	}

	public static void AddCubeMesh(Mesh mesh, Vector3[] corners, bool insideOut)
	{
		List<int> list = ((mesh.vertexCount == 0) ? new List<int>() : new List<int>(mesh.triangles));
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
		return GetCorners(bounds.min, bounds.max);
	}

	public static Vector3[] GetCorners(Vector3 min, Vector3 max)
	{
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
		Matrix4x4 matrix4x = Matrix4x4.TRS(rotation * scale * 0.5f, rotation, Vector3.one * gridSize);
		Vector3 vector = matrix4x.inverse.MultiplyPoint(worldPosition);
		vector = MathFunctions.RoundVector(vector, 0);
		return matrix4x.MultiplyPoint(vector);
	}

	public static IntVector WorldToLocal(GameObject gameObject, Vector3 point, bool floor = false)
	{
		Vector3 vector = gameObject.transform.InverseTransformPoint(point);
		vector = ((!floor) ? MathFunctions.RoundVector(vector, 0) : MathFunctions.FloorVector(vector));
		return new IntVector((short)vector.x, (short)vector.y, (short)vector.z);
	}

	public static Vector3 WorldPosToValidGridPos(GameObject gameObject, Vector3 worldPos, int cubeSegments)
	{
		if (cubeSegments < 0)
		{
			Debug.LogError("CubeSegments is at least 1");
		}
		float num = (float)Math.Round(1f / (float)cubeSegments, 2);
		Vector3 vector = gameObject.transform.InverseTransformPoint(worldPos);
		Vector3 vector2 = new Vector3(vector.x, vector.y, vector.z);
		vector = MathFunctions.FloorVector(vector) - Vector3.one * 0.5f;
		for (int i = 0; i < 3; i++)
		{
			float num2 = vector2[i] - vector[i];
			int num3 = Mathf.FloorToInt(num2 / num);
			vector[i] += (float)num3 * num;
		}
		return gameObject.transform.TransformPoint(vector);
	}

	public static Vector3 LocalToWorld(GameObject gameObject, IntVector iVector)
	{
		return gameObject.transform.TransformPoint(new Vector3(iVector.x, iVector.y, iVector.z));
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
		MeshRenderer component = transform.GetComponent<MeshRenderer>();
		Bounds? result = null;
		if (component != null && component.enabled)
		{
			result = component.bounds;
		}
		foreach (Transform item in transform)
		{
			Bounds? axisAlignedBoundsRecursively = GetAxisAlignedBoundsRecursively(item);
			if (!result.HasValue)
			{
				result = axisAlignedBoundsRecursively;
			}
			else if (axisAlignedBoundsRecursively.HasValue)
			{
				Bounds value = default;
				value.min = result.Value.min;
				value.max = result.Value.max;
				value.min = MathFunctions.GetMinVector(result.Value.min, axisAlignedBoundsRecursively.Value.min);
				value.max = MathFunctions.GetMaxVector(result.Value.max, axisAlignedBoundsRecursively.Value.max);
				result = value;
			}
		}
		return result;
	}

	private static Vector3[] GetTriangleVertices(int triangleIndex, GameObject gameObject)
	{
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
		float num = 0f;
		for (int i = 0; i < 3; i++)
		{
			num += gameObject.transform.localScale[i];
		}
		return num / 3f;
	}

	public static float ScaleFactor(GameObject gameObject, Face face)
	{
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
			Debug.LogError("No face");
			Debug.Break();
			return 0f;
		}
	}

	public static CubeOutOfBoundState MoveEdge(MVCubeModelBase cmb, CubePickingInfo info, Vector3 mousePositionDelta, ref float delta, ref float deltaAccum, float mouseSensitivity, ref bool edgeMoved, bool edgeIndex0, bool edgeIndex1, ref EditCubeChange editCubeChange)
	{
		if (cmb.GetCube(info.iLocalPos) == null)
		{
			return CubeOutOfBoundState.WithinBounds;
		}
		CubeOutOfBoundState outOfBoundState = CubeOutOfBoundState.NoChange;
		float num = ScaleFactor(cmb.GameObject, info.pickedFace) * 0.25f;
		float num2 = (cmb.Scale.x + cmb.Scale.y + cmb.Scale.z) / 3f;
		float num3 = num / num2;
		Vector3 vector = cmb.Transform.localToWorldMatrix * Cube.GetFaceAxis(info.pickedFace);
		Vector3 vector2 = info.point + vector * deltaAccum;
		Debug.DrawLine(vector2, vector2 + vector, Color.magenta);
		Debug.DrawLine(vector2, vector2 + Vector3.up, Color.magenta);
		Vector3 vector3 = Camera.main.WorldToScreenPoint(vector2 + vector) - Camera.main.WorldToScreenPoint(vector2);
		if (vector3.magnitude > 0f)
		{
			float num4 = Vector3.Dot(vector3.normalized, mousePositionDelta) / vector3.magnitude;
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
					Cube.MoveVertex(info, delta, (cmb.Transform.worldToLocalMatrix * vector.normalized).normalized, edgeIndex0, edgeIndex1, ref outOfBoundState);
					editCubeChange = EditCubeChange.VertexMoved;
				}
				else
				{
					Cube.MoveEdge(info, delta, (cmb.Transform.worldToLocalMatrix * vector.normalized).normalized, ref outOfBoundState);
					editCubeChange = EditCubeChange.EdgeMoved;
				}
			}
			else
			{
				Cube.MoveFace(info, delta, (cmb.Transform.worldToLocalMatrix * vector.normalized).normalized, ref outOfBoundState);
				editCubeChange = EditCubeChange.FaceMoved;
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

	public static void GetVertices(CubePickingInfo info, GameObject gameObject)
	{
		if (info.pickedEdge == Edge.None)
		{
			return;
		}
		Vector3[] edgeVerticesWorld = Cube.GetEdgeVerticesWorld(gameObject, info.cube, info.pickedFace, info.pickedEdge, info.iLocalPos);
		Vector3[] edge = Cube.GetEdge(info.cube, info.pickedFace, info.pickedEdge);
		float magnitude = (edge[0] - edge[1]).magnitude;
		float num = 0.15f * ScaleFactor(gameObject) * magnitude;
		for (int i = 0; i < edgeVerticesWorld.Length; i++)
		{
			if ((edgeVerticesWorld[i] - info.point).magnitude < num)
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
				value.min = result.Value.min;
				value.max = result.Value.max;
				value.min = MathFunctions.GetMinVector(result.Value.min, axisAlignedBoundsRecursively.Value.min);
				value.max = MathFunctions.GetMaxVector(result.Value.max, axisAlignedBoundsRecursively.Value.max);
				result = value;
			}
		}
		return result;
	}

	public static Vector3 GetWorldCenter(List<Transform> transforms)
	{
		Bounds? axisAlignedBoundsRecursively = GetAxisAlignedBoundsRecursively(transforms);
		if (axisAlignedBoundsRecursively.HasValue)
		{
			return axisAlignedBoundsRecursively.Value.center;
		}
		return Vector3.zero;
	}

	public static Vector3 GetWorldCenter(Transform transform)
	{
		Bounds? axisAlignedBoundsRecursively = GetAxisAlignedBoundsRecursively(transform);
		if (axisAlignedBoundsRecursively.HasValue)
		{
			return axisAlignedBoundsRecursively.Value.center;
		}
		return Vector3.zero;
	}

	public static void SetLayerRecursively(Transform t, bool select)
	{
		if (select)
		{
			if (t.gameObject.layer == LayerMask.NameToLayer("Default"))
			{
				t.gameObject.layer = LayerMask.NameToLayer("CamRotateTarget");
			}
			else if (t.gameObject.layer == LayerMask.NameToLayer("Logic"))
			{
				t.gameObject.layer = LayerMask.NameToLayer("LogicSelected");
			}
			else if (t.gameObject.layer == LayerMask.NameToLayer("Player"))
			{
				t.gameObject.layer = LayerMask.NameToLayer("PlayerSelected");
			}
		}
		else if (t.gameObject.layer == LayerMask.NameToLayer("CamRotateTarget"))
		{
			t.gameObject.layer = LayerMask.NameToLayer("Default");
		}
		else if (t.gameObject.layer == LayerMask.NameToLayer("LogicSelected"))
		{
			t.gameObject.layer = LayerMask.NameToLayer("Logic");
		}
		else if (t.gameObject.layer == LayerMask.NameToLayer("PlayerSelected"))
		{
			t.gameObject.layer = LayerMask.NameToLayer("Player");
		}
		foreach (Transform item in t)
		{
			SetLayerRecursively(item, select);
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
