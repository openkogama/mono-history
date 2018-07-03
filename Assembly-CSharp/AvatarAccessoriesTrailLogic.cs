using UnityEngine;

public class AvatarAccessoriesTrailLogic : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem particles;

	private Vector3 lastPosition = Vector3.zero;

	private float mininumMovementRequirement = 0.1f;

	private void Update()
	{
		ParticleSystem.EmissionModule emission = particles.emission;
		if ((transform.position - lastPosition).magnitude > mininumMovementRequirement)
		{
			emission.enabled = true;
			particles.startSpeed = 0f;
			lastPosition = transform.position;
		}
		else
		{
			emission.enabled = false;
		}
	}
}
