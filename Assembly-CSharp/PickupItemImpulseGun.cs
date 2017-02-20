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

	public float shakeFrequency = 1f;

	public float shakePower = 1f;

	public Vector3 shakeDirection = new Vector3(1f, 1f, 1f);

	public AudioClip chargeSound;

	public AudioClip releaseSound;

	public AnimationCurve chargeCurve;

	public AnimationCurve shakeCurve;

	[SerializeField]
	private Transform modelTransform;

	[SerializeField]
	private AudioSource audioSource;

	private float maxVolume;

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

	private void Awake()
	{
		maxVolume = audioSource.volume;
	}

	private void DoChargingAnimation()
	{
		float num = chargeCurve.Evaluate(Time.time - chargeBeginTime);
		float num2 = shakeCurve.Evaluate((Time.time - chargeBeginTime) * shakeFrequency * num) / shakePower;
		audioSource.volume = num * maxVolume;
		modelTransform.localPosition = Vector3.zero + shakeDirection * num2;
		chargeObject.localScale = Vector3.one * (num + num2 * 0.5f);
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		if ((bool)chargeSound)
		{
			audioSource.clip = chargeSound;
			audioSource.loop = true;
			if (audioSource.isPlaying)
			{
				audioSource.Play();
			}
		}
		isCharging = true;
		chargeBeginTime = Time.time;
	}

	private void Update()
	{
		if (isCharging)
		{
			if (!chargeObject.gameObject.activeInHierarchy)
			{
				chargeObject.gameObject.SetActive(value: true);
			}
			DoChargingAnimation();
			if (!audioSource.isPlaying)
			{
				audioSource.Play();
			}
		}
		else if (chargeObject.gameObject.activeInHierarchy)
		{
			chargeObject.gameObject.SetActive(value: false);
			modelTransform.localPosition = Vector3.zero;
		}
	}

	public override void TriggerEnd()
	{
		if (isCharging)
		{
			if ((bool)releaseSound && audioSource.gameObject.activeInHierarchy)
			{
				audioSource.Stop();
				audioSource.loop = false;
				audioSource.PlayOneShot(releaseSound);
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
		Vector3 vector = FindRayTarget(lineOfFire);
		if (owner.IsLocal)
		{
			List<MVWorldObjectClient> list = SphereCastAgainstWorldObjects(lineOfFire);
			for (int i = 0; i < list.Count; i++)
			{
				MVWorldObjectClient mVWorldObjectClient = list[i];
				InteractionDataHandlerBase interactionDataHandlerBase = mVWorldObjectClient.InteractionDataHandlerBase;
				if (interactionDataHandlerBase != null && !MVGameControllerBase.Game.TeamManager.IsOnSameTeam(mVWorldObjectClient.OwnerActorNr, MVGameControllerBase.Game.LocalPlayer.ActorNr))
				{
					Vector3 impulse = ComputeImpulseDirection(lineOfFire) * impulseMagnitude;
					interactionDataHandlerBase.HandleInteraction(ImpulseHitPackage.Create(impulse), interactionIsLocal: false);
				}
			}
			Vector3 b = owner.transform.position + Vector3.up * 1.5f;
			float num = Vector3.Distance(vector, b);
			if (num < 10f)
			{
				float num2 = recoilMagnitude / Mathf.Max(num * 0.5f, 1f);
				if (impulseMagnitude > 2500f)
				{
					MVGameControllerBase.Game.World.RuntimeEventManager.SendRuntimeEvent(new ExplosionEvent(RuntimeEventType.ImpulseGunImpact, vector));
				}
				Vector3 impulse2 = -lineOfFire.direction * num2;
				MVRigidBody component = owner.GetComponent<MVRigidBody>();
				if (component != null)
				{
					component.AddImpulse(impulse2, suspendImpactDamage: true);
				}
			}
		}
		ImpulseRay impulseRay = PrefabPool.Instance.EnumPoolManager.Instantiate<ImpulseRay>(PoolEnums.ImpulseGunRay);
		impulseRay.transform.position = muzzlePoint.position;
		impulseRay.radius = radius;
		impulseRay.startColor = missColor;
		impulseRay.Initialize(vector);
	}

	private List<MVWorldObjectClient> SphereCastAgainstWorldObjects(Ray lineOfFire)
	{
		List<MVWorldObjectClient> list = new List<MVWorldObjectClient>();
		List<VoxelHit> list2 = CollisionDetection.MVSphereCastAll(lineOfFire, radius, maxRange, owner.IgnoreWOIDs, 1 << LayerMask.NameToLayer("Player"));
		for (int i = 0; i < list2.Count; i++)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(list2[i].woId);
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
