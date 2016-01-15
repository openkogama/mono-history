using System;
using System.Collections.Generic;
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

	private AudioSource audioSource;

	public override AvatarItemType Type => AvatarItemType.Shotgun;

	public override int Quantity => ammo;

	protected override bool IsAmmoDepleted => (int)ammo <= 0;

	private void Start()
	{
		meshRenderers = GetComponentsInChildren<MeshRenderer>();
		audioSource = GetComponent<AudioSource>();
	}

	public override void OnStateChanged(Dictionary<object, object> newState)
	{
		ammo = 24;
	}

	protected override void OnFire(bool isLocal)
	{
		Quaternion quaternion = Quaternion.LookRotation(owner.LookDirection, Vector3.up);
		Vector3 vector = quaternion * Vector3.right;
		Vector3 vector2 = quaternion * Vector3.up;
		float[] array = new float[5] { -1f, -1f, 0f, 1f, 1f };
		float[] array2 = new float[5] { -1f, 1f, 0f, -1f, 1f };
		muzzleFlare.Play();
		Ray[] array3 = new Ray[5];
		for (int i = 0; i < 5; i++)
		{
			ref Ray reference = ref array3[i];
			reference = new Ray(owner.LookOrigin, owner.LookDirection + vector * array[i] * spread + vector2 * array2[i] * spread);
		}
		for (int j = 0; j < 5; j++)
		{
			Bullet bullet = Bullet.CreateBullet(bulletPrefab, muzzlePoint.position);
			bullet.onHit = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHit, new Bullet.OnHitDelegate(HandleHit));
			if (isLocal)
			{
				bullet.onHitLocal = (Bullet.OnHitDelegate)Delegate.Combine(bullet.onHitLocal, new Bullet.OnHitDelegate(HandleDirectHit));
			}
			bullet.Fire(owner.GetAbsolutProjectileSpeed(100f), 100f, array3[j], owner.IgnoreWOIDs);
		}
		--ammo;
		MVGameControllerBase.AudioManager.Play("shotgun fire", audioSource, muzzlePoint.position);
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
		Vector3 impulse = lineOfFire.direction * impulseStrength;
		InteractionData interaction = ShotgunHitPackage.Create(impulse, hitDamage);
		MVGameControllerBase.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(voxelHit, interaction.Damage);
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient != null)
		{
			InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
			if (!(interactionDataHandlerBase == null) && interactionDataHandlerBase != null)
			{
				interactionDataHandlerBase.HandleInteraction(interaction, interactionIsLocal: false);
			}
		}
	}
}
