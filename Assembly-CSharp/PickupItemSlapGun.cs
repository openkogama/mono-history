using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class PickupItemSlapGun : PickupItemWithDelay
{
	private int layerMask;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioClip[] slapSounds;

	public ParticleSystem hitParticles;

	public float maxRange = 50f;

	public float slapStrength = 500f;

	public ImpulseRay impulseRayPrefab;

	public Color slapColor = new Color(128f, 128f, 128f, 128f);

	public override AvatarItemType Type => AvatarItemType.SlapGun;

	public override bool CanUnequip => false;

	public override bool ActivateGunModeOnEquip => false;

	public override int Quantity => 0;

	private void Awake()
	{
		layerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Player"));
	}

	protected override void OnFire(bool isLocal)
	{
		Ray lineOfFire = new Ray(muzzlePoint.position, owner.LookDirection);
		if (audioSource.gameObject.activeInHierarchy)
		{
			audioSource.PlayOneShot(slapSounds[Random.Range(0, 2)]);
		}
		List<MVWorldObjectClient> list = SphereCastAgainstWorldObjects(lineOfFire);
		if (list.Count > 0 && owner.IsLocal)
		{
			foreach (MVWorldObjectClient item in list)
			{
				Vector3 impulse = ComputeImpulseDirection(lineOfFire) * slapStrength;
				InteractionDataHandlerBase interactionDataHandlerBase = item.InteractionDataHandlerBase;
				if (interactionDataHandlerBase != null)
				{
					interactionDataHandlerBase.HandleInteraction(ImpulseHitPackage.Create(impulse), interactionIsLocal: false);
				}
			}
		}
		Vector3 target = FindRayTarget(lineOfFire);
		ImpulseRay impulseRay = Object.Instantiate(impulseRayPrefab, muzzlePoint.position, Quaternion.identity) as ImpulseRay;
		impulseRay.target = target;
		impulseRay.radius = 1.2f;
		impulseRay.startColor = slapColor;
	}

	private List<MVWorldObjectClient> SphereCastAgainstWorldObjects(Ray lineOfFire)
	{
		List<MVWorldObjectClient> list = new List<MVWorldObjectClient>();
		List<VoxelHit> list2 = CollisionDetection.MVSphereCastAll(lineOfFire, 2f, maxRange, owner.IgnoreWOIDs, layerMask);
		foreach (VoxelHit item in list2)
		{
			MVGameControllerBase.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(item, slapStrength);
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(item.woId);
			if (worldObjectClient != null)
			{
				list.Add(worldObjectClient);
			}
		}
		return list;
	}

	private Vector3 FindRayTarget(Ray lineOfFire)
	{
		VoxelHit voxelHit;
		return (!CollisionDetection.MVHit(lineOfFire, out voxelHit, maxRange, new HashSet<int>(), 1 << LayerMask.NameToLayer("Default"))) ? lineOfFire.GetPoint(maxRange) : voxelHit.point;
	}

	private Vector3 ComputeImpulseDirection(Ray lineOfFire)
	{
		Vector3 direction = lineOfFire.direction;
		direction.y += 0.2f;
		return direction.normalized;
	}
}
