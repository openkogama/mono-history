using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class PickupItemBazooka : PickupItemWithDelay
{
	public int ammo = 10;

	public float blastRadius = 10f;

	public AnimationCurve damageFalloff;

	public AnimationCurve impulseFalloff;

	public float baseImpulse = 1500f;

	public float baseDamage = 100f;

	public float rocketSpeed = 30f;

	public float rocketRange = 200f;

	public Bullet rocketPrefab;

	public AudioClip rocketHitSound;

	private int currentAmmo = 10;

	public override AvatarItemType Type => AvatarItemType.Bazooka;

	public override int Quantity => currentAmmo;

	protected override bool IsAmmoDepleted => currentAmmo <= 0;

	protected override void OnStart()
	{
		currentAmmo = ammo;
	}

	public override void OnStateChanged(Hashtable newState)
	{
		currentAmmo = ammo;
	}

	protected override void OnFire(bool isLocal)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		Bullet bullet = Bullet.CreateBullet(rocketPrefab, muzzlePoint.position);
		bullet.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHit, new Bullet.OnHitDelegate(HandleRocketHit));
		if (isLocal)
		{
			bullet.onHitLocal = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHitLocal, new Bullet.OnHitDelegate(HandleRocketHitLocal));
		}
		bullet.Fire(lineOfFire: new Ray(owner.LookOrigin, owner.LookDirection), speed: owner.GetAbsolutProjectileSpeed(rocketSpeed), range: rocketRange, ignoreWoIDs: owner.IgnoreWOIDs);
		MVGameController.Instance.AudioManager.Play("rocket fired", ((Component)this).audio, muzzlePoint.position);
		currentAmmo--;
	}

	private void HandleRocketHitLocal(VoxelHit voxelHit, Ray lineOfFire)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		Collider[] array = Physics.OverlapSphere(voxelHit.point, blastRadius);
		HashSet<int> hashSet = new HashSet<int>();
		Collider[] array2 = array;
		foreach (Collider val in array2)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(((Component)val).transform);
			if (mVObject != null && !hashSet.Contains(mVObject.Id))
			{
				InteractionDataHandlerBase component = mVObject.GameObject.GetComponent<InteractionDataHandlerBase>();
				if ((Object)(object)component != (Object)null)
				{
					float num = Vector3.Distance(voxelHit.point, ((Component)val).transform.position) / blastRadius;
					float damage = damageFalloff.Evaluate(num) * baseDamage;
					Vector3 val2 = ((Component)val).transform.position - voxelHit.point;
					Vector3 normalized = val2.normalized;
					normalized.y += 0.1f;
					normalized.Normalize();
					Vector3 impulse = normalized * baseImpulse * impulseFalloff.Evaluate(num);
					component.HandleInteraction(ProximityDamageAndImpulse.Create(damage, impulse, PlayerKilledByType.BazookaGun), interactionIsLocal: false);
					hashSet.Add(mVObject.Id);
				}
			}
		}
	}

	private void HandleRocketHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		MVGameController.Instance.AudioManager.Play("rocket hit", rocketHitSound, voxelHit.point, 0.4f, SoundRangeDistance.Long);
		Object.Instantiate(Resources.Load("ParticleFX/Explosion"), voxelHit.point, Quaternion.identity);
	}
}
