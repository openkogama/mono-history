using UnityEngine;

internal class MeshData
{
	public void SetToMesh(ref Mesh mesh, ref Material material)
	{
		mesh.Clear();
		mesh.vertices = MeshDataPool.GetVertices();
		mesh.colors = MeshDataPool.GetColors();
		mesh.uv = MeshDataPool.GetUvs();
		mesh.triangles = MeshDataPool.GetIndices();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		material = MVGameControllerBase.MaterialLoader.CubeModelMaterial;
	}
}
