using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class PickupItemIceGun : PickupItemWithDelay
{
	public ObscuredInt ammo;

	public float recoilImpact = 700f;

	public float baseDamage = 30f;

	public float rangeDamage = 70f;

	public float bulletRange = 50f;

	public float bulletSpeed = 80f;

	public float hitImpact;

	public AudioClip fireSoundClip;

	public AudioClip bulletHitSound;

	public Bullet bulletPrefab;

	public ParticleEmitter fireEmitter;

	public Material hitDecalMaterial;

	public Animation animComponent;

	public AnimationClip gunFireAnim;

	public AnimationCurve damageFalloff;

	private string animRecoil = "RevolverRecoil";

	public override int Quantity => ammo;

	public override AvatarItemType Type => AvatarItemType.IceGun;

	protected override bool IsAmmoDepleted => (int)ammo <= 0;

	public override void OnStateChanged(Dictionary<object, object> newState)
	{
		ammo = 5;
	}

	protected override void OnFire(bool isLocal)
	{
		Bullet bullet = Bullet.CreateBullet(bulletPrefab, muzzlePoint.position);
		animComponent.Play(animRecoil);
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
			MVGameControllerBase.AudioManager.Play("projectile fire", fireSoundClip, Camera.main.transform.position + Camera.main.transform.forward, 0.28f, SoundRangeDistance.Long);
		}
		else
		{
			MVGameControllerBase.AudioManager.Play("projectile fire", fireSoundClip, muzzlePoint.position, 0.28f, SoundRangeDistance.Long);
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
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
		string path = ((!(worldObjectClient is MVAvatar)) ? "ParticleFX/Sparks" : "ParticleFX/FluffySmoke");
		UnityEngine.Object.Instantiate(Resources.Load(path), voxelHit.point, rotation);
		MeshDecal.Create(new MeshDecal.Hit(voxelHit.point, voxelHit.normal, 1f), hitDecalMaterial, null);
	}

	private void HandleDirectHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		float num = Vector3.Distance(voxelHit.point, owner.transform.position);
		float time = num / bulletRange;
		float damage = damageFalloff.Evaluate(time) * rangeDamage + baseDamage;
		MVGameControllerBase.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(voxelHit, damage);
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient != null)
		{
			InteractionDataHandlerBase component = worldObjectClient.GameObject.GetComponent<InteractionDataHandlerBase>();
			if (component != null)
			{
				Vector3 value = voxelHit.point - owner.transform.position;
				value = Vector3.Normalize(value);
				InteractionData interaction = IceGunHitPackage.Create(value * hitImpact);
				component.HandleInteraction(interaction, interactionIsLocal: false);
			}
		}
	}
}
