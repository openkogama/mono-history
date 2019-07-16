using System;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using UnityEngine;

public class PickupItemThrowingStar : PickupItemWithDelay
{
	[SerializeField]
	private ObscuredInt maxAmmo;

	[SerializeField]
	private float bulletRangeStraight = 50f;

	[SerializeField]
	private float bulletRangeFall = 50f;

	[SerializeField]
	private float bulletFallRate = 0.1f;

	[SerializeField]
	private float bulletSpeed = 80f;

	[SerializeField]
	private AudioSource fireSound;

	private static readonly float damage = ThrowingStarHitPackage.Create().Damage;

	private ObscuredInt currentAmmo;

	public override AvatarItemType Type => AvatarItemType.ThrowingStar;

	public override int Quantity => currentAmmo;

	protected override bool IsAmmoDepleted => (int)currentAmmo <= 0 && !HasUnlimitedAmmo;

	private void Awake()
	{
		ResetAmmo();
	}

	public override void ResetAmmo()
	{
		base.ResetAmmo();
		currentAmmo = GetAmmoMultiplier(maxAmmo);
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
		--currentAmmo;
		if (isLocal)
		{
			MVGameControllerBase.AudioManager.Play("projectile fire", fireSound, Camera.main.transform.position + Camera.main.transform.forward);
		}
		else
		{
			MVGameControllerBase.AudioManager.Play("projectile fire", fireSound, muzzlePoint.position);
		}
		isFiring = false;
	}

	private void OnBulletHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient is IBulletImpactVisualizer)
		{
			((IBulletImpactVisualizer)worldObjectClient).VisualizeBulletImpact(voxelHit, lineOfFire, owner.WorldObjectOwner.OwnerActorNr, damage);
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
			if (interactionDataHandlerBase != null && !MVGameControllerBase.Game.LocalPlayer.IsOnSameTeam(worldObjectClient))
			{
				interactionDataHandlerBase.HandleInteraction(owner, ThrowingStarHitPackage.Create(), interactionIsLocal: false);
			}
		}
	}
}
