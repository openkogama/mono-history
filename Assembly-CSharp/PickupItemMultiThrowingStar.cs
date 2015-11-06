using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using UnityEngine;

public class PickupItemMultiThrowingStar : PickupItemWithDelay
{
	private Vector3 shootingAngleAxis = Vector3.up;

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

	public ParticleEmitter fireEmitter;

	public Animation animComponent;

	public AnimationClip gunFireAnim;

	public int numStars = 5;

	public float spread = 0.2f;

	public float randomDirection = 0.001f;

	public float fireDelay = 0.1f;

	public override AvatarItemType Type => AvatarItemType.MultiThrowingStar;

	public override int Quantity => ammo;

	protected override bool IsAmmoDepleted => (int)ammo <= 0;

	public override void OnStateChanged(Dictionary<object, object> newState)
	{
		ammo = 50;
		if (animComponent != null)
		{
			animComponent.Play("MultiThrowingStarRotation");
		}
	}

	public override void OnEquip()
	{
		base.OnEquip();
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			shootingAngleAxis = Vector3.forward;
		}
	}

	protected override void OnFire(bool isLocal)
	{
		StartCoroutine(DoFire(isLocal));
	}

	private IEnumerator DoFire(bool isLocal)
	{
		for (int i = 0; i < numStars; i++)
		{
			float fireTime = Time.time;
			BulletThrowingStar p = BulletThrowingStar.CreateBullet(bulletPrefab, muzzlePoint.position);
			float r = UnityEngine.Random.Range(0f - randomDirection, randomDirection);
			if (i == 2)
			{
				r = 0f;
			}
			Ray lineOfFire = new Ray(direction: Quaternion.AngleAxis(((float)i - (float)numStars / 2f + r) * spread, shootingAngleAxis) * owner.LookDirection, origin: owner.LookOrigin);
			p.onHit = (BulletThrowingStar.OnHitDelegate)Delegate.Combine(p.onHit, new BulletThrowingStar.OnHitDelegate(HandleHit));
			if (isLocal)
			{
				p.onHitLocal = HandleDirectHit;
			}
			p.Fire(owner.GetAbsolutProjectileSpeed(bulletSpeed), bulletRangeStraight, lineOfFire, owner.IgnoreWOIDs, bulletRangeFall, bulletFallRate);
			if (isLocal)
			{
				MVGameControllerBase.AudioManager.Play("projectile fire", fireSoundClip, Camera.main.transform.position + Camera.main.transform.forward, 0.5f, SoundRangeDistance.Long);
			}
			else
			{
				MVGameControllerBase.AudioManager.Play("projectile fire", fireSoundClip, muzzlePoint.position, 0.5f, SoundRangeDistance.Long);
			}
			--ammo;
			while (Time.time < fireTime + fireDelay)
			{
				yield return 0;
			}
		}
		isFiring = false;
		if ((int)ammo <= 0)
		{
			MVEquipable equipable = owner.WorldObjectOwner.GameObject.GetComponent<MVEquipable>();
			if (equipable != null)
			{
				equipable.Unequip();
			}
		}
	}

	private void HandleHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, voxelHit.normal);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
		string path = ((!(worldObjectClient is MVAvatar)) ? "ParticleFX/SparksThrowingStar" : "ParticleFX/BloodThrowingStar");
		UnityEngine.Object.Instantiate(Resources.Load(path), voxelHit.point, rotation);
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
			InteractionDataHandlerBase component = worldObjectClient.GameObject.GetComponent<InteractionDataHandlerBase>();
			if (component != null)
			{
				Vector3 value = voxelHit.point - owner.transform.position;
				value = Vector3.Normalize(value);
				component.HandleInteraction(MultiThrowingStarHitPackage.Create(Vector3.zero, damage), interactionIsLocal: false);
			}
		}
	}
}
