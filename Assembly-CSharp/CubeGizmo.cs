using UnityEngine;

internal class CubeGizmo : IGizmo
{
	public Bounds b;

	public Color color;

	public CubeGizmo(Bounds bounds, Color color)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		b = bounds;
		this.color = color;
	}

	public void OnDrawGizmos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = color;
		Gizmos.DrawWireCube(b.center, b.size);
	}
}
