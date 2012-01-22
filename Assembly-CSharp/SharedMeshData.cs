using UnityEngine;

public struct SharedMeshData(Mesh mesh)
{
	public Mesh mesh = mesh;

	public Material[] materials = new Material[0];

	public void SetToMesh(ref Mesh mesh, ref Material[] materials)
	{
		mesh = this.mesh;
		materials = this.materials;
	}
}
