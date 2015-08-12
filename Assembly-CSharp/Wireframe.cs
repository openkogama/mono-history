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

	private void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.enabled = false;
		lineMaterial = new Material("Shader \"Lines/Colored Blended\" { SubShader { Pass { Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Front Fog { Mode Off } } } }");
		lineMaterial.hideFlags = HideFlags.HideAndDontSave;
		lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		linesArray = new List<Vector3>();
		MeshFilter component = GetComponent<MeshFilter>();
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
		meshRenderer.material.color = backgroundColor;
		lineMaterial.SetPass(0);
		GL.PushMatrix();
		GL.MultMatrix(transform.localToWorldMatrix);
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
