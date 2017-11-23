using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

public class PickupItemBazooka : PickupItemWithDelay
{
	[SerializeField]
	private ObscuredInt maxAmmo = 10;

	[SerializeField]
	private ObscuredFloat baseDamage = 75f;

	[SerializeField]
	private float blastRadius = 10f;

	[SerializeField]
	private AnimationCurve damageFalloff;

	[SerializeField]
	private float baseImpulse = 1500f;

	[SerializeField]
	private float rocketSpeed = 30f;

	[SerializeField]
	private float rocketRange = 200f;

	[SerializeField]
	private AudioSource aSource;

	private int layerMask;

	private ObscuredInt currentAmmo;

	public override AvatarItemType Type => AvatarItemType.Bazooka;

	public override int Quantity => currentAmmo;

	protected override bool IsAmmoDepleted => (int)currentAmmo <= 0;

	private void Awake()
	{
		currentAmmo = maxAmmo;
		layerMask = (1 << LayerUtil.GetLayerNumber(LayerFlags.Player)) + (1 << LayerUtil.GetLayerNumber(LayerFlags.Default));
	}

	public override void ResetAmmo()
	{
		base.ResetAmmo();
		currentAmmo = maxAmmo;
	}

	protected override void OnFire(bool isLocal)
	{
		Bullet bullet = Bullet.CreateBullet(PoolEnums.BazookaBullet, muzzlePoint.position);
		bullet.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHit, new Bullet.OnHitDelegate(OnHit));
		if (isLocal)
		{
			bullet.onHitLocal = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHitLocal, new Bullet.OnHitDelegate(OnHitLocal));
			bullet.onOutOfRange = (Action<Ray>)Delegate.Combine(bullet.onOutOfRange, new Action<Ray>(OnHitMaxRangeLocal));
		}
		else
		{
			bullet.onOutOfRange = (Action<Ray>)Delegate.Combine(bullet.onOutOfRange, new Action<Ray>(OnHitMaxRangeRemote));
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
			if (!(interactionDataHandlerBase != null) || (mVObject.OwnerActorNr != MVGameControllerBase.Game.LocalPlayer.ActorNr && MVGameControllerBase.Game.TeamManager.IsOnSameTeam(mVObject, MVGameControllerBase.Game.LocalPlayer.Avatar)))
			{
				continue;
			}
			float time = Vector3.Distance(voxelHit.point, interactionDataHandlerBase.GetClosestPoint(voxelHit.point)) / blastRadius;
			float num2 = damageFalloff.Evaluate(time);
			float num3 = Mathf.Clamp(num2 * (float)baseDamage, 0f, float.MaxValue);
			if (num3 > 0f)
			{
				Vector3 normalized = (collider.transform.position - voxelHit.point).normalized;
				normalized.y += 0.1f;
				normalized.Normalize();
				Vector3 impulse = normalized * baseImpulse * num2;
				bool interactionIsLocal = mVObject.OwnerActorNr == MVGameControllerBase.Game.LocalPlayer.ActorNr;
				interactionDataHandlerBase.HandleInteraction(owner, ProximityDamageAndImpulse.Create(num3, impulse, PlayerKilledByType.BazookaGun), interactionIsLocal);
				if (mVObject is IBulletImpactVisualizer)
				{
					((IBulletImpactVisualizer)mVObject).VisualizeBulletImpact(default, lineOfFire, owner.WorldObjectOwner.OwnerActorNr, 0f);
				}
				hashSet.Add(mVObject.Id);
			}
		}
	}

	private void OnHitMaxRangeRemote(Ray lineOfFire)
	{
		OnHit(new VoxelHit
		{
			point = lineOfFire.origin + lineOfFire.direction * rocketRange,
			normal = -lineOfFire.direction
		}, lineOfFire);
	}

	private void OnHitMaxRangeLocal(Ray lineOfFire)
	{
		VoxelHit voxelHit = default;
		if (rocketRange > Camera.main.farClipPlane)
		{
			voxelHit.point = lineOfFire.origin + lineOfFire.direction * (Camera.main.farClipPlane * 0.95f);
		}
		else
		{
			voxelHit.point = lineOfFire.origin + lineOfFire.direction * rocketRange;
		}
		voxelHit.normal = -lineOfFire.direction;
		OnHitLocal(voxelHit, lineOfFire);
		SharedWorldObjectGameplayFunctions.DustEfffect(PrefabPool.Instance.ParticleExplosion, voxelHit.point, 10f);
	}
}
