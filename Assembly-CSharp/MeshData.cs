using System.Collections.Generic;
using UnityEngine;

internal class MeshData
{
	public List<Vector3> vertices = new List<Vector3>();

	public List<Vector2> uv = new List<Vector2>();

	public List<Color> colors = new List<Color>();

	public List<Material> materials = new List<Material>();

	public List<List<int>> subMeshTriangles = new List<List<int>>();

	public void SetToMesh(ref Mesh mesh, ref Material[] materials)
	{
		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.colors = colors.ToArray();
		mesh.uv = uv.ToArray();
		mesh.subMeshCount = this.materials.Count;
		for (int i = 0; i < this.materials.Count; i++)
		{
			mesh.SetTriangles(subMeshTriangles[i].ToArray(), i);
		}
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		materials = this.materials.ToArray();
		vertices.Clear();
		uv.Clear();
		colors.Clear();
		this.materials.Clear();
		subMeshTriangles.Clear();
	}
}
