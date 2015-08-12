using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class UXUtils
{
	private static MVGUIRoot guiRoot = null;

	private static Dictionary<Type, Component> cachedReferences = new Dictionary<Type, Component>();

	public static UXDialogFactory UXDialogFactory => FindObjectOfType<UXDialogFactory>();

	public static UXScreen UXScreen
	{
		get
		{
			try
			{
				return FindObjectOfType<UXScreen>();
			}
			catch (Exception)
			{
				return null;
			}
		}
	}

	public static UXInputDispatcher UXInputDispatcher => FindGUIObjectOfType<UXInputDispatcher>();

	public static UXCamera UXCamera => FindGUIObjectOfType<UXCamera>();

	private static MVGUIRoot GuiRoot
	{
		get
		{
			if (guiRoot == null)
			{
				guiRoot = FindObjectOfType<MVGUIRoot>();
			}
			if (guiRoot == null)
			{
				Debug.LogError("Failed to find MVGUIRoot");
			}
			return guiRoot;
		}
	}

	public static void VisitSubtree(Transform root, Action<GameObject> visitor)
	{
		for (int i = 0; i < root.childCount; i++)
		{
			VisitSubtree(root.GetChild(i), visitor);
		}
		visitor(root.gameObject);
	}

	public static Vector3 Multiply(this Vector3 a, Vector3 b)
	{
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	public static Component FindComponentInParents(Type type, Transform parent)
	{
		return (!(parent != null)) ? null : (parent.GetComponent(type) ?? FindComponentInParents(type, parent.parent));
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
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(T));
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
			if (component != null)
			{
				return component;
			}
		}
		return (T)null;
	}

	public static GameObject FindChild(GameObject root, string name)
	{
		Component[] componentsInChildren = root.GetComponentsInChildren(typeof(Transform), includeInactive: true);
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = (Transform)array[i];
			if (transform.gameObject.name == name)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	public static void AddSubTree(Transform transform)
	{
		transform.parent = GuiRoot.transform;
	}

	public static T FindGUIObjectOfType<T>() where T : Component
	{
		if (!cachedReferences.ContainsKey(typeof(T)))
		{
			Component[] componentsInChildren = GuiRoot.gameObject.GetComponentsInChildren<T>(includeInactive: true);
			if (componentsInChildren.Length == 0)
			{
				throw new Exception("Failed to get component");
			}
			if (componentsInChildren.Length > 1)
			{
				string text = string.Empty;
				Component[] array = componentsInChildren;
				foreach (Component component in array)
				{
					text = text + component.gameObject.name + " ";
				}
				throw new Exception("Duplicate components detected " + text);
			}
			cachedReferences.Add(typeof(T), componentsInChildren[0]);
		}
		return cachedReferences[typeof(T)] as T;
	}

	public static T AddComponentIfNotExists<T>(GameObject gameObject) where T : Component
	{
		T val = gameObject.GetComponent<T>();
		if (val == null)
		{
			val = gameObject.AddComponent<T>();
		}
		return val;
	}

	public static Mesh BuildPlaneMesh(Mesh mesh, float width = 1f, float height = 1f, string name = "")
	{
		return BuildPlaneMesh(mesh, new Vector3(width / 2f, height / 2f, 0f), width, height, name);
	}

	public static Mesh BuildPlaneMesh(Mesh mesh, Vector3 alignment, float width = 1f, float height = 1f, string name = "")
	{
		mesh.Clear();
		Vector3[] array = new Vector3[4];
		int[] array2 = new int[6];
		Vector2[] array3 = new Vector2[array.Length];
		ref Vector3 reference = ref array[0];
		reference = new Vector3(0f, 0f, 0f) - alignment;
		ref Vector3 reference2 = ref array[1];
		reference2 = new Vector3(0f, height, 0f) - alignment;
		ref Vector3 reference3 = ref array[2];
		reference3 = new Vector3(width, height, 0f) - alignment;
		ref Vector3 reference4 = ref array[3];
		reference4 = new Vector3(width, 0f, 0f) - alignment;
		ref Vector2 reference5 = ref array3[0];
		reference5 = new Vector2(0f, 0f);
		ref Vector2 reference6 = ref array3[1];
		reference6 = new Vector2(0f, 1f);
		ref Vector2 reference7 = ref array3[2];
		reference7 = new Vector2(1f, 1f);
		ref Vector2 reference8 = ref array3[3];
		reference8 = new Vector2(1f, 0f);
		array2[0] = 0;
		array2[1] = 1;
		array2[2] = 2;
		array2[3] = 2;
		array2[4] = 3;
		array2[5] = 0;
		mesh.vertices = array;
		mesh.triangles = array2;
		mesh.uv = array3;
		if (name != string.Empty)
		{
			mesh.name = name;
		}
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		return mesh;
	}

	public static Mesh Build9PatchPlaneMesh(Mesh mesh, Vector3 alignment, Texture texture, float width = 1f, float height = 1f, string name = "")
	{
		mesh.Clear();
		Vector2 vector = new Vector2(texture.width, texture.height);
		TextAsset textAsset = Resources.Load("GUIFormatting/" + texture.name) as TextAsset;
		if (textAsset == null)
		{
			Debug.LogError("Found no text file containing Insert info for texture '" + texture.name + "'. Can't create 9patch mesh");
			return null;
		}
		int[] insertsFromCSVString = GetInsertsFromCSVString(textAsset.text);
		if (insertsFromCSVString == null)
		{
			Debug.LogError("Failed parsing inserts from Insert info file. Can't create 9patch mesh");
			return null;
		}
		int num = insertsFromCSVString[0];
		int num2 = insertsFromCSVString[1];
		int num3 = insertsFromCSVString[2];
		int num4 = insertsFromCSVString[3];
		Vector3[] array = new Vector3[16];
		int[] array2 = new int[54];
		Vector2[] array3 = new Vector2[array.Length];
		UXScreen uXScreen = FindGUIObjectOfType<UXScreen>();
		float num5 = uXScreen.screenUnit / uXScreen.worldUnit;
		float x = (float)num / num5;
		float num6 = (float)num2 / num5;
		float y = (float)num3 / num5;
		float num7 = (float)num4 / num5;
		ref Vector3 reference = ref array[0];
		reference = new Vector3(0f, 0f, 0f);
		ref Vector3 reference2 = ref array[1];
		reference2 = new Vector3(0f, y, 0f);
		ref Vector3 reference3 = ref array[2];
		reference3 = new Vector3(x, y, 0f);
		ref Vector3 reference4 = ref array[3];
		reference4 = new Vector3(x, 0f, 0f);
		ref Vector3 reference5 = ref array[4];
		reference5 = new Vector3(0f, height - num7, 0f);
		ref Vector3 reference6 = ref array[5];
		reference6 = new Vector3(0f, height, 0f);
		ref Vector3 reference7 = ref array[6];
		reference7 = new Vector3(x, height, 0f);
		ref Vector3 reference8 = ref array[7];
		reference8 = new Vector3(x, height - num7, 0f);
		ref Vector3 reference9 = ref array[8];
		reference9 = new Vector3(width - num6, height - num7, 0f);
		ref Vector3 reference10 = ref array[9];
		reference10 = new Vector3(width - num6, height, 0f);
		ref Vector3 reference11 = ref array[10];
		reference11 = new Vector3(width, height, 0f);
		ref Vector3 reference12 = ref array[11];
		reference12 = new Vector3(width, height - num7, 0f);
		ref Vector3 reference13 = ref array[12];
		reference13 = new Vector3(width - num6, 0f, 0f);
		ref Vector3 reference14 = ref array[13];
		reference14 = new Vector3(width - num6, y, 0f);
		ref Vector3 reference15 = ref array[14];
		reference15 = new Vector3(width, y, 0f);
		ref Vector3 reference16 = ref array[15];
		reference16 = new Vector3(width, 0f, 0f);
		for (int i = 0; i < array.Length; i++)
		{
			ref Vector3 reference17 = ref array[i];
			reference17 = array[i] - alignment;
		}
		float x2 = (float)num / vector.x;
		float x3 = 1f - (float)num2 / vector.x;
		float y2 = (float)num3 / vector.y;
		float y3 = 1f - (float)num4 / vector.y;
		ref Vector2 reference18 = ref array3[0];
		reference18 = new Vector2(0f, 0f);
		ref Vector2 reference19 = ref array3[1];
		reference19 = new Vector2(0f, y2);
		ref Vector2 reference20 = ref array3[2];
		reference20 = new Vector2(x2, y2);
		ref Vector2 reference21 = ref array3[3];
		reference21 = new Vector2(x2, 0f);
		ref Vector2 reference22 = ref array3[4];
		reference22 = new Vector2(0f, y3);
		ref Vector2 reference23 = ref array3[5];
		reference23 = new Vector2(0f, 1f);
		ref Vector2 reference24 = ref array3[6];
		reference24 = new Vector2(x2, 1f);
		ref Vector2 reference25 = ref array3[7];
		reference25 = new Vector2(x2, y3);
		ref Vector2 reference26 = ref array3[8];
		reference26 = new Vector2(x3, y3);
		ref Vector2 reference27 = ref array3[9];
		reference27 = new Vector2(x3, 1f);
		ref Vector2 reference28 = ref array3[10];
		reference28 = new Vector2(1f, 1f);
		ref Vector2 reference29 = ref array3[11];
		reference29 = new Vector2(1f, y3);
		ref Vector2 reference30 = ref array3[12];
		reference30 = new Vector2(x3, 0f);
		ref Vector2 reference31 = ref array3[13];
		reference31 = new Vector2(x3, y2);
		ref Vector2 reference32 = ref array3[14];
		reference32 = new Vector2(1f, y2);
		ref Vector2 reference33 = ref array3[15];
		reference33 = new Vector2(1f, 0f);
		array2 = new int[54]
		{
			0, 1, 2, 2, 3, 0, 1, 4, 7, 7,
			2, 1, 4, 5, 6, 6, 7, 4, 7, 6,
			9, 9, 8, 7, 2, 7, 8, 8, 13, 2,
			3, 2, 13, 13, 12, 3, 12, 13, 14, 14,
			15, 12, 13, 8, 11, 11, 14, 13, 8, 9,
			10, 10, 11, 8
		};
		mesh.vertices = array;
		mesh.triangles = array2;
		mesh.uv = array3;
		if (name != string.Empty)
		{
			mesh.name = name;
		}
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		return mesh;
	}

	private static int[] GetInsertsFromCSVString(string inserts)
	{
		string[] array = inserts.Split(',');
		if (array.Length < 4)
		{
			Debug.LogError("Not enough inserts in file");
			return null;
		}
		int[] array2 = new int[4];
		int result = 0;
		for (int i = 0; i < array2.Length; i++)
		{
			if (!int.TryParse(array[i], out result))
			{
				Debug.LogError("Failed parsing inserts.");
				return null;
			}
			array2[i] = result;
		}
		return array2;
	}

	public static string RepeatString(string s, int count)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < count; i++)
		{
			stringBuilder.Append(s);
		}
		return stringBuilder.ToString();
	}
}
