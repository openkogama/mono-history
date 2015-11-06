using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PickupItemImpulseGun : PickupItem
{
	public Transform chargeObject;

	public float hitImpulse = 2400f;

	public float recoilImpulse = 1600f;

	public float maxRange = 50f;

	public float chargingRate = 100f;

	public float radius = 1.2f;

	public ImpulseRay impulseRayPrefab;

	public Color hitColor = new Color(0.2f, 0.3f, 0.9f);

	public Color missColor = new Color(0.9f, 0.3f, 0.2f);

	public AudioClip chargeSound;

	public AudioClip releaseSound;

	public AnimationCurve chargeCurve;

	private bool isCharging;

	private float chargeBeginTime;

	public override AvatarItemType Type => AvatarItemType.ImpulseGun;

	public override int Quantity => 0;

	public override float ChargeState
	{
		get
		{
			if (!isCharging)
			{
				return 0f;
			}
			return chargeCurve.Evaluate(Time.time - chargeBeginTime);
		}
	}

	private void Start()
	{
		chargeObject.gameObject.SetActive(value: false);
		meshRenderers = GetComponentsInChildren<MeshRenderer>();
	}

	private IEnumerator DoChargingAnimation()
	{
		chargeObject.gameObject.SetActive(value: true);
		while (isCharging)
		{
			float scale = chargeCurve.Evaluate(Time.time - chargeBeginTime);
			GetComponent<AudioSource>().volume = scale;
			float size = Random.Range(0.1f, 0.3f);
			transform.localScale = new Vector3(size, size, size) * scale + Vector3.one;
			chargeObject.localScale = Vector3.one * (scale + size * 0.5f);
			yield return 0;
		}
		chargeObject.gameObject.SetActive(value: false);
		transform.localScale = Vector3.one;
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		if ((bool)chargeSound)
		{
			GetComponent<AudioSource>().clip = chargeSound;
			GetComponent<AudioSource>().loop = true;
			GetComponent<AudioSource>().Play();
		}
		isCharging = true;
		chargeBeginTime = Time.time;
		StartCoroutine(DoChargingAnimation());
	}

	public override void TriggerEnd()
	{
		if (isCharging)
		{
			if ((bool)releaseSound)
			{
				GetComponent<AudioSource>().Stop();
				GetComponent<AudioSource>().loop = false;
				GetComponent<AudioSource>().volume = Mathf.Min(0.6f, GetComponent<AudioSource>().volume);
				GetComponent<AudioSource>().PlayOneShot(releaseSound);
			}
			float num = chargeCurve.Evaluate(Time.time - chargeBeginTime);
			missColor.a = num;
			hitColor.a = missColor.a;
			Fire(owner.WorldObjectOwner.Id, num * hitImpulse, num * recoilImpulse);
			isCharging = false;
		}
	}

	private void Fire(int avatarId, float impulseMagnitude, float recoilMagnitude)
	{
		Ray lineOfFire = new Ray(owner.LookOrigin, owner.LookDirection);
		bool flag = false;
		List<MVWorldObjectClient> list = SphereCastAgainstWorldObjects(lineOfFire);
		if (list.Count > 0)
		{
			flag = true;
			if (owner.IsLocal)
			{
				foreach (MVWorldObjectClient item in list)
				{
					Vector3 impulse = ComputeImpulseDirection(lineOfFire) * impulseMagnitude;
					InteractionDataHandlerBase component = item.GameObject.GetComponent<InteractionDataHandlerBase>();
					if (component != null)
					{
						component.HandleInteraction(ImpulseHitPackage.Create(impulse), interactionIsLocal: false);
					}
				}
			}
		}
		Vector3 vector = FindRayTarget(lineOfFire);
		Vector3 b = owner.transform.position + Vector3.up * 1.5f;
		float num = Vector3.Distance(vector, b);
		if (owner.IsLocal && num < 10f)
		{
			float num2 = recoilMagnitude / Mathf.Max(num * 0.5f, 1f);
			if (impulseMagnitude > 2500f)
			{
				MVGameControllerBase.Game.World.RuntimeEventManager.SendRuntimeEvent(new ExplosionEvent(RuntimeEventType.Bazooka, vector));
			}
			Vector3 impulse2 = -lineOfFire.direction * num2;
			MVRigidBody component2 = owner.GetComponent<MVRigidBody>();
			if (component2 != null)
			{
				component2.AddImpulse(impulse2, suspendImpactDamage: true);
			}
		}
		ImpulseRay impulseRay = Object.Instantiate(impulseRayPrefab, muzzlePoint.position, Quaternion.identity) as ImpulseRay;
		impulseRay.target = vector;
		impulseRay.radius = radius;
		impulseRay.startColor = ((!flag) ? missColor : hitColor);
	}

	private List<MVWorldObjectClient> SphereCastAgainstWorldObjects(Ray lineOfFire)
	{
		List<MVWorldObjectClient> list = new List<MVWorldObjectClient>();
		List<VoxelHit> list2 = CollisionDetection.MVSphereCastAll(lineOfFire, radius, maxRange, owner.IgnoreWOIDs, 1 << LayerMask.NameToLayer("Player"));
		foreach (VoxelHit item in list2)
		{
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
