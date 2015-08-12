using UnityEngine;

public class DragonHead : MonoBehaviour
{
	private ParticleSystem linkToTarget;

	private MVDragon worldObject;

	private bool isFiring = true;

	public void Initialize(MVDragon owner)
	{
		worldObject = owner;
		linkToTarget = gameObject.GetComponentInChildren<ParticleSystem>();
		linkToTarget.transform.position = gameObject.transform.position;
		StopFiring();
	}

	private float getParticlesLifeTime()
	{
		float magnitude = (gameObject.transform.position - worldObject.DragonTargetArea.transform.position).magnitude;
		return magnitude / linkToTarget.startSpeed;
	}

	private void Update()
	{
		if (worldObject == null)
		{
			StopFiring();
		}
		else if (isFiring)
		{
			linkToTarget.startLifetime = getParticlesLifeTime();
		}
	}

	public void StopFiring()
	{
		if (isFiring)
		{
			isFiring = false;
			if ((bool)linkToTarget)
			{
				linkToTarget.enableEmission = false;
			}
		}
	}

	public void StartFiring()
	{
		if (!isFiring)
		{
			isFiring = true;
			if ((bool)linkToTarget)
			{
				linkToTarget.enableEmission = true;
			}
		}
	}
}
