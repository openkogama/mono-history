using UnityEngine;

public struct SharedMeshData(Mesh mesh)
{
	public Mesh mesh = mesh;

	public Material material = null;

	public void SetToMesh(ref Mesh mesh, ref Material material)
	{
		mesh = this.mesh;
		material = this.material;
	}
}
