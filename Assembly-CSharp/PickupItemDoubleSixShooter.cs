using System;
using System.Collections;
using MV.Common;
using UnityEngine;

public class PickupItemDoubleSixShooter : PickupItemWithDelay
{
	public int ammo;

	public Material hitDecalMaterial;

	public Bullet bulletPrefab;

	public AudioClip bulletHitSound;

	public float recoilImpact = 700f;

	public AnimationCurve damageFalloff;

	public float baseDamage = 30f;

	public float rangeDamage = 70f;

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

	protected override bool IsAmmoDepleted => ammo <= 0;

	public override void OnStateChanged(Hashtable newState)
	{
		ammo = 12;
	}

	protected override void OnFire(bool isLocal)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		Bullet bullet = null;
		bullet = ((ammo % 2 != 0) ? Bullet.CreateBullet(bulletPrefab, muzzlePoint2.position) : Bullet.CreateBullet(bulletPrefab, muzzlePoint.position));
		if (ammo % 2 == 0)
		{
			Object.Instantiate((Object)(object)fireEmitter, muzzlePoint.position, Quaternion.identity);
			animComponentL.Play("RevolverRecoil");
		}
		else
		{
			Object.Instantiate((Object)(object)fireEmitter, muzzlePoint2.position, Quaternion.identity);
			animComponentR.Play("RevolverRecoil");
		}
		Ray lineOfFire = new Ray(owner.LookOrigin, owner.LookDirection);
		Bullet bullet2 = bullet;
		bullet2.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet2.onHit, new Bullet.OnHitDelegate(HandleHit));
		if (isLocal)
		{
			bullet.onHitLocal = HandleDirectHit;
		}
		bullet.Fire(owner.GetAbsolutProjectileSpeed(bulletSpeed), bulletRange, lineOfFire, owner.IgnoreWOIDs);
		ammo--;
		MVGameController.Instance.AudioManager.Play("projectile fire", fireSoundClip, muzzlePoint.position, 0.28f, SoundRangeDistance.Long);
		isFiring = false;
		MVRigidBody component = ((Component)owner).GetComponent<MVRigidBody>();
		if ((Object)(object)component != (Object)null)
		{
			component.AddImpulse(-((Component)this).transform.forward * recoilImpact);
		}
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
		string text = ((!(worldObjectClient is MVAvatar)) ? "ParticleFX/SparksSixShooter" : "ParticleFX/BloodSixShooter");
		Object.Instantiate(Resources.Load(text), voxelHit.point, val);
		MeshDecal.Create(new MeshDecal.Hit(voxelHit.point, voxelHit.normal, 1f), hitDecalMaterial, null);
	}

	private void HandleDirectHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		int woIDHighestInHierarchyWithComponent = MVGameController.Instance.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient != null)
		{
			float num = Vector3.Distance(voxelHit.point, ((Component)owner).transform.position);
			float num2 = num / bulletRange;
			float damage = damageFalloff.Evaluate(num2) * rangeDamage + baseDamage;
			InteractionDataHandlerBase component = worldObjectClient.GameObject.GetComponent<InteractionDataHandlerBase>();
			if ((Object)(object)component != (Object)null)
			{
				Vector3 val = voxelHit.point - ((Component)owner).transform.position;
				val = Vector3.Normalize(val);
				component.HandleInteraction(SixShooterHitPackage.Create(val * hitImpact, damage), interactionIsLocal: false);
			}
		}
	}
}
