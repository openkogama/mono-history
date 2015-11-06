using UnityEngine;

public class CameraLerpToDesiredDistance
{
	private const float moveBackSpeedMPrSec = 4f;

	private float newDistance = float.PositiveInfinity;

	public Vector3 Update(Vector3 targetPosition, Vector3 cameraPosition)
	{
		float magnitude = (targetPosition - cameraPosition).magnitude;
		if (magnitude > newDistance)
		{
			newDistance = Mathf.Min(newDistance + 4f * Time.deltaTime, magnitude);
		}
		else
		{
			newDistance = magnitude;
		}
		return targetPosition + (cameraPosition - targetPosition).normalized * newDistance;
	}
}
