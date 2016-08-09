using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using UnityEngine;

public class PickupItemThrowingStar : PickupItemWithDelay
{
	public ObscuredInt ammo;

	public Material hitDecalMaterial;

	public BulletThrowingStar bulletPrefab;

	public AudioClip bulletHitSound;

	public AnimationCurve damageFalloff;

	public float baseDamage = 30f;

	public float rangeDamage = 70f;

	public float bulletRangeStraight = 50f;

	public float bulletRangeFall = 50f;

	public float bulletFallRate = 0.1f;

	public float bulletSpeed = 80f;

	public AudioClip fireSoundClip;

	public Animation animationComponent;

	public override AvatarItemType Type => AvatarItemType.ThrowingStar;

	public override int Quantity => ammo;

	protected override bool IsAmmoDepleted => (int)ammo <= 0;

	public override void OnStateChanged(Dictionary<object, object> newState)
	{
		ammo = 50;
		if (animationComponent != null)
		{
			animationComponent.Play("ThrowingStarRotation");
		}
	}

	protected override void OnFire(bool isLocal)
	{
		BulletThrowingStar bulletThrowingStar = BulletThrowingStar.CreateBullet(PoolEnums.ThrowingStarBullet, muzzlePoint.position);
		Ray lineOfFire = new Ray(owner.LookOrigin, owner.LookDirection);
		bulletThrowingStar.onHit = (BulletThrowingStar.OnHitDelegate)Delegate.Combine(bulletThrowingStar.onHit, new BulletThrowingStar.OnHitDelegate(HandleHit));
		if (isLocal)
		{
			bulletThrowingStar.onHitLocal = HandleDirectHit;
		}
		bulletThrowingStar.Fire(owner.GetAbsolutProjectileSpeed(bulletSpeed), bulletRangeStraight, lineOfFire, owner.IgnoreWOIDs, bulletRangeFall, bulletFallRate);
		--ammo;
		if (isLocal)
		{
			MVGameControllerBase.AudioManager.Play("projectile fire", fireSoundClip, Camera.main.transform.position + Camera.main.transform.forward, 0.5f, SoundRangeDistance.Long);
		}
		else
		{
			MVGameControllerBase.AudioManager.Play("projectile fire", fireSoundClip, muzzlePoint.position, 0.5f, SoundRangeDistance.Long);
		}
		isFiring = false;
	}

	private void HandleHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, voxelHit.normal);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
		HitParticle hitParticle = ((!(worldObjectClient is MVAvatar)) ? PrefabPool.Instance.EnumPoolManager.Instantiate<HitParticle>(PoolEnums.NinjaStarSparks) : PrefabPool.Instance.EnumPoolManager.Instantiate<HitParticle>(PoolEnums.NinjaStarBlood));
		hitParticle.transform.position = voxelHit.point;
		hitParticle.transform.rotation = rotation;
		hitParticle.Initialize();
	}

	private void HandleDirectHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient != null)
		{
			float num = Vector3.Distance(voxelHit.point, owner.transform.position);
			float time = num / bulletRangeStraight;
			float damage = damageFalloff.Evaluate(time) * rangeDamage + baseDamage;
			InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
			if (interactionDataHandlerBase != null)
			{
				Vector3 value = voxelHit.point - owner.transform.position;
				value = Vector3.Normalize(value);
				interactionDataHandlerBase.HandleInteraction(ThrowingStarHitPackage.Create(Vector3.zero, damage), interactionIsLocal: false);
			}
		}
	}
}
