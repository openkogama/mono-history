using UnityEngine;

public class ClosestPointCapsule : ClosestPointBase
{
	[SerializeField]
	private CapsuleCollider collider;

	private Transform Transform => collider.gameObject.transform;

	private Vector3 Position => Transform.position;

	private Vector3 Scale => Transform.lossyScale;

	public ClosestPointCapsule(CapsuleCollider c)
	{
		collider = c;
	}

	public override Vector3 GetClosestPoint(Vector3 from)
	{
		Vector3 vector = Position + collider.center.Multiply(Scale);
		float num = (collider.height / 2f - collider.radius) * Scale.y;
		float y = vector.y;
		y += num;
		float y2 = vector.y;
		y2 -= num;
		float y3 = Mathf.Clamp(from.y, y2, y);
		Vector3 position = Position;
		position.y = y3;
		Vector3 vector2 = from - position;
		return position + vector2.normalized * collider.radius * Scale.y;
	}

	private void OnValidate()
	{
		if (collider == null)
		{
			collider = GetComponent<CapsuleCollider>();
		}
	}
}
