using UnityEngine;

public class Aiming2DCamera
{
	private const float circleRadiusMin = 0.5f;

	private const float circleRadiusMax = 10f;

	private float xFactor = 1f;

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
		xFactor = UpdateXFactor(xFactor);
		lineOfSightDirection = UpdateAim(lineOfSightDirection, xFactor);
		lineOfSightDirection = ConstrainToScreen(origin, lineOfSightDirection, MVGameControllerBase.CameraController.MainCamera.transform.rotation);
		return lineOfSightDirection;
	}

	private Vector3 ConstrainToScreen(Vector3 position, Vector3 direction, Quaternion cameraRotation)
	{
		Vector3 vector = cameraRotation * direction;
		Vector3 position2 = MVGameControllerBase.CameraController.MainCamera.WorldToScreenPoint(vector + position);
		if (position2.x < 0f)
		{
			position2.x = 0f;
		}
		if (position2.y < 0f)
		{
			position2.y = 0f;
		}
		if (position2.x > (float)Screen.width)
		{
			position2.x = Screen.width;
		}
		if (position2.y > (float)Screen.height)
		{
			position2.y = Screen.height;
		}
		Vector3 vector2 = MVGameControllerBase.CameraController.MainCamera.ScreenToWorldPoint(position2);
		vector = vector2 - position;
		return Quaternion.Inverse(cameraRotation) * vector;
	}

	private Vector3 UpdateAim(Vector3 lineOfSightDirection, float xFactor)
	{
		lineOfSightDirection.y += MVInputWrapper.GetAxis("Mouse Y");
		lineOfSightDirection.x += MVInputWrapper.GetAxis("Mouse X");
		lineOfSightDirection = SetOutOfInnerCircle(lineOfSightDirection, xFactor, 0.5f * Scale, 10f * Scale);
		return lineOfSightDirection;
	}

	private static float UpdateXFactor(float xFactor)
	{
		if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveLeft) && !MVInputWrapper.GetBooleanControl(KogamaControls.MoveRight))
		{
			xFactor = -1f;
		}
		if (MVInputWrapper.GetBooleanControl(KogamaControls.MoveRight) && !MVInputWrapper.GetBooleanControl(KogamaControls.MoveLeft))
		{
			xFactor = 1f;
		}
		return xFactor;
	}

	private static Vector3 SetOutOfInnerCircle(Vector3 lineOfSight, float xFactor, float min, float max)
	{
		lineOfSight = HandleZeroVector(xFactor, lineOfSight);
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

	private static Vector3 HandleZeroVector(float xFactor, Vector3 lineOfSightDirection)
	{
		if (lineOfSightDirection == Vector3.zero)
		{
			return new Vector3(xFactor, 0f, 0f);
		}
		return lineOfSightDirection;
	}
}
