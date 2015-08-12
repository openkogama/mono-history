using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class PickupItemSixShooter : PickupItemWithDelay
{
	public ObscuredInt ammo;

	public Material hitDecalMaterial;

	public Bullet bulletPrefab;

	public AudioClip bulletHitSound;

	public float recoilImpact = 700f;

	public AnimationCurve damageFalloff;

	public float baseDamage = 30f;

	public float rangeDamage = 70f;

	public float bulletRange = 50f;

	public float bulletSpeed = 80f;

	public AudioClip fireSoundClip;

	public ParticleEmitter fireEmitter;

	public float hitImpact = 300f;

	public Animation animComponent;

	public AnimationClip gunFireAnim;

	public override AvatarItemType Type => AvatarItemType.SixShooter;

	public override int Quantity => ammo;

	protected override bool IsAmmoDepleted => (int)ammo <= 0;

	public override void OnStateChanged(Dictionary<object, object> newState)
	{
		ammo = 6;
	}

	protected override void OnFire(bool isLocal)
	{
		Bullet bullet = Bullet.CreateBullet(bulletPrefab, muzzlePoint.position);
		animComponent.Play("RevolverRecoil");
		Ray lineOfFire = new Ray(owner.LookOrigin, owner.LookDirection);
		bullet.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHit, new Bullet.OnHitDelegate(HandleHit));
		if (isLocal)
		{
			bullet.onHitLocal = HandleDirectHit;
		}
		bullet.Fire(owner.GetAbsolutProjectileSpeed(bulletSpeed), bulletRange, lineOfFire, owner.IgnoreWOIDs);
		--ammo;
		UnityEngine.Object.Instantiate(fireEmitter, muzzlePoint.position, Quaternion.identity);
		if (isLocal)
		{
			MVGameController.AudioManager.Play("projectile fire", fireSoundClip, Camera.main.transform.position + Camera.main.transform.forward, 0.28f, SoundRangeDistance.Long);
		}
		else
		{
			MVGameController.AudioManager.Play("projectile fire", fireSoundClip, muzzlePoint.position, 0.28f, SoundRangeDistance.Long);
		}
		isFiring = false;
		MVRigidBody component = owner.GetComponent<MVRigidBody>();
		if (component != null)
		{
			component.AddImpulse(-transform.forward * recoilImpact);
		}
	}

	private void HandleHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, voxelHit.normal);
		MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(voxelHit.woId);
		string path = ((!(worldObjectClient is MVAvatar)) ? "ParticleFX/SparksSixShooter" : "ParticleFX/BloodSixShooter");
		UnityEngine.Object.Instantiate(Resources.Load(path), voxelHit.point, rotation);
		MeshDecal.Create(new MeshDecal.Hit(voxelHit.point, voxelHit.normal, 1f), hitDecalMaterial, null);
	}

	private void HandleDirectHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		float num = Vector3.Distance(voxelHit.point, owner.transform.position);
		float time = num / bulletRange;
		float damage = damageFalloff.Evaluate(time) * rangeDamage + baseDamage;
		MVGameController.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(voxelHit, damage);
		int woIDHighestInHierarchyWithComponent = MVGameController.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient != null)
		{
			InteractionDataHandlerBase component = worldObjectClient.GameObject.GetComponent<InteractionDataHandlerBase>();
			if (component != null)
			{
				Vector3 value = voxelHit.point - owner.transform.position;
				value = Vector3.Normalize(value);
				InteractionData interaction = SixShooterHitPackage.Create(value * hitImpact, damage);
				component.HandleInteraction(interaction, interactionIsLocal: false);
			}
		}
	}
}
