using UnityEngine;

public class ClosestPointSphere : ClosestPointBase
{
	[SerializeField]
	private Vector3 offset;

	[SerializeField]
	private float radius = 1f;

	protected void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position + offset, radius);
	}

	public override Vector3 GetClosestPoint(Vector3 spectator)
	{
		Vector3 vector = offset.Multiply(transform.lossyScale);
		float num = radius * transform.lossyScale.x;
		Vector3 vector2 = transform.position + vector;
		Vector3 vector3 = (spectator - transform.position).normalized * num;
		return vector2 + vector3;
	}
}
