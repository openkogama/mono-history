using System.Collections.Generic;
using UnityEngine;

internal class FaceCursor : ICursor
{
	private Material materialCorner;

	private Material materialEdge;

	private Material materialNone;

	private GameObject gameObject;

	public GameObject GameObject => gameObject;

	public FaceCursor()
	{
		gameObject = new GameObject("Cursor");
		gameObject.layer = LayerMask.NameToLayer("UIItems");
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		gameObject.AddComponent<MeshFilter>();
		materialEdge = (Material)Resources.Load("Materials/CursorMaterial");
		materialCorner = (Material)Resources.Load("Materials/CursorMaterialCorner");
		materialNone = (Material)Resources.Load("Materials/CursorMaterialNone");
		meshRenderer.material = materialEdge;
	}

	public void Remove()
	{
		Object.Destroy(gameObject);
	}

	public void UpdateCursor(CubePickingInfo info, GameObject cubeGameObject)
	{
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
			gameObject.GetComponent<MeshRenderer>().material = materialNone;
		}
		else if (info.pickedEdgeIndex0 || info.pickedEdgeIndex1)
		{
			gameObject.GetComponent<MeshRenderer>().material = materialCorner;
		}
		else
		{
			gameObject.GetComponent<MeshRenderer>().material = materialEdge;
		}
		Vector3 vector = gameObject.transform.TransformPoint(mesh.vertices[0]);
		gameObject.transform.position += faceVerticesWorld[0] - vector + info.normal * 0.001f;
	}

	private Vector2[] SetUVs(Edge edge, bool mirror)
	{
		List<Vector2> list = new List<Vector2>();
		switch (edge)
		{
		case Edge.Front:
			list.Add(new Vector2(mirror ? 1 : 0, 0f));
			list.Add(new Vector2((!mirror) ? 1 : 0, 0f));
			list.Add(new Vector2((!mirror) ? 1 : 0, 1f));
			list.Add(new Vector2(mirror ? 1 : 0, 1f));
			break;
		case Edge.Back:
			list.Add(new Vector2((!mirror) ? 1 : 0, 1f));
			list.Add(new Vector2(mirror ? 1 : 0, 1f));
			list.Add(new Vector2(mirror ? 1 : 0, 0f));
			list.Add(new Vector2((!mirror) ? 1 : 0, 0f));
			break;
		case Edge.Left:
			list.Add(new Vector2((!mirror) ? 1 : 0, 0f));
			list.Add(new Vector2((!mirror) ? 1 : 0, 1f));
			list.Add(new Vector2(mirror ? 1 : 0, 1f));
			list.Add(new Vector2(mirror ? 1 : 0, 0f));
			break;
		case Edge.Right:
			list.Add(new Vector2(mirror ? 1 : 0, 1f));
			list.Add(new Vector2(mirror ? 1 : 0, 0f));
			list.Add(new Vector2((!mirror) ? 1 : 0, 0f));
			list.Add(new Vector2((!mirror) ? 1 : 0, 1f));
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
