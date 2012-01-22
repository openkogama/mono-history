using System.Collections.Generic;
using UnityEngine;

public class IndentArea : IModelCursor
{
	private Material materialEdge;

	private Material materialNone;

	private GameObject gameObject;

	private float size;

	public GameObject GameObject => gameObject;

	public float Size
	{
		set
		{
			size = value;
		}
	}

	public IndentArea()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected Obj, but got Unknown
		gameObject = new GameObject("IndentArea");
		gameObject.layer = LayerMask.NameToLayer("UIItems");
		MeshRenderer val = gameObject.AddComponent<MeshRenderer>();
		gameObject.AddComponent<MeshFilter>();
		materialNone = (Material)Resources.Load("Materials/IndentMaterial");
		((Renderer)val).material = materialNone;
	}

	public void Destroy()
	{
		Object.Destroy((Object)(object)gameObject);
	}

	public void UpdateIndentArea(CubePickingInfo info, GameObject cubeGameObject)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] faceVerticesWorld = Cube.GetFaceVerticesWorld(cubeGameObject, info.cube, info.pickedFace, info.iLocalPos);
		List<int> list = new List<int>();
		Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		list.Add(0);
		list.Add(3);
		list.Add(2);
		list.Add(2);
		list.Add(1);
		list.Add(0);
		mesh.vertices = faceVerticesWorld;
		mesh.uv = SetUVs();
		mesh.triangles = list.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		Vector3 val = (faceVerticesWorld[2] - faceVerticesWorld[0]) / 2f + faceVerticesWorld[0];
		gameObject.transform.localScale = Vector3.one * size;
		Vector3 val2 = gameObject.transform.TransformPoint(mesh.vertices[0]);
		Vector3 val3 = gameObject.transform.TransformPoint(mesh.vertices[2]);
		Vector3 val4 = (val3 - val2) / 2f + val2;
		Transform transform = gameObject.transform;
		transform.position += val - val4 + info.normal * 0.0015f;
	}

	public bool IsColliding()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
		Ray val = Camera.main.ScreenPointToRay(Input.mousePosition);
		Vector3[] array = new Vector3[4];
		for (int i = 0; i < mesh.vertices.Length; i++)
		{
			ref Vector3 reference = ref array[i];
			reference = gameObject.transform.TransformPoint(mesh.vertices[i]);
		}
		Vector3 origin = val.origin;
		Vector3 p = val.origin + val.direction * 5000f;
		Vector3 p2 = default;
		bool flag = MathFunctions.LineFacet(origin, p, array[0], array[3], array[2], ref p2);
		bool flag2 = MathFunctions.LineFacet(origin, p, array[2], array[1], array[0], ref p2);
		if (flag || flag2)
		{
			return true;
		}
		return false;
	}

	private Vector2[] SetUVs()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		List<Vector2> list = new List<Vector2>();
		list.Add(new Vector2(0f, 0f));
		list.Add(new Vector2(1f, 0f));
		list.Add(new Vector2(1f, 1f));
		list.Add(new Vector2(0f, 1f));
		return list.ToArray();
	}
}
