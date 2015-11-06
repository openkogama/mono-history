using System.Collections.Generic;
using UnityEngine;

public class SmoothLookAt
{
	private int samleLength = 5;

	private Queue<Vector3> prevVelocities = new Queue<Vector3>();

	private float maxMag = 30f;

	public Vector3 GetCurrentLookAt(Vector3 velocity)
	{
		while (prevVelocities.Count >= samleLength)
		{
			prevVelocities.Dequeue();
		}
		velocity.x = (velocity.z = 0f);
		prevVelocities.Enqueue(velocity * Time.deltaTime);
		Vector3 result = Vector3.zero;
		foreach (Vector3 prevVelocity in prevVelocities)
		{
			result += prevVelocity;
		}
		result /= (float)prevVelocities.Count;
		if (result.sqrMagnitude > maxMag * maxMag)
		{
			result = result.normalized * maxMag;
		}
		return result;
	}

	public void Clear()
	{
		prevVelocities.Clear();
	}
}
