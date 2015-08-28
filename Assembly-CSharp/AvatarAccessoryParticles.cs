using MV.Common;
using UnityEngine;

public class AvatarAccessoryParticles : AvatarAccessory
{
	private Vector3 prevPosition;

	private AccessoryParticlesSettings accessoryParticlesSettings;

	public ParticleSystem RootParticleSystem;

	public override AccessorySettings AccessorySettings => accessoryParticlesSettings;

	protected override void Awake()
	{
		accessoryParticlesSettings = GetComponent<AccessoryParticlesSettings>();
		base.Awake();
		Category = AvatarAccessoryCategory.Particles;
		RootParticleSystem = GetComponentInChildren<ParticleSystem>();
	}

	protected override void Start()
	{
		base.Start();
	}

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
		if (prevPosition != Transform.position)
		{
			RootParticleSystem.emissionRate = accessoryParticlesSettings.EmitRateMoving;
			prevPosition = Transform.position;
		}
		else
		{
			RootParticleSystem.emissionRate = accessoryParticlesSettings.EmitRateNormal;
		}
	}
}
