using System.Collections.Generic;
using UnityEngine;

public class SmoothTouchAxis
{
	private readonly int sampleLength;

	private readonly Queue<Vector3> prevVelocities = new Queue<Vector3>();

	public SmoothTouchAxis(int sampleLength)
	{
		this.sampleLength = sampleLength;
	}

	public void Reset()
	{
		prevVelocities.Clear();
	}

	public Vector3 UpdateSmoothVelocity(Vector3 movement)
	{
		movement /= Time.deltaTime;
		prevVelocities.Enqueue(movement);
		while (prevVelocities.Count >= sampleLength)
		{
			prevVelocities.Dequeue();
		}
		Vector3 zero = Vector3.zero;
		foreach (Vector3 prevVelocity in prevVelocities)
		{
			zero += prevVelocity;
		}
		zero /= (float)prevVelocities.Count;
		return zero * Time.deltaTime;
	}
}
