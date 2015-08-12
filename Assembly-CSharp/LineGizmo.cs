using UnityEngine;

internal class LineGizmo : IGizmo
{
	public Vector3 from;

	public Vector3 to;

	public Color color;

	public LineGizmo(Vector3 from, Vector3 to, Color color)
	{
		this.from = from;
		this.to = to;
		this.color = color;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = color;
		Gizmos.DrawLine(from, to);
	}
}
