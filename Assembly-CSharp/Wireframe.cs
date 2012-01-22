using System.Collections.Generic;
using UnityEngine;

public class Wireframe : MonoBehaviour
{
	private Color lineColor = Color.black;

	private Color backgroundColor = Color.white;

	private Vector3[] lines;

	private List<Vector3> linesArray;

	private Material lineMaterial;

	private MeshRenderer meshRenderer;

	public Wireframe()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected Obj, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		meshRenderer = ((Component)this).GetComponent<MeshRenderer>();
		((Renderer)meshRenderer).enabled = false;
		lineMaterial = new Material("Shader \"Lines/Colored Blended\" { SubShader { Pass { Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Front Fog { Mode Off } } } }");
		((Object)lineMaterial).hideFlags = (HideFlags)13;
		((Object)lineMaterial.shader).hideFlags = (HideFlags)13;
		linesArray = new List<Vector3>();
		MeshFilter component = ((Component)this).GetComponent<MeshFilter>();
		Mesh mesh = component.mesh;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		for (int i = 0; i < triangles.Length / 3; i++)
		{
			linesArray.Add(vertices[triangles[i * 3]]);
			linesArray.Add(vertices[triangles[i * 3 + 1]]);
			linesArray.Add(vertices[triangles[i * 3 + 2]]);
		}
		lines = linesArray.ToArray();
	}

	private void OnRenderObject()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		((Renderer)meshRenderer).material.color = backgroundColor;
		lineMaterial.SetPass(0);
		GL.PushMatrix();
		GL.MultMatrix(((Component)this).transform.localToWorldMatrix);
		GL.Begin(1);
		GL.Color(lineColor);
		for (int i = 0; i < lines.Length / 3; i++)
		{
			GL.Vertex(lines[i * 3]);
			GL.Vertex(lines[i * 3 + 1]);
			GL.Vertex(lines[i * 3 + 1]);
			GL.Vertex(lines[i * 3 + 2]);
			GL.Vertex(lines[i * 3 + 2]);
			GL.Vertex(lines[i * 3]);
		}
		GL.End();
		GL.PopMatrix();
	}
}
