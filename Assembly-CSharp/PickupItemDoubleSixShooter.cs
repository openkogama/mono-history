using System;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using UnityEngine;

public class PickupItemDoubleSixShooter : PickupItemWithDelay
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

	public Transform muzzlePoint2;

	public ParticleEmitter fireEmitter;

	public float hitImpact = 300f;

	public Animation animComponentL;

	public Animation animComponentR;

	public AnimationClip gunFireAnim;

	public override AvatarItemType Type => AvatarItemType.DoubleSixShooter;

	public override int Quantity => ammo;

	protected override bool IsAmmoDepleted => (int)ammo <= 0;

	public override void ResetAmmo()
	{
		base.ResetAmmo();
		ammo = 12;
	}

	protected override void OnFire(bool isLocal)
	{
		Bullet bullet = null;
		if ((int)ammo % 2 == 0)
		{
			bullet = Bullet.CreateBullet(PoolEnums.SixShooterBullet, muzzlePoint.position);
			UnityEngine.Object.Instantiate(fireEmitter, muzzlePoint.position, Quaternion.identity);
			animComponentL.Play("RevolverRecoil");
		}
		else
		{
			bullet = Bullet.CreateBullet(PoolEnums.SixShooterBullet, muzzlePoint2.position);
			UnityEngine.Object.Instantiate(fireEmitter, muzzlePoint2.position, Quaternion.identity);
			animComponentR.Play("RevolverRecoil");
		}
		Bullet bullet2 = bullet;
		bullet2.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet2.onHit, new Bullet.OnHitDelegate(OnHit));
		if (isLocal)
		{
			bullet.onHitLocal = OnLoclaHit;
		}
		bullet.Fire(lineOfFire: new Ray(owner.LookOrigin, owner.LookDirection), speed: owner.GetAbsolutProjectileSpeed(bulletSpeed), range: bulletRange, ignoreWoIDs: owner.IgnoreWOIDs);
		--ammo;
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

	private void OnHit(VoxelHit voxelHit, Ray lineOfFire)
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

	private void OnLoclaHit(VoxelHit voxelHit, Ray lineOfFire)
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
				interactionDataHandlerBase.HandleInteraction(DoubleSixShooterHitPackage.Create(value * hitImpact), interactionIsLocal: false);
			}
		}
	}
}
