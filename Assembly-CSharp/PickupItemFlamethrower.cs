using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using UnityEngine;

public class PickupItemFlamethrower : PickupItem
{
	[SerializeField]
	private ParticleSystem flameParticles;

	[SerializeField]
	private float hitRadius = 1.2f;

	[SerializeField]
	[Tooltip("How many seconds a fueltank lasts.")]
	private ObscuredFloat maxFuelTime = 100f;

	[SerializeField]
	private float maxRange = 50f;

	[SerializeField]
	private float flamerMinimumBurnTime = 0.5f;

	[SerializeField]
	private AudioSource audioSource;

	private float flamerStartTime;

	private bool isFlaming;

	private ObscuredFloat currentFuel = 0f;

	public override int MaxQuantity => 100;

	public override AvatarItemType Type => AvatarItemType.Flamethrower;

	public override int Quantity => Mathf.RoundToInt((float)currentFuel / (float)maxFuelTime * 100f);

	private bool IsStillFlaming()
	{
		return isFlaming || flamerStartTime + flamerMinimumBurnTime >= Time.time;
	}

	private void Awake()
	{
		currentFuel = maxFuelTime;
	}

	public override void ResetAmmo()
	{
		base.ResetAmmo();
		currentFuel = maxFuelTime;
	}

	private IEnumerator DoFlaming()
	{
		while (IsStillFlaming() && (float)currentFuel > 0f)
		{
			Ray lineofFire = new Ray(muzzlePoint.position, owner.LookDirection);
			List<VoxelHit> hits = CollisionDetection.MVSphereCastAll(layerMask: 1 << LayerMask.NameToLayer("Player"), ray: lineofFire, radius: hitRadius, distance: maxRange, ignoreWoIds: owner.IgnoreWOIDs);
			for (int i = 0; i < hits.Count; i++)
			{
				MVWorldObjectClient hitObject = MVWorldObjectClientManager.GetMVObject(hits[i].transform);
				if (hitObject == null || MVGameControllerBase.Game.TeamManager.IsOnSameTeam(hitObject, MVGameControllerBase.Game.LocalPlayer.Avatar))
				{
					continue;
				}
				InteractionDataHandlerBase interactionHandler = hitObject.InteractionDataHandlerBase;
				if (interactionHandler != null)
				{
					interactionHandler.HandleInteraction(owner, FlamethrowerHitPackage.Create(), interactionIsLocal: false);
					if (hitObject is IBulletImpactVisualizer)
					{
						((IBulletImpactVisualizer)hitObject).VisualizeBulletImpact(hits[i], lineofFire, owner.WorldObjectOwner.OwnerActorNr, 0f);
					}
				}
			}
			yield return new WaitForSeconds(0.2f);
		}
	}

	private IEnumerator DoFuelBurn()
	{
		while (IsStillFlaming())
		{
			PickupItemFlamethrower pickupItemFlamethrower = this;
			pickupItemFlamethrower.currentFuel = (float)pickupItemFlamethrower.currentFuel - Time.deltaTime;
			muzzlePoint.transform.forward = owner.LookDirection;
			MVRigidBody mvRigidBody = owner.WorldObjectOwner.GameObject.GetComponent<MVRigidBody>();
			if (!mvRigidBody.Grounded)
			{
				float verticalVelocity = mvRigidBody.Velocity.y;
				if (verticalVelocity < 0f)
				{
					float flamerImpulse = owner.LookDirection.y * (float)MVPhysics.Gravity * 0.95f * Time.deltaTime * 40f;
					float impulseY = Mathf.Min(flamerImpulse, verticalVelocity);
					mvRigidBody.AddImpulse(new Vector3(0f, 0f - impulseY, 0f), suspendImpactDamage: true);
				}
			}
			if ((float)currentFuel <= 0f)
			{
				MVEquipable equipable = owner.WorldObjectOwner.GameObject.GetComponent<MVEquipable>();
				if (equipable != null)
				{
					equipable.Unequip();
				}
				break;
			}
			yield return 0;
		}
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		ParticleSystem.EmissionModule emission = flameParticles.emission;
		if (IsStillFlaming())
		{
			isFlaming = true;
			flamerStartTime = Time.time;
			emission.enabled = true;
			isFlaming = true;
			return;
		}
		flamerStartTime = Time.time;
		emission.enabled = true;
		isFlaming = true;
		if (owner.IsLocal)
		{
			StartCoroutine(DoFlaming());
			StartCoroutine(DoFuelBurn());
		}
	}

	public override void TriggerEnd()
	{
		isFlaming = false;
		ParticleSystem.EmissionModule emission = flameParticles.emission;
		emission.enabled = false;
		audioSource.Stop();
	}

	private void Update()
	{
		if (isFlaming)
		{
			if (!audioSource.isPlaying)
			{
				audioSource.Play();
			}
			if (!flameParticles.isPlaying)
			{
				flameParticles.Play();
			}
			if (!owner.IsLocal)
			{
				flameParticles.transform.rotation = Quaternion.LookRotation(owner.LookDirection);
			}
		}
	}
}
