using UnityEngine;

public class TargetRotation : MonoBehaviour
{
	private Vector3 eulerAngles = default;

	[SerializeField]
	private float lerpSpeedX;

	[SerializeField]
	private float lerpSpeedY;

	public Vector3 EulerAngles => eulerAngles;

	public void SetTargetRotation(Vector2 pitchYaw)
	{
		SetTargetRotation(pitchYaw.x, pitchYaw.y);
	}

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
		float y = Mathf.LerpAngle(vector.y, eulerAngles.y, Time.deltaTime * lerpSpeedY);
		return Quaternion.Euler(x, y, 0f);
	}
}
