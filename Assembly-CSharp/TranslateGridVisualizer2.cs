using System.Collections.Generic;
using UnityEngine;

public class TranslateGridVisualizer2
{
	private GameObject gameObject;

	public GameObject GameObject => gameObject;

	public TranslateGridVisualizer2(MVWorldObjectClient wo)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected Obj, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		gameObject = new GameObject(((Object)wo.GameObject).name + " translateGridVisualizer");
		gameObject.transform.position = wo.GameObject.transform.position;
		gameObject.transform.localScale = wo.GameObject.transform.localScale * 1.01f;
		gameObject.transform.rotation = wo.GameObject.transform.rotation;
		CreateInsideOutCube(wo.GetBoundsCornersLocal());
	}

	public void Destroy()
	{
		Object.Destroy((Object)(object)gameObject);
	}

	private void CreateInsideOutCube(Vector3[] corners)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected Obj, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		MeshFilter val = gameObject.AddComponent<MeshFilter>();
		MeshRenderer val2 = gameObject.AddComponent<MeshRenderer>();
		((Renderer)val2).material = (Material)Resources.Load("Materials/ModelConstraints");
		Mesh mesh = val.mesh;
		List<int> list = new List<int>();
		List<Vector2> list2 = new List<Vector2>();
		mesh.vertices = GetVertices(corners);
		for (int i = 0; i < 6; i++)
		{
			list.Add(i * 4);
			list.Add(i * 4 + 3);
			list.Add(i * 4 + 2);
			list.Add(i * 4 + 2);
			list.Add(i * 4 + 1);
			list.Add(i * 4);
			list2.Add(new Vector2(0f, 0f));
			list2.Add(new Vector2(1f, 0f));
			list2.Add(new Vector2(1f, 1f));
			list2.Add(new Vector2(0f, 1f));
		}
		mesh.uv = list2.ToArray();
		mesh.triangles = list.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}

	private Vector3[] GetVertices(Vector3[] corners)
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
}
