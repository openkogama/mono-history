using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using MV.WorldObject.RuntimeEvents;
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

	private ObscuredInt currentAmmo = 10;

	public override AvatarItemType Type => AvatarItemType.Bazooka;

	public override int Quantity => currentAmmo;

	protected override bool IsAmmoDepleted => (int)currentAmmo <= 0;

	protected override void OnStart()
	{
		currentAmmo = ammo;
	}

	public override void OnStateChanged(Dictionary<object, object> newState)
	{
		currentAmmo = ammo;
	}

	protected override void OnFire(bool isLocal)
	{
		Debug.Log("Firing isLocal " + Time.frameCount);
		Bullet bullet = Bullet.CreateBullet(rocketPrefab, muzzlePoint.position);
		bullet.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHit, new Bullet.OnHitDelegate(HandleRocketHit));
		if (isLocal)
		{
			bullet.onHitLocal = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHitLocal, new Bullet.OnHitDelegate(HandleRocketHitLocal));
		}
		bullet.Fire(lineOfFire: new Ray(owner.LookOrigin, owner.LookDirection), speed: owner.GetAbsolutProjectileSpeed(rocketSpeed), range: rocketRange, ignoreWoIDs: owner.IgnoreWOIDs);
		MVGameControllerBase.AudioManager.Play("rocket fired", GetComponent<AudioSource>(), muzzlePoint.position);
		if (isLocal)
		{
			MVGameControllerBase.AudioManager.Play("rocket fired", GetComponent<AudioSource>(), Camera.main.transform.position + Camera.main.transform.forward);
		}
		else
		{
			MVGameControllerBase.AudioManager.Play("rocket fired", GetComponent<AudioSource>(), muzzlePoint.position);
		}
		currentAmmo = (int)currentAmmo - 1;
	}

	private void HandleRocketHitLocal(VoxelHit voxelHit, Ray lineOfFire)
	{
		Collider[] array = Physics.OverlapSphere(voxelHit.point, blastRadius);
		HashSet<int> hashSet = new HashSet<int>();
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(collider.transform);
			if (mVObject != null && !hashSet.Contains(mVObject.Id))
			{
				if (mVObject.WorldObjectType == WorldObjectType.CubeModelPrototypeTerrain)
				{
					ExplosionEvent explosion = new ExplosionEvent(RuntimeEventType.Bazooka, voxelHit.point, voxelHit.normal);
					MVGameControllerBase.Game.World.RuntimeEventManager.SendRuntimeEvent(explosion);
				}
				InteractionDataHandlerBase component = mVObject.GameObject.GetComponent<InteractionDataHandlerBase>();
				if (component != null)
				{
					float time = Vector3.Distance(voxelHit.point, collider.transform.position) / blastRadius;
					float damage = damageFalloff.Evaluate(time) * baseDamage;
					Vector3 normalized = (collider.transform.position - voxelHit.point).normalized;
					normalized.y += 0.1f;
					normalized.Normalize();
					Vector3 impulse = normalized * baseImpulse * impulseFalloff.Evaluate(time);
					component.HandleInteraction(ProximityDamageAndImpulse.Create(damage, impulse, PlayerKilledByType.BazookaGun), interactionIsLocal: false);
					hashSet.Add(mVObject.Id);
				}
			}
		}
	}

	private void HandleRocketHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		MVGameControllerBase.AudioManager.Play("rocket hit", rocketHitSound, voxelHit.point, 0.4f, SoundRangeDistance.Long);
		UnityEngine.Object.Instantiate(Resources.Load("ParticleFX/Explosion"), voxelHit.point, Quaternion.identity);
	}
}
