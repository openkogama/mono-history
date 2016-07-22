using System;
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

	private bool isLocal;

	private int throwingStarsFired;

	private float fireTime;

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
		this.isLocal = isLocal;
		StartFire();
	}

	public override void UpdateControllerUpdate()
	{
		base.UpdateControllerUpdate();
		if (isFiring)
		{
			DoFire(isLocal);
		}
	}

	public override void TriggerEnd()
	{
	}

	private void StartFire()
	{
		isFiring = true;
		throwingStarsFired = 0;
		fireTime = Time.time;
	}

	private void FireThrowingStar(bool isLocal)
	{
		BulletThrowingStar bulletThrowingStar = BulletThrowingStar.CreateBullet(PoolEnums.MultiThrowingStarBullet, muzzlePoint.position);
		float num = UnityEngine.Random.Range(0f - randomDirection, randomDirection);
		if (throwingStarsFired == 2)
		{
			num = 0f;
		}
		Vector3 direction = Quaternion.AngleAxis(((float)throwingStarsFired - (float)numStars / 2f + num) * spread, shootingAngleAxis) * owner.LookDirection;
		Ray lineOfFire = new Ray(owner.LookOrigin, direction);
		bulletThrowingStar.onHit = (BulletThrowingStar.OnHitDelegate)Delegate.Combine(bulletThrowingStar.onHit, new BulletThrowingStar.OnHitDelegate(HandleHit));
		if (isLocal)
		{
			bulletThrowingStar.onHitLocal = HandleDirectHit;
		}
		bulletThrowingStar.Fire(owner.GetAbsolutProjectileSpeed(bulletSpeed), bulletRangeStraight, lineOfFire, owner.IgnoreWOIDs, bulletRangeFall, bulletFallRate);
		if (isLocal)
		{
			MVGameControllerBase.AudioManager.Play("projectile fire", fireSoundClip, Camera.main.transform.position + Camera.main.transform.forward, 0.5f, SoundRangeDistance.Long);
		}
		else
		{
			MVGameControllerBase.AudioManager.Play("projectile fire", fireSoundClip, muzzlePoint.position, 0.5f, SoundRangeDistance.Long);
		}
		throwingStarsFired++;
		--ammo;
		if (throwingStarsFired >= numStars)
		{
			isFiring = false;
		}
	}

	private void DoFire(bool isLocal)
	{
		if (throwingStarsFired < numStars && Time.time >= fireTime + fireDelay)
		{
			FireThrowingStar(isLocal);
		}
		if ((int)ammo <= 0)
		{
			MVEquipable component = owner.WorldObjectOwner.GameObject.GetComponent<MVEquipable>();
			if (component != null)
			{
				component.Unequip();
			}
		}
	}

	private void HandleHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, voxelHit.normal);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
		GameObject original = ((!(worldObjectClient is MVAvatar)) ? PrefabPool.Instance.ParticleSparksThrowingStar : PrefabPool.Instance.ParticleBlooxThrowingStar);
		UnityEngine.Object.Instantiate(original, voxelHit.point, rotation);
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
				interactionDataHandlerBase.HandleInteraction(MultiThrowingStarHitPackage.Create(Vector3.zero, damage), interactionIsLocal: false);
			}
		}
	}
}
