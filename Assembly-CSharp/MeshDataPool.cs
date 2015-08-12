using System;
using UnityEngine;

public static class MeshDataPool
{
	private static int maxVertices = 786432;

	private static int vertexPos = 0;

	private static readonly Vector3[] vertices = new Vector3[maxVertices];

	private static int uvPos = 0;

	private static readonly Vector2[] uvs = new Vector2[maxVertices];

	private static int colorPos = 0;

	private static readonly Color[] colors = new Color[maxVertices];

	public static void AddVertex(Vector3 vertex)
	{
		vertices[vertexPos] = vertex;
		vertexPos++;
	}

	public static Vector3[] GetVertices()
	{
		Vector3[] array = new Vector3[vertexPos];
		Array.Copy(vertices, 0, array, 0, vertexPos);
		return array;
	}

	public static void AddUv(Vector2 uv)
	{
		uvs[uvPos] = uv;
		uvPos++;
	}

	public static Vector2[] GetUvs()
	{
		Vector2[] array = new Vector2[uvPos];
		Array.Copy(uvs, 0, array, 0, uvPos);
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
		colors[colorPos] = color;
		colorPos++;
	}

	public static Color[] GetColors()
	{
		Color[] array = new Color[colorPos];
		Array.Copy(colors, 0, array, 0, colorPos);
		return array;
	}

	public static void Reset()
	{
		vertexPos = (uvPos = (colorPos = 0));
	}
}
