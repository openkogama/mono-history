using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class PickupItemCenterGun : PickupItemWithDelay
{
	public ObscuredInt ammo;

	public Material hitDecalMaterial;

	public Bullet bulletPrefab;

	public AudioClip bulletHitSound;

	public override AvatarItemType Type => AvatarItemType.CenterGun;

	public override int Quantity => ammo;

	protected override bool IsAmmoDepleted => (int)ammo <= 0;

	public override void OnStateChanged(Dictionary<object, object> newState)
	{
		ammo = 100;
	}

	protected override void OnFire(bool isLocal)
	{
		Bullet bullet = Bullet.CreateBullet(bulletPrefab, muzzlePoint.position);
		Ray lineOfFire = new Ray(owner.LookOrigin, owner.LookDirection);
		bullet.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHit, new Bullet.OnHitDelegate(HandleHit));
		if (isLocal)
		{
			bullet.onHitLocal = HandleDirectHit;
		}
		bullet.Fire(owner.GetAbsolutProjectileSpeed(70f), 100f, lineOfFire, owner.IgnoreWOIDs);
		--ammo;
		MVGameControllerBase.AudioManager.Play("projectile fire", GetComponent<AudioSource>(), muzzlePoint.position);
		if (isLocal)
		{
			MVGameControllerBase.AudioManager.Play("projectile fire", GetComponent<AudioSource>(), Camera.main.transform.position + Camera.main.transform.forward);
		}
		else
		{
			MVGameControllerBase.AudioManager.Play("projectile fire", GetComponent<AudioSource>(), muzzlePoint.position);
		}
		isFiring = false;
	}

	private void HandleHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, voxelHit.normal);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
		GameObject original = ((!(worldObjectClient is MVAvatar)) ? PrefabPool.Instance.ParticleSparks : PrefabPool.Instance.ParticleBlood);
		UnityEngine.Object.Instantiate(original, voxelHit.point, rotation);
		MeshDecal.Create(new MeshDecal.Hit(voxelHit.point, voxelHit.normal, 1f), hitDecalMaterial, null);
	}

	private void HandleDirectHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		InteractionData interaction = CenterGunHitPackage.Create();
		MVGameControllerBase.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(voxelHit, interaction.Damage);
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient != null)
		{
			InteractionDataHandlerBase component = worldObjectClient.GameObject.GetComponent<InteractionDataHandlerBase>();
			if (component != null)
			{
				component.HandleInteraction(interaction, interactionIsLocal: false);
			}
		}
	}
}
