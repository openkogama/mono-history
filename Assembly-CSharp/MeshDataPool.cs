using System;
using UnityEngine;

public class MeshDataPool
{
	private const int maxVertices = 786432;

	private const int maxIndices = 1179648;

	private int vertexPos;

	private readonly Vector3[] vertices = new Vector3[786432];

	private int uvPos;

	private readonly Vector2[] uvs = new Vector2[786432];

	private int colorPos;

	private readonly Color[] colors = new Color[786432];

	private int indicesPos;

	private readonly int[] indices = new int[1179648];

	private static MeshDataPool instance;

	private static int VertexPos
	{
		get
		{
			return instance.vertexPos;
		}
		set
		{
			instance.vertexPos = value;
		}
	}

	private static Vector3[] Vertices => instance.vertices;

	private static int UvPos
	{
		get
		{
			return instance.uvPos;
		}
		set
		{
			instance.uvPos = value;
		}
	}

	private static Vector2[] Uvs => instance.uvs;

	private static int ColorPos
	{
		get
		{
			return instance.colorPos;
		}
		set
		{
			instance.colorPos = value;
		}
	}

	private static Color[] Colors => instance.colors;

	private static int IndicesPos
	{
		get
		{
			return instance.indicesPos;
		}
		set
		{
			instance.indicesPos = value;
		}
	}

	private static int[] Indices => instance.indices;

	public static void Create()
	{
		instance = new MeshDataPool();
	}

	public static void Destroy()
	{
		instance = null;
	}

	public static void AddVertex(Vector3 vertex)
	{
		Vertices[VertexPos] = vertex;
		VertexPos++;
	}

	public static Vector3[] GetVertices()
	{
		Vector3[] array = new Vector3[VertexPos];
		Array.Copy(Vertices, 0, array, 0, VertexPos);
		return array;
	}

	public static void AddUv(Vector2 uv)
	{
		Uvs[UvPos] = uv;
		UvPos++;
	}

	public static Vector2[] GetUvs()
	{
		Vector2[] array = new Vector2[UvPos];
		Array.Copy(Uvs, 0, array, 0, UvPos);
		return array;
	}

	public static void AddUvRange(Vector2[] uvRange)
	{
		for (int i = 0; i < uvRange.Length; i++)
		{
			AddUv(uvRange[i]);
		}
	}

	public static void AddColor(Color color)
	{
		Colors[ColorPos] = color;
		ColorPos++;
	}

	public static Color[] GetColors()
	{
		Color[] array = new Color[ColorPos];
		Array.Copy(Colors, 0, array, 0, ColorPos);
		return array;
	}

	public static void AddIndex(int index)
	{
		Indices[IndicesPos] = index;
		IndicesPos++;
	}

	public static int[] GetIndices()
	{
		int[] array = new int[IndicesPos];
		Array.Copy(Indices, array, IndicesPos);
		return array;
	}

	public static void Reset()
	{
		int num = (IndicesPos = 0);
		num = (ColorPos = num);
		num = (UvPos = num);
		VertexPos = num;
	}
}
