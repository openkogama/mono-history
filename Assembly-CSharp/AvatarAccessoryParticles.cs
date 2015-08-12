using MV.Common;
using UnityEngine;

[AddComponentMenu("KoGaMa/AvatarAccessories/Particles")]
public class AvatarAccessoryParticles : AvatarAccessory
{
	public ParticleSystem RootParticleSystem;

	public float EmitRateNormal = 4f;

	public float EmitRateMoving = 10f;

	private Vector3 prevPosition;

	protected override void Awake()
	{
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
			RootParticleSystem.emissionRate = EmitRateMoving;
			prevPosition = Transform.position;
		}
		else
		{
			RootParticleSystem.emissionRate = EmitRateNormal;
		}
	}
}
