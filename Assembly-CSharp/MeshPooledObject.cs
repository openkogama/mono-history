using UnityEngine;

public struct MeshPooledObject
{
	private int id;

	private Mesh mesh;

	public int Id => id;

	public Mesh Mesh => mesh;

	public void Init(int id)
	{
		this.id = id;
		mesh = new Mesh();
	}
}
