using UnityEngine;

public class Aiming2DCameraDesktop
{
	private const float circleRadiusMin = 0.1f;

	private const float circleRadiusMax = 10f;

	private float speed = 0.6f;

	private Vector3 lineOfSightDirection = Vector3.right;

	private float scale = 1f;

	public Vector3 Direction => lineOfSightDirection;

	public float Scale
	{
		get
		{
			return scale;
		}
		set
		{
			scale = value;
		}
	}

	public void EnterGunModeDirection(Vector3 direction)
	{
		lineOfSightDirection = direction;
	}

	public Vector3 UpdateFireDirection(Vector3 origin)
	{
		lineOfSightDirection = UpdateAim(lineOfSightDirection);
		return lineOfSightDirection;
	}

	private Vector3 UpdateAim(Vector3 lineOfSightDirection)
	{
		lineOfSightDirection.y += MVInputWrapper.GetAxis("Mouse Y") * speed;
		lineOfSightDirection.x += MVInputWrapper.GetAxis("Mouse X") * speed;
		lineOfSightDirection = SetOutOfInnerCircle(lineOfSightDirection, 0.1f * Scale, 10f * Scale);
		return lineOfSightDirection;
	}

	private static Vector3 SetOutOfInnerCircle(Vector3 lineOfSight, float min, float max)
	{
		float magnitude = lineOfSight.magnitude;
		if (magnitude > max)
		{
			return lineOfSight.normalized * max;
		}
		if (magnitude < min)
		{
			return lineOfSight.normalized * min;
		}
		return lineOfSight;
	}
}
