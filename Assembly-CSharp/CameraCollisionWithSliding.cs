using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollisionWithSliding : CameraCollision
{
	private float slideDir;

	private int prevFrameCount;

	private float checkDistanceFactor = 0.5f;

	public bool CollideWithSliding(out Vector3 newCameraPosition, float cameraRadius, float distanceToAvatar, Vector3 targetPosition, Vector3 cameraPosition, HashSet<int> ignoreAvatarId)
	{
		ResetIfNotUpdate();
		if (DoCollideWithSliding(out newCameraPosition, cameraRadius, distanceToAvatar * checkDistanceFactor, targetPosition, cameraPosition, ignoreAvatarId))
		{
			return true;
		}
		Reset();
		return false;
	}

	private void ResetIfNotUpdate()
	{
		if (Time.frameCount - prevFrameCount > 1)
		{
			Reset();
		}
		prevFrameCount = Time.frameCount;
	}

	private bool DoCollideWithSliding(out Vector3 newPos, float cameraRadius, float baseDistance, Vector3 targetPosition, Vector3 cameraPosition, HashSet<int> ignoreIDs)
	{
		if (!Collide(out var hit, out newPos, cameraRadius, baseDistance, targetPosition, cameraPosition, ignoreIDs))
		{
			return false;
		}
		Vector3 slideVector = GetSlideVector(newPos, hit.point, targetPosition, cameraPosition, baseDistance);
		newPos += slideVector;
		return true;
	}

	private void Reset()
	{
		slideDir = 0f;
	}

	private Vector3 GetSlideVector(Vector3 newPos, Vector3 hitPoint, Vector3 targetPosition, Vector3 cameraPosition, float baseDistance)
	{
		Vector3 normalized = (targetPosition - cameraPosition).normalized;
		normalized *= baseDistance;
		normalized.y = 0f;
		baseDistance = normalized.magnitude;
		newPos.y = 0f;
		hitPoint.y = 0f;
		targetPosition.y = 0f;
		cameraPosition.y = 0f;
		return GetSlideVectorXZPlane(newPos, hitPoint, targetPosition, cameraPosition, baseDistance);
	}

	private Vector3 GetSlideVectorXZPlane(Vector3 newPos, Vector3 hitPoint, Vector3 targetPosition, Vector3 cameraPosition, float baseDistance)
	{
		Vector3 vector = GetSlideDir(newPos, hitPoint, targetPosition, cameraPosition);
		float slideVectorLength = GetSlideVectorLength(vector, newPos, targetPosition, baseDistance);
		return vector * slideVectorLength;
	}

	private Vector3 GetSlideDir(Vector3 newPos, Vector3 hitPoint, Vector3 targetPosition, Vector3 cameraPosition)
	{
		Vector3 normalized = (newPos - hitPoint).normalized;
		Vector3 normalized2 = (targetPosition - cameraPosition).normalized;
		Vector3 vector = Vector3.Cross(normalized, normalized2);
		if (vector.y == 0f)
		{
			throw new Exception();
		}
		if (slideDir == 0f)
		{
			slideDir = 1f;
			if (vector.y < 0f)
			{
				slideDir = -1f;
			}
		}
		Vector3 vector2 = Vector3.Cross(normalized, Vector3.up);
		vector2.Normalize();
		return vector2 * slideDir;
	}

	private static float GetSlideVectorLength(Vector3 slideVector, Vector3 newPos, Vector3 targetPosition, float baseDistance)
	{
		Vector3 vector = targetPosition - newPos;
		Debug.DrawLine(newPos, newPos + vector);
		float num = Vector3.Angle(vector, slideVector);
		float magnitude = vector.magnitude;
		float num2 = baseDistance / Mathf.Sin(num * ((float)Math.PI / 180f));
		float f = magnitude / num2;
		float num3 = Mathf.Asin(f) * 57.29578f;
		float num4 = 180f - (num3 + num);
		float num5 = num2 * Mathf.Sin(num4 * ((float)Math.PI / 180f));
		Debug.DrawLine(newPos, newPos + slideVector * num5);
		return num5;
	}
}
