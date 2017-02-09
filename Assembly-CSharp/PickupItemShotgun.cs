using System;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class PickupItemShotgun : PickupItemWithDelay
{
	public ShotgunShot shotgunShotPrefab;

	public Bullet bulletPrefab;

	public Material hitDecalMaterial;

	public ParticleSystem muzzleFlare;

	public ObscuredInt ammo;

	public float spread = 0.1f;

	public float hitDamage = 34f;

	public float impulseStrength = 700f;

	public float maxRange = 50f;

	public float fireRate = 1f;

	public float bulletSpeed = 100f;

	[SerializeField]
	private AudioSource audioSource;

	private static readonly float[] offsetsX = new float[5] { -1f, -1f, 0f, 1f, 1f };

	private static readonly float[] offsetsY = new float[5] { -1f, 1f, 0f, -1f, 1f };

	public override AvatarItemType Type => AvatarItemType.Shotgun;

	public override int Quantity => ammo;

	protected override bool IsAmmoDepleted => (int)ammo <= 0;

	private void Awake()
	{
		fireInterval = fireRate;
	}

	public override void ResetAmmo()
	{
		base.ResetAmmo();
		ammo = 24;
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
		--ammo;
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
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
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
