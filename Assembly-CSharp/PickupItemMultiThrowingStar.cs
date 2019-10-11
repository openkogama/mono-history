using System;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using UnityEngine;

public class PickupItemMultiThrowingStar : PickupItemWithDelay
{
	[SerializeField]
	private ObscuredInt maxAmmo = 150;

	[SerializeField]
	private float bulletRangeStraight = 50f;

	[SerializeField]
	private float bulletRangeFall = 50f;

	[SerializeField]
	private float bulletFallRate = 0.1f;

	[SerializeField]
	private float bulletSpeed = 80f;

	[SerializeField]
	private int numStars = 5;

	[SerializeField]
	private float fireSpacingDelay = 0.1f;

	[SerializeField]
	private float fireRate = 1f;

	[SerializeField]
	private AudioSource fireSound;

	private static readonly float baseDamage = MultiThrowingStarHitPackage.Create().Damage;

	private ObscuredInt currentAmmo;

	private int throwingStarsFired;

	private float fireTime;

	private bool isLocal;

	public override AvatarItemType Type => AvatarItemType.MultiThrowingStar;

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

	protected override void OnHolstered()
	{
		base.OnHolstered();
	}

	protected override void OnUnholstered()
	{
		base.OnUnholstered();
	}

	public override void OnEquip()
	{
		base.OnEquip();
	}

	protected override void OnFire(bool isLocal)
	{
		this.isLocal = isLocal;
		TriggerBegin(owner.WorldObjectOwner.Id);
	}

	public override void UpdateControllerUpdate()
	{
		base.UpdateControllerUpdate();
		if (isFiring)
		{
			DoFire(isLocal);
		}
	}

	public override void OnEnterVehicleWithWeapon()
	{
		base.OnEnterVehicleWithWeapon();
		if (throwingStarsFired >= numStars)
		{
			isFiring = false;
		}
	}

	public override void OnLeaveVehicleWithWeapon()
	{
		base.OnLeaveVehicleWithWeapon();
		if (throwingStarsFired >= numStars)
		{
			isFiring = false;
		}
	}

	private void TriggerFire()
	{
		if (!isFiring && Time.time > fireRate + fireTime)
		{
			isFiring = true;
			throwingStarsFired = 0;
			fireTime = Time.time;
		}
	}

	public override void TriggerBegin(int instigator)
	{
		TriggerFire();
	}

	public override void TriggerEnd()
	{
	}

	private void DoFire(bool isLocal)
	{
		if (throwingStarsFired < numStars && Time.time >= fireTime + fireSpacingDelay)
		{
			fireTime = Time.time;
			Fire(isLocal);
		}
		if ((int)currentAmmo <= 0 && !HasUnlimitedAmmo)
		{
			MVEquipable component = owner.WorldObjectOwner.GameObject.GetComponent<MVEquipable>();
			if (component != null)
			{
				component.Unequip();
			}
		}
	}

	private void Fire(bool isLocal)
	{
		BulletThrowingStar bulletThrowingStar = BulletThrowingStar.CreateBullet(PoolEnums.MultiThrowingStarBullet, muzzlePoint.position);
		Ray lineOfFire = new Ray(owner.LookOrigin, owner.LookDirection);
		bulletThrowingStar.onHit = (BulletThrowingStar.OnHitDelegate)Delegate.Combine(bulletThrowingStar.onHit, new BulletThrowingStar.OnHitDelegate(OnBulletHit));
		if (isLocal)
		{
			bulletThrowingStar.onHitLocal = OnLocalBulletHit;
		}
		bulletThrowingStar.Fire(owner.GetAbsolutProjectileSpeed(bulletSpeed), bulletRangeStraight, lineOfFire, owner.IgnoreWOIDs, bulletRangeFall, bulletFallRate);
		if (isLocal)
		{
			MVGameControllerBase.AudioManager.Play("projectile fire", fireSound, Camera.main.transform.position + Camera.main.transform.forward);
		}
		else
		{
			MVGameControllerBase.AudioManager.Play("projectile fire", fireSound, muzzlePoint.position);
		}
		throwingStarsFired++;
		--currentAmmo;
		if (throwingStarsFired >= numStars)
		{
			isFiring = false;
			throwingStarsFired = 0;
		}
	}

	private void OnBulletHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
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
			if (interactionDataHandlerBase != null && !MVGameControllerBase.Game.LocalPlayer.IsOnSameTeam(worldObjectClient))
			{
				Vector3 value = voxelHit.point - owner.transform.position;
				value = Vector3.Normalize(value);
				interactionDataHandlerBase.HandleInteraction(owner, MultiThrowingStarHitPackage.Create(), interactionIsLocal: false);
			}
		}
	}
}
