using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class PickupItemSlapGun : PickupItemWithDelay
{
	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioClip[] slapSounds;

	public ParticleSystem hitParticles;

	public float maxRange = 50f;

	public float slapStrength = 500f;

	public ImpulseRay impulseRayPrefab;

	public Color slapColor = new Color(128f, 128f, 128f, 128f);

	private int layerMask;

	private static readonly float damage = SlapGunHitPackage.Create(new Vector3(0f, 0f, 0f)).Damage;

	public override AvatarItemType Type => AvatarItemType.SlapGun;

	public override bool CanUnequip => false;

	public override bool ActivateGunModeOnEquip => false;

	public override int Quantity => 0;

	public override bool CanHolster => false;

	private void Awake()
	{
		layerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Player"));
	}

	protected override void OnFire(bool isLocal)
	{
		Ray ray = new Ray(muzzlePoint.position, owner.LookDirection);
		audioSource.clip = slapSounds[Random.Range(0, slapSounds.Length - 1)];
		MVGameControllerBase.AudioManager.Play("Sound - slapGunFire", audioSource, audioSource.transform.position);
		if (owner.IsLocal)
		{
			List<VoxelHit> list = CollisionDetection.MVSphereCastAll(ray, 2f, maxRange, owner.IgnoreWOIDs, layerMask);
			for (int i = 0; i < list.Count; i++)
			{
				VoxelHit voxelHit = list[i];
				MVGameControllerBase.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(voxelHit, slapStrength);
				MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(voxelHit.woId);
				Vector3 impulse = ComputeImpulseDirection(ray) * slapStrength;
				InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
				if (interactionDataHandlerBase != null && !MVGameControllerBase.Game.TeamManager.IsOnSameTeam(worldObjectClient, MVGameControllerBase.Game.LocalPlayer.Avatar))
				{
					interactionDataHandlerBase.HandleInteraction(owner, SlapGunHitPackage.Create(impulse), interactionIsLocal: false);
					if (worldObjectClient is IBulletImpactVisualizer)
					{
						((IBulletImpactVisualizer)worldObjectClient).VisualizeBulletImpact(voxelHit, ray, owner.WorldObjectOwner.OwnerActorNr, damage);
					}
				}
			}
		}
		Vector3 target = FindRayTarget(ray);
		ImpulseRay impulseRay = Object.Instantiate(impulseRayPrefab, muzzlePoint.position, Quaternion.identity) as ImpulseRay;
		impulseRay.Initialize(target);
		impulseRay.radius = 1.2f;
		impulseRay.startColor = slapColor;
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
