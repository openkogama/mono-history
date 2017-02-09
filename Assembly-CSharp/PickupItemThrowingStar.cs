using System;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using UnityEngine;

public class PickupItemThrowingStar : PickupItemWithDelay
{
	public ObscuredInt ammo;

	public Material hitDecalMaterial;

	public BulletThrowingStar bulletPrefab;

	public AudioClip bulletHitSound;

	public float baseDamage = 30f;

	public float bulletRangeStraight = 50f;

	public float bulletRangeFall = 50f;

	public float bulletFallRate = 0.1f;

	public float bulletSpeed = 80f;

	public AudioClip fireSoundClip;

	public override AvatarItemType Type => AvatarItemType.ThrowingStar;

	public override int Quantity => ammo;

	protected override bool IsAmmoDepleted => (int)ammo <= 0;

	public override void ResetAmmo()
	{
		base.ResetAmmo();
		ammo = 50;
	}

	protected override void OnFire(bool isLocal)
	{
		BulletThrowingStar bulletThrowingStar = BulletThrowingStar.CreateBullet(PoolEnums.ThrowingStarBullet, muzzlePoint.position);
		bulletThrowingStar.onHit = (BulletThrowingStar.OnHitDelegate)Delegate.Combine(bulletThrowingStar.onHit, new BulletThrowingStar.OnHitDelegate(OnBulletHit));
		if (isLocal)
		{
			bulletThrowingStar.onHitLocal = OnLocalBulletHit;
		}
		bulletThrowingStar.Fire(lineOfFire: new Ray(owner.LookOrigin, owner.LookDirection), speed: owner.GetAbsolutProjectileSpeed(bulletSpeed), rangeStraight: bulletRangeStraight, ignoreWoIDs: owner.IgnoreWOIDs, rangeFall: bulletRangeFall, fallRate: bulletFallRate);
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

	private void OnBulletHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
		if (worldObjectClient is IBulletImpactVisualizer)
		{
			((IBulletImpactVisualizer)worldObjectClient).VisualizeBulletImpact(voxelHit, lineOfFire, owner.WorldObjectOwner.OwnerActorNr, baseDamage);
		}
		else
		{
			OneShotPooledParticleSystem.Instantiate(PoolEnums.NormalBulletSparks, voxelHit.point, Quaternion.LookRotation(voxelHit.normal));
		}
	}

	private void OnLocalBulletHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient != null)
		{
			InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
			if (interactionDataHandlerBase != null && !MVGameControllerBase.Game.TeamManager.IsOnSameTeam(worldObjectClient.OwnerActorNr, MVGameControllerBase.Game.LocalPlayer.ActorNr))
			{
				interactionDataHandlerBase.HandleInteraction(ThrowingStarHitPackage.Create(), interactionIsLocal: false);
			}
		}
	}
}
