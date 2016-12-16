using UnityEngine;

public class ClosestPointBox : ClosestPointBase
{
	private struct Plane
	{
		public Vector3 norm;

		public float constant;
	}

	[SerializeField]
	private BoxCollider collider;

	private Transform Transform => gameObject.transform;

	private Plane GetClosestPlane(Vector3 from)
	{
		Vector3[] array = new Vector3[3] { Transform.right, Transform.up, Transform.forward };
		Vector3[] array2 = new Vector3[3]
		{
			-Transform.right,
			-Transform.up,
			-Transform.forward
		};
		Vector3 vector = collider.size.Multiply(Transform.lossyScale);
		float[] array3 = new float[3]
		{
			vector.x / 2f,
			vector.y / 2f,
			vector.z / 2f
		};
		Vector3 normalized = (from - Transform.position).normalized;
		float num = 0f;
		Plane result = default;
		float constant = 0f;
		for (byte b = 0; b < 3; b++)
		{
			Vector3 vector2 = array[b];
			float num2 = Vector3.Dot(vector2, normalized);
			if (num2 < 0f)
			{
				vector2 = array2[b];
				num2 = Vector3.Dot(array2[b], normalized);
			}
			if (num2 > num)
			{
				num = num2;
				result.norm = vector2;
				constant = array3[b];
			}
		}
		result.constant = constant;
		return result;
	}

	public override Vector3 GetClosestPoint(Vector3 from)
	{
		Plane closestPlane = GetClosestPlane(from);
		float num = Vector3.Dot(closestPlane.norm, from) - closestPlane.constant;
		return from - num * closestPlane.norm;
	}

	private void OnValidate()
	{
		Debug.LogError("ClosestPointBox is tested and contains errors.");
		if (collider == null)
		{
			collider = GetComponent<BoxCollider>();
		}
	}
}
