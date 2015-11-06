using System.Collections.Generic;
using UnityEngine;

public class CameraCollision
{
	private const float verySmallDistance = 0.005f;

	public bool Collide(out Vector3 newPos, float cameraRadius, float baseDistance, Vector3 targetPosition, Vector3 cameraPosition, HashSet<int> ignoreIDs)
	{
		VoxelHit hit;
		return Collide(out hit, out newPos, cameraRadius, baseDistance, targetPosition, cameraPosition, ignoreIDs);
	}

	protected bool Collide(out VoxelHit hit, out Vector3 newPos, float cameraRadius, float baseDistance, Vector3 targetPosition, Vector3 cameraPosition, HashSet<int> ignoreIDs)
	{
		newPos = cameraPosition;
		Vector3 vector = cameraPosition - targetPosition;
		vector.Normalize();
		Ray ray = new Ray(targetPosition, vector);
		int num = LayerMask.NameToLayer("Default");
		int layerMask = 1 << num;
		if (!CollisionDetection.MVSphereCast(ray, cameraRadius, out hit, baseDistance, ignoreIDs, layerMask))
		{
			return false;
		}
		if (hit.distance < Mathf.Epsilon)
		{
			return false;
		}
		MathFunctions.DistancePointLine(hit.point, targetPosition, cameraPosition + vector * cameraRadius, out var distance, out var intersection, out var u);
		if (u < 0f)
		{
			Debug.LogWarning("u < 0.0f");
			newPos = targetPosition;
			return true;
		}
		if (u > 1f)
		{
			Debug.LogWarning("u > 1.0f");
			return false;
		}
		float num2 = 0f;
		if (cameraRadius > distance)
		{
			num2 = Mathf.Sqrt(cameraRadius * cameraRadius - distance * distance);
		}
		newPos = intersection - ray.direction * (num2 - 0.005f);
		return true;
	}
}
