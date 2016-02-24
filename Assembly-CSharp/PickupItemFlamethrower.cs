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

	public override void OnStateChanged(Dictionary<object, object> newState)
	{
		currentFuel = fuelAmount;
	}

	private IEnumerator DoFlaming()
	{
		while (IsStillFlaming() && currentFuel > 0f)
		{
			Fire(owner.WorldObjectOwner.Id);
			yield return new WaitForSeconds(0.2f);
		}
	}

	private IEnumerator DoFuelBurn()
	{
		while (IsStillFlaming())
		{
			currentFuel -= burnRate * Time.deltaTime;
			muzzlePoint.transform.forward = owner.LookDirection;
			if (owner.IsLocal)
			{
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
			}
			if (currentFuel < 0f)
			{
				MVEquipable equipable = owner.WorldObjectOwner.GameObject.GetComponent<MVEquipable>();
				if (equipable != null)
				{
					equipable.Unequip();
				}
				yield break;
			}
			yield return 0;
		}
		flameParticles.enableEmission = false;
		audioSource.Stop();
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		if (IsStillFlaming())
		{
			isFlaming = true;
			flamerStartTime = Time.time;
			return;
		}
		flamerStartTime = Time.time;
		flameParticles.enableEmission = true;
		audioSource.Play();
		flameParticles.Play();
		isFlaming = true;
		StartCoroutine(DoFlaming());
		StartCoroutine(DoFuelBurn());
	}

	public override void TriggerEnd()
	{
		isFlaming = false;
	}

	private void Fire(int avatarId)
	{
		List<InteractionDataHandlerBase> list = SphereOverlapAgainsWos();
		if (!owner.IsLocal || list.Count <= 0)
		{
			return;
		}
		foreach (InteractionDataHandlerBase item in list)
		{
			item.HandleInteraction(FlamethrowerHitPackage.Create(), interactionIsLocal: false);
		}
	}

	private List<InteractionDataHandlerBase> SphereOverlapAgainsWos()
	{
		List<InteractionDataHandlerBase> list = new List<InteractionDataHandlerBase>();
		Vector3 vector = hitZoneCenter.position - muzzlePoint.position;
		Ray ray = new Ray(muzzlePoint.position, vector.normalized);
		int layerMask = 1 << LayerMask.NameToLayer("Player");
		List<VoxelHit> list2 = CollisionDetection.MVSphereCastAll(ray, hitRadius, vector.magnitude, owner.IgnoreWOIDs, layerMask);
		foreach (VoxelHit item in list2)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(item.transform);
			if (mVObject != null)
			{
				InteractionDataHandlerBase interactionDataHandlerBase = mVObject.InteractionDataHandlerBase;
				if (interactionDataHandlerBase != null)
				{
					list.Add(interactionDataHandlerBase);
				}
			}
		}
		return list;
	}
}
