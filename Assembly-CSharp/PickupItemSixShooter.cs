using System;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class PickupItemSixShooter : PickupItemWithDelay
{
	[SerializeField]
	private ObscuredInt maxAmmo;

	[SerializeField]
	private float recoilImpact = 700f;

	[SerializeField]
	private float bulletRange = 50f;

	[SerializeField]
	private float bulletSpeed = 80f;

	[SerializeField]
	private float hitImpact = 300f;

	[SerializeField]
	private AudioSource fireSound;

	[SerializeField]
	private ParticleEmitter fireEmitter;

	public Animation animComponent;

	private ObscuredInt currentAmmo;

	private static readonly float baseDamage = SixShooterHitPackage.Create(new Vector3(0f, 0f, 0f)).Damage;

	public override AvatarItemType Type => AvatarItemType.SixShooter;

	public override int Quantity => currentAmmo;

	protected override bool IsAmmoDepleted => (int)currentAmmo <= 0;

	public void Awake()
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
		animComponent.Play("RevolverRecoil");
		Bullet bullet = Bullet.CreateBullet(PoolEnums.SixShooterBullet, muzzlePoint.position);
		bullet.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHit, new Bullet.OnHitDelegate(OnBulletHit));
		if (isLocal)
		{
			bullet.onHitLocal = OnLocalBulletHit;
		}
		bullet.Fire(lineOfFire: new Ray(owner.LookOrigin, owner.LookDirection), speed: owner.GetAbsolutProjectileSpeed(bulletSpeed), range: bulletRange, ignoreWoIDs: owner.IgnoreWOIDs);
		--currentAmmo;
		UnityEngine.Object.Instantiate(fireEmitter, muzzlePoint.position, Quaternion.identity);
		if (isLocal)
		{
			MVGameControllerBase.AudioManager.Play("projectile fire", fireSound, Camera.main.transform.position + Camera.main.transform.forward);
		}
		else
		{
			MVGameControllerBase.AudioManager.Play("projectile fire", fireSound, transform.position);
		}
		isFiring = false;
		MVRigidBody component = owner.GetComponent<MVRigidBody>();
		if (component != null)
		{
			component.AddImpulse(-owner.LookDirection * recoilImpact);
		}
	}

	private void OnBulletHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient is IBulletImpactVisualizer)
		{
			((IBulletImpactVisualizer)worldObjectClient).VisualizeBulletImpact(voxelHit, lineOfFire, owner.WorldObjectOwner.OwnerActorNr, baseDamage);
		}
		else
		{
			OneShotPooledParticleSystem.Instantiate(PoolEnums.SixShooterSparks, voxelHit.point, Quaternion.LookRotation(voxelHit.normal));
		}
	}

	private void OnLocalBulletHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		MVGameControllerBase.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(voxelHit, baseDamage);
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient != null)
		{
			InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
			if (interactionDataHandlerBase != null && !MVGameControllerBase.Game.TeamManager.IsOnSameTeam(worldObjectClient.OwnerActorNr, MVGameControllerBase.Game.LocalPlayer.ActorNr))
			{
				Vector3 value = voxelHit.point - owner.transform.position;
				value = Vector3.Normalize(value);
				InteractionData interaction = SixShooterHitPackage.Create(value * hitImpact);
				interactionDataHandlerBase.HandleInteraction(interaction, interactionIsLocal: false);
			}
		}
	}
}
