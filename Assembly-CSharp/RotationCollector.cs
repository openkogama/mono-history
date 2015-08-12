using UnityEngine;

public class RotationCollector : MonoBehaviour
{
	private Vector3 prevDirection = Vector3.zero;

	private float angleDiff;

	private void Start()
	{
		prevDirection = transform.rotation * Vector3.forward;
		prevDirection.y = 0f;
		prevDirection.Normalize();
	}

	private void Update()
	{
		Vector3 v = transform.rotation * Vector3.forward;
		v.y = 0f;
		v.Normalize();
		angleDiff += MathFunctions.SignedAngle(prevDirection, v, Vector3.up) * 57.29578f;
		prevDirection = v;
		int num = Mathf.FloorToInt(angleDiff);
		GameSessionCounters.SetCount(GameSessionCounterType.CameraRotation, num);
		angleDiff -= num;
	}
}
