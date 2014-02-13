using System;
using System.Collections;
using MV.Common;
using UnityEngine;

public class PickupItemCenterGun : PickupItemWithDelay
{
	public int ammo;

	public Material hitDecalMaterial;

	public Bullet bulletPrefab;

	public AudioClip bulletHitSound;

	public override AvatarItemType Type => AvatarItemType.CenterGun;

	public override int Quantity => ammo;

	protected override bool IsAmmoDepleted => ammo <= 0;

	public override void OnStateChanged(Hashtable newState)
	{
		ammo = 100;
	}

	protected override void OnFire(bool isLocal)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		Bullet bullet = Bullet.CreateBullet(bulletPrefab, muzzlePoint.position);
		Ray lineOfFire = new Ray(owner.LookOrigin, owner.LookDirection);
		bullet.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHit, new Bullet.OnHitDelegate(HandleHit));
		if (isLocal)
		{
			bullet.onHitLocal = HandleDirectHit;
		}
		bullet.Fire(owner.GetAbsolutProjectileSpeed(70f), 100f, lineOfFire, owner.IgnoreWOIDs);
		ammo--;
		MVGameController.Instance.AudioManager.Play("projectile fire", ((Component)this).audio, muzzlePoint.position);
		isFiring = false;
	}

	private void HandleHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.FromToRotation(Vector3.up, voxelHit.normal);
		MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(voxelHit.woId);
		string text = ((!(worldObjectClient is MVAvatar)) ? "ParticleFX/Sparks" : "ParticleFX/Blood");
		Object.Instantiate(Resources.Load(text), voxelHit.point, val);
		MeshDecal.Create(new MeshDecal.Hit(voxelHit.point, voxelHit.normal, 1f), hitDecalMaterial, null);
	}

	private void HandleDirectHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		int woIDHighestInHierarchyWithComponent = MVGameController.Instance.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient != null)
		{
			InteractionDataHandlerBase component = worldObjectClient.GameObject.GetComponent<InteractionDataHandlerBase>();
			if ((Object)(object)component != (Object)null)
			{
				component.HandleInteraction(CenterGunHitPackage.Create(), interactionIsLocal: false);
			}
		}
	}
}
