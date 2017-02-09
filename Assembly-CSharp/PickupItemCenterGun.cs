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

	public ObscuredInt ammo;

	public float projectileSpeed = 70f;

	public float range = 100f;

	public float impulseStrength = 700f;

	public float damage = 13f;

	protected override bool IsAmmoDepleted => (int)ammo <= 0;

	public override AvatarItemType Type => AvatarItemType.CenterGun;

	public override int Quantity => ammo;

	public override void ResetAmmo()
	{
		base.ResetAmmo();
		ammo = 100;
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
		--ammo;
		Vector3 position = ((!isLocal) ? muzzlePoint.position : (Camera.main.transform.position + Camera.main.transform.forward));
		MVGameControllerBase.AudioManager.Play("CenterGun fire", audioSource, position);
	}

	private void OnBulletHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
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
			if (interactionDataHandlerBase != null && !MVGameControllerBase.Game.TeamManager.IsOnSameTeam(worldObjectClient.OwnerActorNr, MVGameControllerBase.Game.LocalPlayer.ActorNr))
			{
				InteractionData interaction = CenterGunHitPackage.Create(lineOfFire.direction * impulseStrength);
				interactionDataHandlerBase.HandleInteraction(interaction, interactionIsLocal: false);
			}
		}
	}
}
