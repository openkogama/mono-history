using System;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class PickupItemCenterGun : PickupItemWithDelay
{
	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private ParticleSystem muzzleFlare;

	[SerializeField]
	private ObscuredInt maxAmmo;

	[SerializeField]
	private float projectileSpeed = 70f;

	[SerializeField]
	private float range = 100f;

	[SerializeField]
	private float impulseStrength = 700f;

	private static readonly float damage = CenterGunHitPackage.Create(new Vector3(0f, 0f, 0f)).Damage;

	private ObscuredInt currentAmmo;

	public override AvatarItemType Type => AvatarItemType.CenterGun;

	public override int Quantity => currentAmmo;

	protected override bool IsAmmoDepleted => (int)currentAmmo <= 0;

	private void Awake()
	{
		ResetAmmo();
	}

	public override void ResetAmmo()
	{
		base.ResetAmmo();
		currentAmmo = GetAmmoMultiplier(maxAmmo);
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		isFiring = true;
	}

	public override void TriggerEnd()
	{
		isFiring = false;
	}

	protected override void OnFire(bool isLocal)
	{
		Bullet bullet = Bullet.CreateBullet(PoolEnums.CenterGunBullet, muzzlePoint.position);
		muzzleFlare.Play();
		Ray lineOfFire = new Ray(owner.LookOrigin, owner.LookDirection);
		bullet.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHit, new Bullet.OnHitDelegate(OnBulletHit));
		if (isLocal)
		{
			bullet.onHitLocal = OnLocalBulletHit;
		}
		bullet.Fire(owner.GetAbsolutProjectileSpeed(projectileSpeed), range, lineOfFire, owner.IgnoreWOIDs);
		--currentAmmo;
		Vector3 position = ((!isLocal) ? muzzlePoint.position : (Camera.main.transform.position + Camera.main.transform.forward));
		MVGameControllerBase.AudioManager.Play("CenterGun fire", audioSource, position);
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
		MVGameControllerBase.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(voxelHit, damage);
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient != null)
		{
			InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
			if (interactionDataHandlerBase != null && !MVGameControllerBase.Game.LocalPlayer.IsOnSameTeam(worldObjectClient))
			{
				InteractionData interaction = CenterGunHitPackage.Create(lineOfFire.direction * impulseStrength);
				interactionDataHandlerBase.HandleInteraction(owner, interaction, interactionIsLocal: false);
			}
		}
	}
}
