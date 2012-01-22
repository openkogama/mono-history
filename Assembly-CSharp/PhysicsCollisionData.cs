using UnityEngine;

public struct PhysicsCollisionData
{
	public Vector3 point;

	public Transform transform;

	public bool isInsideCollider;

	public float distance;

	public Vector3 normal;

	public Collider collider;

	public PhysicsCollisionData(Vector3 point, Transform transform, bool isInsideCollider, float distance, Vector3 normal, Collider collider)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		this.point = point;
		this.transform = transform;
		this.isInsideCollider = isInsideCollider;
		this.distance = distance;
		this.normal = normal;
		this.collider = collider;
	}

	public PhysicsCollisionData(RaycastHit hit)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		point = hit.point;
		transform = hit.transform;
		isInsideCollider = false;
		distance = hit.distance;
		normal = hit.normal;
		collider = hit.collider;
	}

	public PhysicsCollisionData(Collider collider, Vector3 origin)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		point = origin;
		transform = ((Component)collider).transform;
		isInsideCollider = true;
		distance = 0f;
		normal = Vector3.zero;
		this.collider = collider;
	}
}
