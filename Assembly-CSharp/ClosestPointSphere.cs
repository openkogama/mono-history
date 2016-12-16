using UnityEngine;

public class ClosestPointSphere : ClosestPointBase
{
	[SerializeField]
	private SphereCollider collider;

	private Transform Transform => collider.gameObject.transform;

	private Vector3 Position => Transform.position;

	private float ScaledRadius => collider.radius * Transform.lossyScale.y;

	public ClosestPointSphere(SphereCollider c)
	{
		collider = c;
	}

	public override Vector3 GetClosestPoint(Vector3 from)
	{
		Vector3 vector = from - Position + Position + collider.center.Multiply(Transform.lossyScale);
		return Position + vector.normalized * ScaledRadius;
	}

	private void OnValidate()
	{
		Debug.LogWarning("ClosestPointSphere is not properly tested.");
		if (collider == null)
		{
			collider = GetComponent<SphereCollider>();
		}
	}
}
