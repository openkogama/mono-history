using UnityEngine;

public class TargetRotation : MonoBehaviour
{
	private const float maxInertialAngleBehind = 70f;

	private Vector3 eulerAngles = default;

	[SerializeField]
	private float lerpSpeedX;

	[SerializeField]
	private float lerpSpeedY;

	public Vector3 EulerAngles => eulerAngles;

	public void SetTargetRotation(float pitch, float yaw)
	{
		eulerAngles.x = pitch;
		eulerAngles.y = yaw;
		eulerAngles.z = 0f;
	}

	public Quaternion GetLerpRotation(Quaternion from)
	{
		Vector3 vector = from.eulerAngles;
		float x = Mathf.LerpAngle(vector.x, eulerAngles.x, Time.deltaTime * lerpSpeedX);
		vector.y = ClampDegreeDiff(vector.y, eulerAngles.y, 70f);
		float y = Mathf.LerpAngle(vector.y, eulerAngles.y, Time.deltaTime * lerpSpeedY);
		return Quaternion.Euler(x, y, 0f);
	}

	private static float ClampDegreeDiff(float target, float to, float maxDiff)
	{
		float num = Mathf.DeltaAngle(target, to);
		float num2 = 0f;
		if (num > maxDiff)
		{
			num2 = num - maxDiff;
		}
		else if (num < 0f - maxDiff)
		{
			num2 = num + maxDiff;
		}
		return target + num2;
	}
}
