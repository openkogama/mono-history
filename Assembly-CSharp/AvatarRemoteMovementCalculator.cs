using UnityEngine;

public class AvatarRemoteMovementCalculator : MonoBehaviour
{
	private Vector3 prevPos = Vector3.zero;

	private Vector3 velocityEstimate = Vector3.zero;

	public Vector3 VelocityEstimate => velocityEstimate;

	private void Awake()
	{
		prevPos = transform.position;
	}

	private void Update()
	{
		velocityEstimate = (transform.position - prevPos) / Time.deltaTime;
		prevPos = transform.position;
	}
}
