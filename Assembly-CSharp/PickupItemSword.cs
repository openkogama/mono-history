using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

public class PickupItemSword : PickupItemWithDelay
{
	[SerializeField]
	private Transform swordHandle;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	[Tooltip("Impulse delivered to enemy on hit.")]
	private float impulseStrength = 500f;

	[SerializeField]
	private Animation swordAnim;

	[SerializeField]
	private float recoilForce = 700f;

	[SerializeField]
	private float bladeRadius = 3f;

	[SerializeField]
	private float range = 1f;

	private int hitLayerMask;

	private static readonly float hitDamage = SwordHitPackage.Create(new Vector3(0f, 0f, 0f)).Damage;

	public override AvatarItemType Type => AvatarItemType.Sword;

	public override bool ActivateGunModeOnEquip => false;

	public override int Quantity => 0;

	private void Awake()
	{
		hitLayerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Player"));
	}

	public override void OnEquip()
	{
		base.OnEquip();
	}

	protected override void OnFire(bool isLocal)
	{
		swordAnim.Play();
		isFiring = false;
		Ray ray = new Ray(swordHandle.position - owner.LookDirection * bladeRadius, owner.LookDirection);
		List<VoxelHit> list = CollisionDetection.MVSphereCastAll(ray, bladeRadius, range + bladeRadius, owner.IgnoreWOIDs, hitLayerMask);
		if (list.Count > 0)
		{
			OnSwordHit(list, ray);
			if (isLocal)
			{
				OnLocalSwordHit(list, ray);
			}
		}
		if (isLocal)
		{
			MVGameControllerBase.AudioManager.Play("sword swing", audioSource, Camera.main.transform.position + Camera.main.transform.forward);
		}
		else
		{
			MVGameControllerBase.AudioManager.Play("sword swing", audioSource, muzzlePoint.position);
		}
	}

	private void OnSwordHit(List<VoxelHit> voxelHits, Ray lineOfFire)
	{
		OneShotPooledParticleSystem.Instantiate(PoolEnums.NormalBulletSparks, voxelHits[0].point, Quaternion.LookRotation(voxelHits[0].normal));
		for (int i = 0; i < voxelHits.Count; i++)
		{
			OnSwordHit(voxelHits[i], lineOfFire);
		}
	}

	private void OnSwordHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient is IBulletImpactVisualizer)
		{
			((IBulletImpactVisualizer)worldObjectClient).VisualizeBulletImpact(voxelHit, lineOfFire, owner.WorldObjectOwner.OwnerActorNr, hitDamage);
		}
	}

	private void OnLocalSwordHit(List<VoxelHit> voxelHits, Ray lineOfFire)
	{
		MVGameControllerBase.Game.World.RuntimeEventManager.SendRuntimeEvent(new ExplosionEvent(RuntimeEventType.SwordTerrainDestroy, voxelHits[0].point, voxelHits[0].normal));
		for (int i = 0; i < voxelHits.Count; i++)
		{
			OnLocalSwordHit(voxelHits[i], lineOfFire);
		}
	}

	private Vector3 FindRayTarget(Ray lineOfFire)
	{
		VoxelHit voxelHit;
		return (!CollisionDetection.MVHit(lineOfFire, out voxelHit, range, null, hitLayerMask)) ? lineOfFire.GetPoint(range) : voxelHit.point;
	}

	private void OnLocalSwordHit(VoxelHit voxelHit, Ray lineOfFire)
	{
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (worldObjectClient == null)
		{
			return;
		}
		InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
		if (interactionDataHandlerBase != null && !MVGameControllerBase.Game.TeamManager.IsOnSameTeam(worldObjectClient.OwnerActorNr, MVGameControllerBase.Game.LocalPlayer.ActorNr))
		{
			Vector3 lookDirection = owner.LookDirection;
			lookDirection.y = 0.02f;
			lookDirection.Normalize();
			Vector3 impulse = lookDirection * impulseStrength;
			interactionDataHandlerBase.HandleInteraction(SwordHitPackage.Create(impulse), interactionIsLocal: false);
			MVRigidBody component = owner.GetComponent<MVRigidBody>();
			if (component != null)
			{
				component.AddImpulse(-lookDirection * recoilForce);
			}
		}
	}

	public override void UpdateWithDirection(Vector3 dir)
	{
	}
}
