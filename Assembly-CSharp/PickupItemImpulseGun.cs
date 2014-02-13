using System.Collections;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PickupItemImpulseGun : PickupItem
{
	public Transform muzzlePoint;

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

	public PickupItemImpulseGun()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
		((Component)chargeObject).gameObject.SetActiveRecursively(false);
		meshRenderers = ((Component)this).GetComponentsInChildren<MeshRenderer>();
	}

	private IEnumerator DoChargingAnimation()
	{
		((Component)chargeObject).gameObject.SetActiveRecursively(true);
		while (isCharging)
		{
			float scale = chargeCurve.Evaluate(Time.time - chargeBeginTime);
			((Component)this).audio.volume = scale;
			float size = Random.Range(0.1f, 0.3f);
			((Component)this).transform.localScale = new Vector3(size, size, size) * scale + Vector3.one;
			chargeObject.localScale = Vector3.one * (scale + size * 0.5f);
			yield return 0;
		}
		((Component)chargeObject).gameObject.SetActiveRecursively(false);
		((Component)this).transform.localScale = Vector3.one;
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		if (Object.op_Implicit((Object)(object)chargeSound))
		{
			((Component)this).audio.clip = chargeSound;
			((Component)this).audio.loop = true;
			((Component)this).audio.Play();
		}
		isCharging = true;
		chargeBeginTime = Time.time;
		((MonoBehaviour)this).StartCoroutine(DoChargingAnimation());
	}

	public override void TriggerEnd()
	{
		if (isCharging)
		{
			if (Object.op_Implicit((Object)(object)releaseSound))
			{
				((Component)this).audio.Stop();
				((Component)this).audio.loop = false;
				((Component)this).audio.volume = Mathf.Min(0.6f, ((Component)this).audio.volume);
				((Component)this).audio.PlayOneShot(releaseSound);
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
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
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
					if ((Object)(object)component != (Object)null)
					{
						component.HandleInteraction(ImpulseHitPackage.Create(impulse), interactionIsLocal: false);
					}
				}
			}
		}
		Vector3 val = FindRayTarget(lineOfFire);
		Vector3 val2 = ((Component)owner).transform.position + Vector3.up * 1.5f;
		float num = Vector3.Distance(val, val2);
		if (owner.IsLocal && num < 10f)
		{
			Vector3 impulse2 = -lineOfFire.direction * recoilMagnitude / Mathf.Max(num * 0.5f, 1f);
			MVRigidBody component2 = ((Component)owner).GetComponent<MVRigidBody>();
			if ((Object)(object)component2 != (Object)null)
			{
				component2.AddImpulse(impulse2, suspendImpactDamage: true);
			}
		}
		ImpulseRay impulseRay = Object.Instantiate((Object)(object)impulseRayPrefab, muzzlePoint.position, Quaternion.identity) as ImpulseRay;
		impulseRay.target = val;
		impulseRay.radius = radius;
		impulseRay.startColor = ((!flag) ? missColor : hitColor);
	}

	private List<MVWorldObjectClient> SphereCastAgainstWorldObjects(Ray lineOfFire)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		List<MVWorldObjectClient> list = new List<MVWorldObjectClient>();
		List<VoxelHit> list2 = CollisionDetection.MVSphereCastAll(lineOfFire, radius, maxRange, owner.IgnoreWOIDs, 1 << LayerMask.NameToLayer("Player"));
		foreach (VoxelHit item in list2)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(item.woId);
			if (worldObjectClient != null)
			{
				list.Add(worldObjectClient);
			}
		}
		return list;
	}

	private Vector3 FindRayTarget(Ray lineOfFire)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		VoxelHit voxelHit;
		return (!CollisionDetection.MVHit(lineOfFire, out voxelHit, maxRange, new HashSet<int>(), 1 << LayerMask.NameToLayer("Default"))) ? lineOfFire.GetPoint(maxRange) : voxelHit.point;
	}

	private Vector3 ComputeImpulseDirection(Ray lineOfFire)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 direction = lineOfFire.direction;
		direction.y += 0.2f;
		return direction.normalized;
	}
}
