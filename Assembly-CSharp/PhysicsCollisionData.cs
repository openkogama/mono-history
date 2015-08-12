using UnityEngine;

public class PhysicsCollisionData
{
	public Vector3 point;

	public Transform transform;

	public bool isInsideCollider;

	public float distance;

	public Vector3 normal;

	public Collider collider;

	public void Set(RaycastHit hit)
	{
		point = hit.point;
		transform = hit.collider.transform;
		isInsideCollider = false;
		distance = hit.distance;
		normal = hit.normal;
		collider = hit.collider;
	}

	public void Set(Collider collider, Vector3 origin)
	{
		point = origin;
		transform = collider.transform;
		isInsideCollider = true;
		distance = 0f;
		normal = Vector3.zero;
		this.collider = collider;
	}

	public void Clear()
	{
		transform = null;
		collider = null;
	}
}
