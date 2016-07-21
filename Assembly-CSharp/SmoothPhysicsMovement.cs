using System.Collections.Generic;
using UnityEngine;

public class SmoothPhysicsMovement : MonoBehaviour
{
	private class Package
	{
		public readonly Vector3 position;

		public readonly Quaternion rotation;

		public readonly float time;

		public Package(Vector3 position, Quaternion rotation)
		{
			time = Time.fixedTime;
			this.position = position;
			this.rotation = rotation;
		}
	}

	private Queue<Package> packages = new Queue<Package>();

	private CullingSubscriberBase cullingSubscriberBase;

	private Package next;

	private Package current;

	private Transform targetTransform;

	public void Init(Transform targetTransform, CullingSubscriberBase cullingSubscriberBase)
	{
		this.targetTransform = targetTransform;
		this.cullingSubscriberBase = cullingSubscriberBase;
		if (cullingSubscriberBase == null)
		{
			Debug.LogWarning("Remember to add culling subscriber");
		}
	}

	public void SmoothMove()
	{
		float num = Time.time - Time.fixedDeltaTime;
		if (current == null && packages.Count > 0)
		{
			current = packages.Dequeue();
		}
		if (current != null && next == null && packages.Count > 0)
		{
			next = packages.Dequeue();
		}
		if (current != null && next != null)
		{
			while (next.time <= num && packages.Count > 0)
			{
				current = next;
				next = packages.Dequeue();
			}
			float num2 = 0f;
			num2 = (num - current.time) / Time.fixedDeltaTime;
			transform.localPosition = Vector3.Lerp(current.position, next.position, num2);
			transform.localRotation = Quaternion.Slerp(current.rotation, next.rotation, num2);
			if (cullingSubscriberBase != null)
			{
				cullingSubscriberBase.Position = transform.position;
			}
		}
	}

	public void Reset()
	{
		packages.Clear();
		current = null;
		next = null;
		targetTransform.position = transform.position;
		targetTransform.rotation = transform.rotation;
	}

	private void FixedUpdate()
	{
		packages.Enqueue(new Package(targetTransform.localPosition, targetTransform.localRotation));
	}
}
