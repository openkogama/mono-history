using System;
using System.Collections;
using MV.Common;
using UnityEngine;

public class PickupItemShotgun : PickupItemWithDelay
{
	public ShotgunShot shotgunShotPrefab;

	public Bullet bulletPrefab;

	public Material hitDecalMaterial;

	public ParticleSystem muzzleFlare;

	public int ammo;

	public float spread = 0.1f;

	public float hitDamage = 34f;

	public float impulseStrength = 700f;

	public float maxRange = 50f;

	public override AvatarItemType Type => AvatarItemType.Shotgun;

	public override int Quantity => ammo;

	protected override bool IsAmmoDepleted => ammo <= 0;

	private void Start()
	{
		meshRenderers = ((Component)this).GetComponentsInChildren<MeshRenderer>();
	}

	public override void OnStateChanged(Hashtable newState)
	{
		ammo = 24;
	}

	protected override void OnFire(bool isLocal)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.LookRotation(owner.LookDirection, Vector3.up);
		Vector3 val2 = val * Vector3.right;
		Vector3 val3 = val * Vector3.up;
		float[] array = new float[5] { -1f, -1f, 0f, 1f, 1f };
		float[] array2 = new float[5] { -1f, 1f, 0f, -1f, 1f };
		muzzleFlare.Play();
		Ray[] array3 = new Ray[5];
		for (int i = 0; i < 5; i++)
		{
			ref Ray reference = ref array3[i];
			reference = new Ray(owner.LookOrigin, owner.LookDirection + val2 * array[i] * spread + val3 * array2[i] * spread);
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
		ammo--;
		MVGameController.Instance.AudioManager.Play("shotgun fire", ((Component)this).audio, muzzlePoint.position);
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
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		int woIDHighestInHierarchyWithComponent = MVGameController.Instance.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient == null)
		{
			return;
		}
		InteractionDataHandlerBase component = worldObjectClient.GameObject.GetComponent<InteractionDataHandlerBase>();
		if (!((Object)(object)component == (Object)null))
		{
			Vector3 impulse = lineOfFire.direction * impulseStrength;
			if ((Object)(object)component != (Object)null)
			{
				component.HandleInteraction(ShotgunHitPackage.Create(impulse, hitDamage), interactionIsLocal: false);
			}
		}
	}
}
