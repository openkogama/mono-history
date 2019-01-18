using UnityEngine;

public class ParticlesFollowWithNoRotate : MonoBehaviour
{
	private Quaternion startRotation;

	private void Start()
	{
		startRotation = transform.localRotation;
	}

	private void LateUpdate()
	{
		transform.rotation = startRotation;
	}
}
