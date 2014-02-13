using System.Collections;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PickupItemFlamethrower : PickupItem
{
	private const bool DEAL_DAMAGE_REMOTELY = true;

	public Transform muzzlePoint;

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

	public override AvatarItemType Type => AvatarItemType.Flamethrower;

	public override int Quantity => Mathf.RoundToInt(currentFuel);

	private bool IsStillFlaming()
	{
		return isFlaming || flamerStartTime + flamerMinimumBurnTime >= Time.time;
	}

	private void Start()
	{
		meshRenderers = ((Component)this).GetComponentsInChildren<MeshRenderer>();
		currentFuel = fuelAmount;
	}

	private void OnDrawGizmos()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.DrawWireSphere(((Component)hitZoneCenter).transform.position, hitRadius);
	}

	public override void OnStateChanged(Hashtable newState)
	{
		currentFuel = fuelAmount;
	}

	private IEnumerator DoFlaming()
	{
		while (IsStillFlaming() && currentFuel > 0f)
		{
			Fire(owner.WorldObjectOwner.Id);
			yield return (object)new WaitForSeconds(0.2f);
		}
	}

	private IEnumerator DoFuelBurn()
	{
		while (IsStillFlaming())
		{
			currentFuel -= burnRate * Time.deltaTime;
			((Component)muzzlePoint).transform.forward = owner.LookDirection;
			if (owner.IsLocal)
			{
				MVRigidBody mvRigidBody = owner.WorldObjectOwner.GameObject.GetComponent<MVRigidBody>();
				if (!mvRigidBody.Grounded)
				{
					float verticalVelocity = mvRigidBody.Velocity.y;
					if (verticalVelocity < 0f)
					{
						float flamerImpulse = owner.LookDirection.y * 30f * 0.95f;
						float impulseY = Mathf.Min(flamerImpulse, verticalVelocity);
						mvRigidBody.AddImpulse(new Vector3(0f, 0f - impulseY, 0f), suspendImpactDamage: true);
					}
				}
			}
			if (currentFuel < 0f)
			{
				MVEquipable equipable = owner.WorldObjectOwner.GameObject.GetComponent<MVEquipable>();
				if ((Object)(object)equipable != (Object)null)
				{
					equipable.Unequip();
				}
				yield break;
			}
			yield return 0;
		}
		flameParticles.enableEmission = false;
		((Component)this).audio.Stop();
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
		((Component)this).audio.Play();
		flameParticles.Play();
		isFlaming = true;
		((MonoBehaviour)this).StartCoroutine(DoFlaming());
		((MonoBehaviour)this).StartCoroutine(DoFuelBurn());
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		List<InteractionDataHandlerBase> list = new List<InteractionDataHandlerBase>();
		Vector3 val = hitZoneCenter.position - muzzlePoint.position;
		Ray ray = new Ray(muzzlePoint.position, val.normalized);
		int layerMask = 1 << LayerMask.NameToLayer("Player");
		List<VoxelHit> list2 = CollisionDetection.MVSphereCastAll(ray, hitRadius, val.magnitude, owner.IgnoreWOIDs, layerMask);
		foreach (VoxelHit item in list2)
		{
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(item.transform);
			if (mVObject != null)
			{
				InteractionDataHandlerBase component = mVObject.GameObject.GetComponent<InteractionDataHandlerBase>();
				if ((Object)(object)component != (Object)null)
				{
					list.Add(component);
				}
			}
		}
		return list;
	}
}
