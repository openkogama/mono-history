using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

public class PickupItemBazooka : PickupItemWithDelay
{
	public int ammo = 10;

	public float blastRadius = 10f;

	public AnimationCurve damageFalloff;

	public float baseImpulse = 1500f;

	public float baseDamage = 100f;

	public float rocketSpeed = 30f;

	public float rocketRange = 200f;

	public Bullet rocketPrefab;

	public AudioClip rocketHitSound;

	[SerializeField]
	private AudioSource aSource;

	private int layerMask;

	private ObscuredInt currentAmmo = 10;

	public override AvatarItemType Type => AvatarItemType.Bazooka;

	public override int Quantity => currentAmmo;

	protected override bool IsAmmoDepleted => (int)currentAmmo <= 0;

	private void Awake()
	{
		currentAmmo = ammo;
		layerMask = 1 << LayerUtil.GetLayerNumber(LayerFlags.Player);
	}

	public override void ResetAmmo()
	{
		base.ResetAmmo();
		currentAmmo = ammo;
	}

	protected override void OnFire(bool isLocal)
	{
		Bullet bullet = Bullet.CreateBullet(PoolEnums.BazookaBullet, muzzlePoint.position);
		bullet.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHit, new Bullet.OnHitDelegate(OnHit));
		if (isLocal)
		{
			bullet.onHitLocal = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHitLocal, new Bullet.OnHitDelegate(OnHitLocal));
		}
		bullet.Fire(lineOfFire: new Ray(owner.LookOrigin, owner.LookDirection), speed: owner.GetAbsolutProjectileSpeed(rocketSpeed), range: rocketRange, ignoreWoIDs: owner.IgnoreWOIDs);
		if (isLocal)
		{
			MVGameControllerBase.AudioManager.Play("rocket fired", aSource, Camera.main.transform.position + Camera.main.transform.forward);
		}
		else
		{
			MVGameControllerBase.AudioManager.Play("rocket fired", aSource, muzzlePoint.position);
		}
		currentAmmo = (int)currentAmmo - 1;
	}

	private void OnHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		MVGameControllerBase.AudioManager.Play("rocket hit", rocketHitSound, voxelHit.point, 0.4f, SoundRangeDistance.Long);
		SharedWorldObjectGameplayFunctions.DustEfffect(PrefabPool.Instance.ParticleExplosion, voxelHit.point, 10f);
	}

	private void OnHitLocal(VoxelHit voxelHit, Ray lineOfFire)
	{
		ExplosionEvent explosion = new ExplosionEvent(RuntimeEventType.Bazooka, voxelHit.point, voxelHit.normal);
		MVGameControllerBase.Game.World.RuntimeEventManager.SendRuntimeEvent(explosion);
		int num = Physics.OverlapSphereNonAlloc(voxelHit.point, blastRadius, CollisionDetectionGlobalBuffers.colliderBuffer, layerMask);
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < num; i++)
		{
			Collider collider = CollisionDetectionGlobalBuffers.colliderBuffer[i];
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(collider.transform);
			if (mVObject == null || hashSet.Contains(mVObject.Id))
			{
				continue;
			}
			InteractionDataHandlerBase interactionDataHandlerBase = mVObject.InteractionDataHandlerBase;
			if (interactionDataHandlerBase != null && (mVObject.OwnerActorNr == MVGameControllerBase.Game.LocalPlayer.ActorNr || !MVGameControllerBase.Game.TeamManager.IsOnSameTeam(mVObject.OwnerActorNr, MVGameControllerBase.Game.LocalPlayer.ActorNr)))
			{
				float time = Vector3.Distance(voxelHit.point, interactionDataHandlerBase.GetClosestPoint(voxelHit.point)) / blastRadius;
				float num2 = damageFalloff.Evaluate(time);
				float num3 = Mathf.Clamp(num2 * baseDamage, 0f, float.MaxValue);
				if (num3 > 0f)
				{
					Vector3 normalized = (collider.transform.position - voxelHit.point).normalized;
					normalized.y += 0.1f;
					normalized.Normalize();
					Vector3 impulse = normalized * baseImpulse * num2;
					bool interactionIsLocal = mVObject.OwnerActorNr == MVGameControllerBase.Game.LocalPlayer.ActorNr;
					interactionDataHandlerBase.HandleInteraction(ProximityDamageAndImpulse.Create(num3, impulse, PlayerKilledByType.BazookaGun), interactionIsLocal);
					hashSet.Add(mVObject.Id);
				}
			}
		}
	}
}
