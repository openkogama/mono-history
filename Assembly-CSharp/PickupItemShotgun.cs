using System;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class PickupItemShotgun : PickupItemWithDelay
{
	[SerializeField]
	private ParticleSystem muzzleFlare;

	[SerializeField]
	private ObscuredInt maxAmmo = 24;

	[SerializeField]
	private float spread = 0.1f;

	[SerializeField]
	private float impulseStrength = 700f;

	[SerializeField]
	private float maxRange = 50f;

	[SerializeField]
	private float bulletSpeed = 100f;

	[SerializeField]
	private AudioSource audioSource;

	private static readonly float hitDamage = ShotgunHitPackage.Create(new Vector3(0f, 0f, 0f)).Damage;

	private static readonly float[] offsetsX = new float[5] { -1f, -1f, 0f, 1f, 1f };

	private static readonly float[] offsetsY = new float[5] { -1f, 1f, 0f, -1f, 1f };

	private ObscuredInt currentAmmo;

	public override AvatarItemType Type => AvatarItemType.Shotgun;

	public override int Quantity => currentAmmo;

	protected override bool IsAmmoDepleted => (int)currentAmmo <= 0;

	private void Awake()
	{
		currentAmmo = maxAmmo;
	}

	public override void ResetAmmo()
	{
		base.ResetAmmo();
		currentAmmo = maxAmmo;
	}

	protected override void OnFire(bool isLocal)
	{
		Quaternion quaternion = Quaternion.LookRotation(owner.LookDirection, Vector3.up);
		Vector3 vector = quaternion * Vector3.right;
		Vector3 vector2 = quaternion * Vector3.up;
		muzzleFlare.Play();
		for (int i = 0; i < 5; i++)
		{
			Ray lineOfFire = new Ray(owner.LookOrigin, owner.LookDirection + vector * offsetsX[i] * spread + vector2 * offsetsY[i] * spread);
			Bullet bullet = Bullet.CreateBullet(PoolEnums.ShotgunBullet, muzzlePoint.position);
			bullet.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHit, new Bullet.OnHitDelegate(OnBulletHit));
			if (isLocal)
			{
				bullet.onHitLocal = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHitLocal, new Bullet.OnHitDelegate(OnLocalBulletHit));
			}
			bullet.Fire(owner.GetAbsolutProjectileSpeed(bulletSpeed), maxRange, lineOfFire, owner.IgnoreWOIDs);
		}
		--currentAmmo;
		if (isLocal)
		{
			MVGameControllerBase.AudioManager.Play("shotgun fire", audioSource, Camera.main.transform.position + Camera.main.transform.forward);
		}
		else
		{
			MVGameControllerBase.AudioManager.Play("shotgun fire", audioSource, muzzlePoint.position);
		}
		isFiring = false;
	}

	private void OnBulletHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient is IBulletImpactVisualizer)
		{
			((IBulletImpactVisualizer)worldObjectClient).VisualizeBulletImpact(voxelHit, lineOfFire, owner.WorldObjectOwner.OwnerActorNr, hitDamage);
		}
		else
		{
			OneShotPooledParticleSystem.Instantiate(PoolEnums.NormalBulletSparks, voxelHit.point, Quaternion.LookRotation(voxelHit.normal));
		}
	}

	private void OnLocalBulletHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		MVGameControllerBase.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(voxelHit, hitDamage);
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient != null)
		{
			InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
			if (interactionDataHandlerBase != null && !MVGameControllerBase.Game.TeamManager.IsOnSameTeam(worldObjectClient.OwnerActorNr, MVGameControllerBase.Game.LocalPlayer.ActorNr))
			{
				Vector3 impulse = lineOfFire.direction * impulseStrength;
				InteractionData interaction = ShotgunHitPackage.Create(impulse);
				interactionDataHandlerBase.HandleInteraction(interaction, interactionIsLocal: false);
			}
		}
	}
}
