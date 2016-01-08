using System.Collections.Generic;
using UnityEngine;

public class IndentArea : ICursor
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
		gameObject = new GameObject("IndentArea");
		gameObject.layer = LayerMask.NameToLayer("UIItems");
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		gameObject.AddComponent<MeshFilter>();
		materialNone = PrefabPool.Instance.IndentMaterial;
		meshRenderer.material = materialNone;
	}

	public void Remove()
	{
		Object.Destroy(gameObject);
	}

	public void UpdateIndentArea(CubePickingInfo info, GameObject cubeGameObject)
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
		mesh.uv = SetUVs();
		mesh.triangles = list.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		Vector3 vector = (faceVerticesWorld[2] - faceVerticesWorld[0]) / 2f + faceVerticesWorld[0];
		gameObject.transform.localScale = Vector3.one * size;
		Vector3 vector2 = gameObject.transform.TransformPoint(mesh.vertices[0]);
		Vector3 vector3 = gameObject.transform.TransformPoint(mesh.vertices[2]);
		Vector3 vector4 = (vector3 - vector2) / 2f + vector2;
		gameObject.transform.position += vector - vector4 + info.normal * 0.0015f;
	}

	public bool IsColliding()
	{
		Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
		Ray ray = Camera.main.ScreenPointToRay(MVInputWrapper.GetPointerPosition());
		Vector3[] array = new Vector3[4];
		for (int i = 0; i < mesh.vertices.Length; i++)
		{
			ref Vector3 reference = ref array[i];
			reference = gameObject.transform.TransformPoint(mesh.vertices[i]);
		}
		Vector3 origin = ray.origin;
		Vector3 p = ray.origin + ray.direction * 5000f;
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
		List<Vector2> list = new List<Vector2>();
		list.Add(new Vector2(0f, 0f));
		list.Add(new Vector2(1f, 0f));
		list.Add(new Vector2(1f, 1f));
		list.Add(new Vector2(0f, 1f));
		return list.ToArray();
	}
}
