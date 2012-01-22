using UnityEngine;

internal class LineGizmo : IGizmo
{
	public Vector3 from;

	public Vector3 to;

	public Color color;

	public LineGizmo(Vector3 from, Vector3 to, Color color)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		this.from = from;
		this.to = to;
		this.color = color;
	}

	public void OnDrawGizmos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = color;
		Gizmos.DrawLine(from, to);
	}
}
