using UnityEngine;

internal class SphereGizmo : IGizmo
{
	public Vector3 c;

	public float r;

	public Color color;

	public SphereGizmo(Vector3 center, float radius, Color color)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		c = center;
		r = radius;
		this.color = color;
	}

	public void OnDrawGizmos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = color;
		Gizmos.DrawWireSphere(c, r);
	}
}
