using UnityEngine;

public class ClosestPointPoint : ClosestPointBase
{
	[SerializeField]
	private new Transform transform;

	public void Init(Transform t)
	{
		transform = t;
	}

	public override Vector3 GetClosestPoint(Vector3 from)
	{
		return GetClosestPoint();
	}

	public Vector3 GetClosestPoint()
	{
		return transform.position;
	}

	private void OnValidate()
	{
		if (transform == null)
		{
			transform = GetComponent<Transform>();
		}
	}
}
