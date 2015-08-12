using UnityEngine;

internal class SphereGizmo : IGizmo
{
	public Vector3 c;

	public float r;

	public Color color;

	public SphereGizmo(Vector3 center, float radius, Color color)
	{
		c = center;
		r = radius;
		this.color = color;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = color;
		Gizmos.DrawWireSphere(c, r);
	}
}
