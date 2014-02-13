using UnityEngine;

public class DragonHead : MonoBehaviour
{
	private ParticleSystem linkToTarget;

	private MVDragon worldObject;

	private bool isFiring = true;

	public void Initialize(MVDragon owner)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		worldObject = owner;
		linkToTarget = ((Component)this).gameObject.GetComponentInChildren<ParticleSystem>();
		((Component)linkToTarget).transform.position = ((Component)this).gameObject.transform.position;
		StopFiring();
	}

	private float getParticlesLifeTime()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).gameObject.transform.position - worldObject.DragonTargetArea.transform.position;
		float magnitude = val.magnitude;
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
			if (Object.op_Implicit((Object)(object)linkToTarget))
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
			if (Object.op_Implicit((Object)(object)linkToTarget))
			{
				linkToTarget.enableEmission = true;
			}
		}
	}
}
