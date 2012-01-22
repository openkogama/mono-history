using System.Collections.Generic;
using UnityEngine;

internal class FaceCursor : IModelCursor
{
	private Material materialCorner;

	private Material materialEdge;

	private Material materialNone;

	private GameObject gameObject;

	public GameObject GameObject => gameObject;

	public FaceCursor()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected Obj, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected Obj, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected Obj, but got Unknown
		gameObject = new GameObject("Cursor");
		gameObject.layer = LayerMask.NameToLayer("UIItems");
		MeshRenderer val = gameObject.AddComponent<MeshRenderer>();
		gameObject.AddComponent<MeshFilter>();
		materialEdge = (Material)Resources.Load("Materials/CursorMaterial");
		materialCorner = (Material)Resources.Load("Materials/CursorMaterialCorner");
		materialNone = (Material)Resources.Load("Materials/CursorMaterialNone");
		((Renderer)val).material = materialEdge;
	}

	public void Destroy()
	{
		Object.Destroy((Object)(object)gameObject);
	}

	public void UpdateCursor(CubePickingInfo info, GameObject cubeGameObject)
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
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
		mesh.uv = SetUVs(info.pickedEdge, info.pickedEdgeIndex1);
		mesh.triangles = list.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		if (info.pickedEdge == Edge.None)
		{
			((Renderer)gameObject.GetComponent<MeshRenderer>()).material = materialNone;
		}
		else if (info.pickedEdgeIndex0 || info.pickedEdgeIndex1)
		{
			((Renderer)gameObject.GetComponent<MeshRenderer>()).material = materialCorner;
		}
		else
		{
			((Renderer)gameObject.GetComponent<MeshRenderer>()).material = materialEdge;
		}
		Vector3 val = gameObject.transform.TransformPoint(mesh.vertices[0]);
		Transform transform = gameObject.transform;
		transform.position += faceVerticesWorld[0] - val + info.normal * 0.001f;
	}

	private Vector2[] SetUVs(Edge edge, bool mirror)
	{
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		List<Vector2> list = new List<Vector2>();
		switch (edge)
		{
		case Edge.Front:
			list.Add(new Vector2((float)(mirror ? 1 : 0), 0f));
			list.Add(new Vector2((float)((!mirror) ? 1 : 0), 0f));
			list.Add(new Vector2((float)((!mirror) ? 1 : 0), 1f));
			list.Add(new Vector2((float)(mirror ? 1 : 0), 1f));
			break;
		case Edge.Back:
			list.Add(new Vector2((float)((!mirror) ? 1 : 0), 1f));
			list.Add(new Vector2((float)(mirror ? 1 : 0), 1f));
			list.Add(new Vector2((float)(mirror ? 1 : 0), 0f));
			list.Add(new Vector2((float)((!mirror) ? 1 : 0), 0f));
			break;
		case Edge.Left:
			list.Add(new Vector2((float)((!mirror) ? 1 : 0), 0f));
			list.Add(new Vector2((float)((!mirror) ? 1 : 0), 1f));
			list.Add(new Vector2((float)(mirror ? 1 : 0), 1f));
			list.Add(new Vector2((float)(mirror ? 1 : 0), 0f));
			break;
		case Edge.Right:
			list.Add(new Vector2((float)(mirror ? 1 : 0), 1f));
			list.Add(new Vector2((float)(mirror ? 1 : 0), 0f));
			list.Add(new Vector2((float)((!mirror) ? 1 : 0), 0f));
			list.Add(new Vector2((float)((!mirror) ? 1 : 0), 1f));
			break;
		case Edge.None:
			list.Add(new Vector2(0f, 0f));
			list.Add(new Vector2(1f, 0f));
			list.Add(new Vector2(1f, 1f));
			list.Add(new Vector2(0f, 1f));
			break;
		}
		return list.ToArray();
	}
}
