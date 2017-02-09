using System.Collections;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PickupItemFlamethrower : PickupItem
{
	private const bool DEAL_DAMAGE_REMOTELY = true;

	public ParticleSystem flameParticles;

	public Transform hitZoneCenter;

	public float hitRadius = 1.2f;

	public float fuelAmount = 100f;

	public float burnRate = 10f;

	public float maxRange = 50f;

	private bool isFlaming;

	private float currentFuel;

	private float flamerStartTime;

	private float flamerMinimumBurnTime = 0.5f;

	[SerializeField]
	private AudioSource audioSource;

	public override AvatarItemType Type => AvatarItemType.Flamethrower;

	public override int Quantity => Mathf.RoundToInt(currentFuel);

	private bool IsStillFlaming()
	{
		return isFlaming || flamerStartTime + flamerMinimumBurnTime >= Time.time;
	}

	private void Awake()
	{
		currentFuel = fuelAmount;
	}

	public override void ResetAmmo()
	{
		base.ResetAmmo();
		currentFuel = fuelAmount;
	}

	private IEnumerator DoFlaming()
	{
		while (IsStillFlaming() && currentFuel > 0f)
		{
			Fire();
			yield return new WaitForSeconds(0.2f);
		}
	}

	private IEnumerator DoFuelBurn()
	{
		while (IsStillFlaming())
		{
			currentFuel -= burnRate * Time.deltaTime;
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
			if (currentFuel < 0f)
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

	private void Fire()
	{
		Ray ray = new Ray(muzzlePoint.position, owner.LookDirection);
		int layerMask = 1 << LayerMask.NameToLayer("Player");
		List<VoxelHit> list = CollisionDetection.MVSphereCastAll(ray, hitRadius, maxRange, owner.IgnoreWOIDs, layerMask);
		for (int i = 0; i < list.Count; i++)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(list[i].transform);
			if (mVObject != null && !MVGameControllerBase.Game.TeamManager.IsOnSameTeam(mVObject.OwnerActorNr, MVGameControllerBase.Game.LocalPlayer.ActorNr))
			{
				InteractionDataHandlerBase interactionDataHandlerBase = mVObject.InteractionDataHandlerBase;
				if (interactionDataHandlerBase != null)
				{
					interactionDataHandlerBase.HandleInteraction(FlamethrowerHitPackage.Create(), interactionIsLocal: false);
				}
			}
		}
	}
}
