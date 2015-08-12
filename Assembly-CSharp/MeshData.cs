using System.Collections.Generic;
using UnityEngine;

internal class MeshData
{
	public List<Material> materials = new List<Material>();

	public List<List<int>> subMeshTriangles = new List<List<int>>();

	public void SetToMesh(ref Mesh mesh, ref Material[] materials)
	{
		mesh.Clear();
		mesh.vertices = MeshDataPool.GetVertices();
		mesh.colors = MeshDataPool.GetColors();
		mesh.uv = MeshDataPool.GetUvs();
		mesh.subMeshCount = this.materials.Count;
		for (int i = 0; i < this.materials.Count; i++)
		{
			mesh.SetTriangles(subMeshTriangles[i].ToArray(), i);
		}
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		materials = this.materials.ToArray();
		this.materials.Clear();
		subMeshTriangles.Clear();
	}
}
