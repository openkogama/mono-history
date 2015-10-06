using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class PickupItemSlapGun : PickupItemWithDelay
{
	public ParticleSystem hitParticles;

	public float maxRange = 50f;

	public float slapStrength = 500f;

	private AudioSource audioSource;

	private static readonly int layerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Player"));

	public AudioClip slapSound1;

	public AudioClip slapSound2;

	public AudioClip slapSound3;

	private AudioClip[] slapSounds;

	public override AvatarItemType Type => AvatarItemType.SlapGun;

	public override bool ActivateGunModeOnEquip => false;

	public override int Quantity => 0;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		slapSounds = new AudioClip[3] { slapSound1, slapSound2, slapSound3 };
	}

	protected override void OnFire(bool isLocal)
	{
		Debug.Log("Slap!");
		Ray lineOfFire = new Ray(owner.LookOrigin, owner.LookDirection);
		audioSource.PlayOneShot(slapSounds[Random.Range(0, 2)]);
		List<MVWorldObjectClient> list = SphereCastAgainstWorldObjects(lineOfFire);
		if (list.Count <= 0 || !owner.IsLocal)
		{
			return;
		}
		foreach (MVWorldObjectClient item in list)
		{
			Vector3 impulse = ComputeImpulseDirection(lineOfFire) * slapStrength;
			InteractionDataHandlerBase component = item.GameObject.GetComponent<InteractionDataHandlerBase>();
			if (component != null)
			{
				component.HandleInteraction(ImpulseHitPackage.Create(impulse), interactionIsLocal: false);
			}
		}
	}

	private List<MVWorldObjectClient> SphereCastAgainstWorldObjects(Ray lineOfFire)
	{
		float magnitude = (lineOfFire.origin - owner.transform.position).magnitude;
		List<MVWorldObjectClient> list = new List<MVWorldObjectClient>();
		List<VoxelHit> list2 = CollisionDetection.MVSphereCastAll(lineOfFire, 2f, maxRange + magnitude, owner.IgnoreWOIDs, layerMask);
		foreach (VoxelHit item in list2)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(item.woId);
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
