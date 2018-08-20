using UnityEngine;

public class AvatarAccessoryParticles : AvatarAccessory
{
	private Vector3 prevPosition;

	private AccessoryParticlesSettings accessoryParticlesSettings;

	private ParticleSystem rootParticleSystem;

	public AccessoryParticlesSettings AccessoryParticlesSettings
	{
		get
		{
			if (accessoryParticlesSettings == null)
			{
				accessoryParticlesSettings = GetComponent<AccessoryParticlesSettings>();
			}
			return accessoryParticlesSettings;
		}
	}

	public ParticleSystem RootParticleSystem
	{
		get
		{
			if (rootParticleSystem == null)
			{
				rootParticleSystem = GetComponentInChildren<ParticleSystem>();
			}
			return rootParticleSystem;
		}
	}

	public override AccessorySettings AccessorySettings => AccessoryParticlesSettings;

	public override Bounds GetWorldBounds()
	{
		return new Bounds(Vector3.zero, new Vector3(2f, 2f, 2f));
	}

	public override Bounds GetLocalBounds()
	{
		return GetWorldBounds();
	}

	protected override void Update()
	{
		base.Update();
		if (AccessoryParticlesSettings.useEmissionMovement)
		{
			if (prevPosition != Transform.position)
			{
				ParticleSystem.EmissionModule emission = RootParticleSystem.emission;
				emission.rateOverTimeMultiplier = AccessoryParticlesSettings.EmitRateMoving;
				prevPosition = Transform.position;
			}
			else
			{
				ParticleSystem.EmissionModule emission2 = RootParticleSystem.emission;
				emission2.rateOverTimeMultiplier = AccessoryParticlesSettings.EmitRateNormal;
			}
		}
	}
}
