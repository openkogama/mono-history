using System;
using System.Collections.Generic;
using UnityEngine;

public static class UXUtils
{
	public static void VisitSubtree(Transform root, Action<GameObject> visitor)
	{
		for (int i = 0; i < root.childCount; i++)
		{
			VisitSubtree(root.GetChild(i), visitor);
		}
		visitor(((Component)root).gameObject);
	}

	public static Vector3 Multiply(this Vector3 a, Vector3 b)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	public static Component FindComponentInParents(Type type, Transform parent)
	{
		return (!((Object)(object)parent != (Object)null)) ? null : (((Component)parent).GetComponent(type) ?? FindComponentInParents(type, parent.parent));
	}

	public static int WrapIndex(int index, int length)
	{
		if (index >= length)
		{
			return index % length;
		}
		if (index < 0)
		{
			return length - -index % length;
		}
		return index;
	}

	public static T FindObjectOfType<T>() where T : class
	{
		Object[] array = Object.FindObjectsOfType(typeof(T));
		if (array.Length == 0)
		{
			throw new Exception($"Instance of type {typeof(T).Name} could not be found.");
		}
		if (array.Length > 1)
		{
			throw new Exception($"Found more than one instance of type {typeof(T).Name}.");
		}
		return array[0] as T;
	}

	public static T FindFirstWithComponent<T>(ICollection<GameObject> gameObjects) where T : Component
	{
		foreach (GameObject gameObject in gameObjects)
		{
			T component = gameObject.GetComponent<T>();
			if ((Object)(object)component != (Object)null)
			{
				return component;
			}
		}
		return (T)(object)null;
	}

	public static Mesh BuildPlaneMesh()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected Obj, but got Unknown
		Vector3[] vertices = new Vector3[4]
		{
			new Vector3(-0.5f, -0.5f, 0f),
			new Vector3(-0.5f, 0.5f, 0f),
			new Vector3(0.5f, 0.5f, 0f),
			new Vector3(0.5f, -0.5f, 0f)
		};
		Vector2[] uv = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f)
		};
		int[] triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
		Mesh val = new Mesh();
		val.vertices = vertices;
		val.triangles = triangles;
		val.uv = uv;
		val.RecalculateBounds();
		val.RecalculateNormals();
		return val;
	}
}
