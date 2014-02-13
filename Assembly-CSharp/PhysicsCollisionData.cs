using UnityEngine;

public struct PhysicsCollisionData
{
	public Vector3 point;

	public Transform transform;

	public bool isInsideCollider;

	public float distance;

	public Vector3 normal;

	public Collider collider;

	public PhysicsCollisionData(RaycastHit hit)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		point = hit.point;
		transform = ((Component)hit.collider).transform;
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
