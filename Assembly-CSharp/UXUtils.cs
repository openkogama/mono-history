using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class UXUtils
{
	private static MVGUIRoot guiRoot = null;

	private static Dictionary<Type, Component> cachedReferences = new Dictionary<Type, Component>();

	private static MVGUIRoot GuiRoot
	{
		get
		{
			if ((Object)(object)guiRoot == (Object)null)
			{
				guiRoot = FindObjectOfType<MVGUIRoot>();
			}
			if ((Object)(object)guiRoot == (Object)null)
			{
				Debug.LogError((object)"Failed to find MVGUIRoot");
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

	public static GameObject FindChild(GameObject root, string name)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected Obj, but got Unknown
		Component[] componentsInChildren = root.GetComponentsInChildren(typeof(Transform), true);
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Transform val = (Transform)array[i];
			if (((Object)((Component)val).gameObject).name == name)
			{
				return ((Component)val).gameObject;
			}
		}
		return null;
	}

	public static void AddSubTree(Transform transform)
	{
		transform.parent = ((Component)GuiRoot).transform;
	}

	public static T FindGUIObjectOfType<T>() where T : class
	{
		if (!cachedReferences.ContainsKey(typeof(T)))
		{
			Component componentInChildren = ((Component)GuiRoot).gameObject.GetComponentInChildren(typeof(T));
			if (!((Object)(object)componentInChildren != (Object)null))
			{
				return (T)null;
			}
			cachedReferences.Add(typeof(T), componentInChildren);
		}
		return cachedReferences[typeof(T)] as T;
	}

	public static T AddComponentIfNotExists<T>(GameObject gameObject) where T : Component
	{
		T val = gameObject.GetComponent<T>();
		if ((Object)(object)val == (Object)null)
		{
			val = gameObject.AddComponent<T>();
		}
		return val;
	}

	public static Mesh BuildPlaneMesh(float width = 1f, float height = 1f, string name = "")
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return BuildPlaneMesh(new Vector3(width / 2f, height / 2f, 0f), width, height, name);
	}

	public static Mesh BuildPlaneMesh(Vector3 alignment, float width = 1f, float height = 1f, string name = "")
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected Obj, but got Unknown
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
		Mesh val = new Mesh();
		val.vertices = array;
		val.triangles = array2;
		val.uv = array3;
		if (name != string.Empty)
		{
			((Object)val).name = name;
		}
		val.RecalculateBounds();
		val.RecalculateNormals();
		return val;
	}

	public static Mesh Build9PatchPlaneMesh(Vector3 alignment, Texture texture, float width = 1f, float height = 1f, string name = "")
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Expected Obj, but got Unknown
		Vector2 val = new Vector2((float)texture.width, (float)texture.height);
		Object val2 = Resources.Load("GUIFormatting/" + ((Object)texture).name);
		TextAsset val3 = (TextAsset)(object)((val2 is TextAsset) ? val2 : null);
		if ((Object)(object)val3 == (Object)null)
		{
			Debug.LogError((object)("Found no text file containing Insert info for texture '" + ((Object)texture).name + "'. Can't create 9patch mesh"));
			return null;
		}
		int[] insertsFromCSVString = GetInsertsFromCSVString(val3.text);
		if (insertsFromCSVString == null)
		{
			Debug.LogError((object)"Failed parsing inserts from Insert info file. Can't create 9patch mesh");
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
		float num6 = (float)num / num5;
		float num7 = (float)num2 / num5;
		float num8 = (float)num3 / num5;
		float num9 = (float)num4 / num5;
		ref Vector3 reference = ref array[0];
		reference = new Vector3(0f, 0f, 0f);
		ref Vector3 reference2 = ref array[1];
		reference2 = new Vector3(0f, num8, 0f);
		ref Vector3 reference3 = ref array[2];
		reference3 = new Vector3(num6, num8, 0f);
		ref Vector3 reference4 = ref array[3];
		reference4 = new Vector3(num6, 0f, 0f);
		ref Vector3 reference5 = ref array[4];
		reference5 = new Vector3(0f, height - num9, 0f);
		ref Vector3 reference6 = ref array[5];
		reference6 = new Vector3(0f, height, 0f);
		ref Vector3 reference7 = ref array[6];
		reference7 = new Vector3(num6, height, 0f);
		ref Vector3 reference8 = ref array[7];
		reference8 = new Vector3(num6, height - num9, 0f);
		ref Vector3 reference9 = ref array[8];
		reference9 = new Vector3(width - num7, height - num9, 0f);
		ref Vector3 reference10 = ref array[9];
		reference10 = new Vector3(width - num7, height, 0f);
		ref Vector3 reference11 = ref array[10];
		reference11 = new Vector3(width, height, 0f);
		ref Vector3 reference12 = ref array[11];
		reference12 = new Vector3(width, height - num9, 0f);
		ref Vector3 reference13 = ref array[12];
		reference13 = new Vector3(width - num7, 0f, 0f);
		ref Vector3 reference14 = ref array[13];
		reference14 = new Vector3(width - num7, num8, 0f);
		ref Vector3 reference15 = ref array[14];
		reference15 = new Vector3(width, num8, 0f);
		ref Vector3 reference16 = ref array[15];
		reference16 = new Vector3(width, 0f, 0f);
		for (int i = 0; i < array.Length; i++)
		{
			ref Vector3 reference17 = ref array[i];
			reference17 = array[i] - alignment;
		}
		float num10 = (float)num / val.x;
		float num11 = 1f - (float)num2 / val.x;
		float num12 = (float)num3 / val.y;
		float num13 = 1f - (float)num4 / val.y;
		ref Vector2 reference18 = ref array3[0];
		reference18 = new Vector2(0f, 0f);
		ref Vector2 reference19 = ref array3[1];
		reference19 = new Vector2(0f, num12);
		ref Vector2 reference20 = ref array3[2];
		reference20 = new Vector2(num10, num12);
		ref Vector2 reference21 = ref array3[3];
		reference21 = new Vector2(num10, 0f);
		ref Vector2 reference22 = ref array3[4];
		reference22 = new Vector2(0f, num13);
		ref Vector2 reference23 = ref array3[5];
		reference23 = new Vector2(0f, 1f);
		ref Vector2 reference24 = ref array3[6];
		reference24 = new Vector2(num10, 1f);
		ref Vector2 reference25 = ref array3[7];
		reference25 = new Vector2(num10, num13);
		ref Vector2 reference26 = ref array3[8];
		reference26 = new Vector2(num11, num13);
		ref Vector2 reference27 = ref array3[9];
		reference27 = new Vector2(num11, 1f);
		ref Vector2 reference28 = ref array3[10];
		reference28 = new Vector2(1f, 1f);
		ref Vector2 reference29 = ref array3[11];
		reference29 = new Vector2(1f, num13);
		ref Vector2 reference30 = ref array3[12];
		reference30 = new Vector2(num11, 0f);
		ref Vector2 reference31 = ref array3[13];
		reference31 = new Vector2(num11, num12);
		ref Vector2 reference32 = ref array3[14];
		reference32 = new Vector2(1f, num12);
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
		Mesh val4 = new Mesh();
		val4.vertices = array;
		val4.triangles = array2;
		val4.uv = array3;
		if (name != string.Empty)
		{
			((Object)val4).name = name;
		}
		val4.RecalculateBounds();
		val4.RecalculateNormals();
		return val4;
	}

	private static int[] GetInsertsFromCSVString(string inserts)
	{
		string[] array = inserts.Split(new char[1] { ',' });
		if (array.Length < 4)
		{
			Debug.LogError((object)"Not enough inserts in file");
			return null;
		}
		int[] array2 = new int[4];
		int result = 0;
		for (int i = 0; i < array2.Length; i++)
		{
			if (!int.TryParse(array[i], out result))
			{
				Debug.LogError((object)"Failed parsing inserts.");
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
