using UnityEngine;

public class ClosestPointCapsule : ClosestPointBase
{
	[SerializeField]
	private CapsuleCollider capsule;

	private Transform Transform => capsule.gameObject.transform;

	private Vector3 Position => Transform.position;

	private Vector3 Scale => Transform.lossyScale;

	public ClosestPointCapsule(CapsuleCollider c)
	{
		capsule = c;
	}

	public override Vector3 GetClosestPoint(Vector3 from)
	{
		Vector3 vector = Position + capsule.center.Multiply(Scale);
		float num = (capsule.height / 2f - capsule.radius) * Scale.y;
		float y = vector.y;
		y += num;
		float y2 = vector.y;
		y2 -= num;
		float y3 = Mathf.Clamp(from.y, y2, y);
		Vector3 position = Position;
		position.y = y3;
		Vector3 vector2 = from - position;
		return position + vector2.normalized * capsule.radius * Scale.y;
	}

	private void OnValidate()
	{
		if (capsule == null)
		{
			capsule = GetComponent<CapsuleCollider>();
		}
	}
}
