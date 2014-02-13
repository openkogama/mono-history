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
		RootParticleSystem = ((Component)this).GetComponentInChildren<ParticleSystem>();
	}

	protected override void Start()
	{
		base.Start();
	}

	public override Bounds GetWorldBounds()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return new Bounds(Vector3.zero, new Vector3(2f, 2f, 2f));
	}

	public override Bounds GetLocalBounds()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return GetWorldBounds();
	}

	protected override void Update()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
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
