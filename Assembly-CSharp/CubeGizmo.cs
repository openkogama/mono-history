using UnityEngine;

internal class CubeGizmo : IGizmo
{
	public Bounds b;

	public Color color;

	public CubeGizmo(Bounds bounds, Color color)
	{
		b = bounds;
		this.color = color;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = color;
		Gizmos.DrawWireCube(b.center, b.size);
	}
}
