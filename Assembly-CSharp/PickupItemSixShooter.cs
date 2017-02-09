using System;
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

	public float baseDamage = 30f;

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

	public override void ResetAmmo()
	{
		base.ResetAmmo();
		ammo = 6;
	}

	protected override void OnFire(bool isLocal)
	{
		animComponent.Play("RevolverRecoil");
		Bullet bullet = Bullet.CreateBullet(PoolEnums.SixShooterBullet, muzzlePoint.position);
		bullet.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHit, new Bullet.OnHitDelegate(OnBulletHit));
		if (isLocal)
		{
			bullet.onHitLocal = OnLocalBulletHit;
		}
		bullet.Fire(lineOfFire: new Ray(owner.LookOrigin, owner.LookDirection), speed: owner.GetAbsolutProjectileSpeed(bulletSpeed), range: bulletRange, ignoreWoIDs: owner.IgnoreWOIDs);
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
			component.AddImpulse(-owner.LookDirection * recoilImpact);
		}
	}

	private void OnBulletHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
		if (worldObjectClient is IBulletImpactVisualizer)
		{
			((IBulletImpactVisualizer)worldObjectClient).VisualizeBulletImpact(voxelHit, lineOfFire, owner.WorldObjectOwner.OwnerActorNr, baseDamage);
		}
		else
		{
			OneShotPooledParticleSystem.Instantiate(PoolEnums.SixShooterSparks, voxelHit.point, Quaternion.LookRotation(voxelHit.normal));
		}
	}

	private void OnLocalBulletHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		MVGameControllerBase.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(voxelHit, baseDamage);
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient != null)
		{
			InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
			if (interactionDataHandlerBase != null && !MVGameControllerBase.Game.TeamManager.IsOnSameTeam(worldObjectClient.OwnerActorNr, MVGameControllerBase.Game.LocalPlayer.ActorNr))
			{
				Vector3 value = voxelHit.point - owner.transform.position;
				value = Vector3.Normalize(value);
				InteractionData interaction = SixShooterHitPackage.Create(value * hitImpact);
				interactionDataHandlerBase.HandleInteraction(interaction, interactionIsLocal: false);
			}
		}
	}
}
