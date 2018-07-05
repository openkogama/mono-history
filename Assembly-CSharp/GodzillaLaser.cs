using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.RuntimeEvents;
using UnityEngine;

public class GodzillaLaser : PickupItem, IUpdatecontrollerSubscriber
{
	[Header("Stats for Godzilla (will be scaled by godzilla size)")]
	[SerializeField]
	private AnimationCurve baseAreaDamageByRange;

	[SerializeField]
	private AnimationCurve baseAreaImpulseByRange;

	[SerializeField]
	private float maxFireRange = 3000f;

	[SerializeField]
	[Header("Graphics")]
	private Color crossHairCanFire;

	[SerializeField]
	private Color crossHairCannotFireLow;

	[SerializeField]
	private Color crossHairCannotFireHigh;

	[SerializeField]
	private AnimationCurve chargeCurve;

	[Header("Network")]
	[Tooltip("Look direction updates per second.")]
	[SerializeField]
	private float lookUpdateRate = 2f;

	[SerializeField]
	[Header("Dependencies")]
	private List<GameObject> toHideInFirstperson = new List<GameObject>();

	[SerializeField]
	private AvatarBlinker blinker;

	[SerializeField]
	private ParticleSystem coreParticles;

	[SerializeField]
	private ParticleSystem chargeParticles;

	[SerializeField]
	private LaserSight laserSight;

	[SerializeField]
	private RailRay railGunRayPrefab;

	[SerializeField]
	private AudioSource chargeSound;

	[SerializeField]
	private AudioSource blastSound;

	[SerializeField]
	private ParticleSystem hitExplosion;

	[SerializeField]
	private CFX_AutoDestructShuriken hitExplosionDestruction;

	[SerializeField]
	private ParticleSystem glassShardExplosion;

	[SerializeField]
	private AudioSource glassShardExplosionSound;

	[SerializeField]
	private CFX_AutoDestructShuriken glassShardDestruction;

	private AnimationCurve scaledAreaDamageByRange;

	private AnimationCurve scaledAreaImpulseByRange;

	private RuntimeEventType laserImpactType;

	private GodzillaModifier.LaserBurnInteractionPackageType laserBurnType;

	private float godzillaSizeModifier;

	private float toFieldOfView;

	private float prevFieldOfView;

	private bool isCharging;

	private float charge;

	private float chargeBeginTime;

	private bool releaseSoundPlaying;

	private bool hitSomething;

	private VoxelHit voxelHit;

	private Vector3 prevLookDir;

	private Vector3 prevLookOrigin;

	private Vector3 prevPointOfImpact;

	private Vector3 pointOfImpact;

	private float baseChargeVolume;

	private float lookDirUpdateTimer;

	private float laserBurnUpdateTimer;

	private int layerMask_DefaultAndPlayer;

	private int layerMask_Player;

	private float BlastRadius => scaledAreaDamageByRange.keys[scaledAreaDamageByRange.length - 1].time;

	public override AvatarItemType Type => AvatarItemType.GodzillaLaser;

	public override int Quantity => 0;

	public override float ChargeState => (!isCharging) ? 0f : chargeCurve.Evaluate(Time.time - chargeBeginTime);

	public override bool CanUnequip => false;

	public override Color CrossHairColor => Color.Lerp(crossHairCannotFireLow, crossHairCannotFireHigh, charge);

	private void Awake()
	{
		prevFieldOfView = Camera.main.fieldOfView;
		baseChargeVolume = chargeSound.volume;
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			toFieldOfView = prevFieldOfView;
		}
		else
		{
			toFieldOfView = 55f;
		}
		laserSight.transform.parent = null;
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
		layerMask_DefaultAndPlayer = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Player"));
		layerMask_Player = 1 << LayerMask.NameToLayer("Player");
	}

	private void OnDestroy()
	{
		if (laserSight != null)
		{
			Object.Destroy(laserSight.gameObject);
		}
		UpdateController.RemoveUpdateObject(this);
	}

	public override void OnEquip()
	{
		base.OnEquip();
		gameObject.SetActive(value: true);
		laserSight.Initialize();
		if (owner.IsLocal)
		{
			HideObjectsForFirstPersonView();
		}
		blinker.MeshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
		blinker.Attach(MVGameControllerBase.WOCM.GetWorldObjectClient(owner.WorldObjectOwner.Id) as MVAvatar);
		blinker.Visible = true;
		hitExplosion.transform.parent = null;
		glassShardExplosion.transform.parent = null;
	}

	private AnimationCurve ScaleAnimCurve(AnimationCurve a, float scale)
	{
		Keyframe[] keys = a.keys;
		for (int i = 0; i < keys.Length; i++)
		{
			Keyframe keyframe = keys[i];
			keyframe.time *= scale;
			keyframe.value *= scale;
			keys.SetValue(keyframe, i);
		}
		AnimationCurve animationCurve = new AnimationCurve(keys);
		animationCurve.preWrapMode = a.preWrapMode;
		animationCurve.postWrapMode = a.postWrapMode;
		return animationCurve;
	}

	public override void OnStateChanged(Dictionary<object, object> data)
	{
		Dictionary<object, object> dictionary = (Dictionary<object, object>)data["itemData"];
		GodzillaModifier.GodzillaModifierPackageType key = (GodzillaModifier.GodzillaModifierPackageType)(byte)dictionary["avatarModifierPackageType"];
		godzillaSizeModifier = GodzillaModifier.constants[key].sizeModifier;
		laserImpactType = GodzillaModifier.constants[key].laserImpactEventType;
		laserBurnType = GodzillaModifier.constants[key].laserBurnInteractionPackageType;
		scaledAreaDamageByRange = ScaleAnimCurve(baseAreaDamageByRange, godzillaSizeModifier);
		scaledAreaImpulseByRange = ScaleAnimCurve(baseAreaImpulseByRange, godzillaSizeModifier);
		hitExplosion.transform.localScale = new Vector3(godzillaSizeModifier, godzillaSizeModifier, godzillaSizeModifier);
		glassShardExplosion.transform.localScale = new Vector3(godzillaSizeModifier, godzillaSizeModifier, godzillaSizeModifier);
	}

	private void HideObjectsForFirstPersonView()
	{
		for (int i = 0; i < toHideInFirstperson.Count; i++)
		{
			toHideInFirstperson[i].SetActive(value: false);
		}
		laserSight.DisableLineRenderer();
		coreParticles.Stop();
		coreParticles.Clear();
	}

	public override void OnUnequip()
	{
		base.OnUnequip();
		gameObject.SetActive(value: false);
		if (owner.IsLocal)
		{
			isCharging = false;
			Camera.main.fieldOfView = prevFieldOfView;
		}
		blinker.Detach();
		hitExplosion.gameObject.transform.localPosition = center.position;
		hitExplosion.Play();
		hitExplosionDestruction.enabled = true;
		glassShardExplosion.gameObject.transform.localPosition = center.position;
		glassShardExplosion.Play();
		glassShardDestruction.enabled = true;
		glassShardExplosionSound.Play();
	}

	public void UpdateControllerUpdate()
	{
		laserSight.transform.localScale = transform.lossyScale;
		UpdatePointOfImpact();
		Vector3 point = ((!owner.IsLocal) ? Vector3.Lerp(prevPointOfImpact, pointOfImpact, lookDirUpdateTimer) : pointOfImpact);
		AimAt(point);
		if (isCharging)
		{
			charge = chargeCurve.Evaluate(Time.time - chargeBeginTime);
			UpdateChargeParticleSystem(charge);
			chargeSound.volume = charge * baseChargeVolume;
			if (owner.IsLocal)
			{
				if (charge >= 1f)
				{
					LocalBurn();
				}
				Camera.main.fieldOfView = Mathf.Lerp(prevFieldOfView, toFieldOfView, charge);
			}
		}
		else
		{
			Camera.main.fieldOfView = prevFieldOfView;
		}
	}

	public void UpdateControllerFixedUpdate()
	{
	}

	private void UpdateChargeParticleSystem(float charge)
	{
		chargeParticles.transform.LookAt(muzzlePoint);
		ParticleSystem.EmissionModule emission = chargeParticles.emission;
		emission.rate = new ParticleSystem.MinMaxCurve(charge * 128f);
	}

	private void AimAt(Vector3 point)
	{
		center.LookAt(point);
		laserSight.AimAt(point);
		chargeParticles.transform.position = point;
	}

	private void UpdatePointOfImpact()
	{
		lookDirUpdateTimer += Time.deltaTime * lookUpdateRate;
		if (owner.IsLocal && lookDirUpdateTimer > 1f)
		{
			lookDirUpdateTimer = 0f;
			MVGameControllerBase.OperationRequests.UpdateLineOfFire(owner.WorldObjectOwner.Id, owner.LookDirection, owner.LookOrigin);
		}
		if (owner.LookDirection != prevLookDir || owner.LookOrigin != prevLookOrigin)
		{
			if (!owner.IsLocal)
			{
				lookDirUpdateTimer = 0f;
			}
			prevLookDir = owner.LookDirection;
			prevLookOrigin = owner.LookOrigin;
			prevPointOfImpact = pointOfImpact;
			Ray ray = new Ray(owner.LookOrigin, owner.LookDirection);
			hitSomething = CollisionDetection.MVHit(ray, out voxelHit, maxFireRange, owner.IgnoreWOIDs, layerMask_DefaultAndPlayer);
			if (hitSomething)
			{
				pointOfImpact = voxelHit.point;
			}
			else
			{
				pointOfImpact = ray.GetPoint(maxFireRange);
			}
		}
	}

	public void LocalBurn()
	{
		laserBurnUpdateTimer -= Time.deltaTime;
		if (!(laserBurnUpdateTimer <= 0f))
		{
			return;
		}
		laserBurnUpdateTimer = 1f;
		int num = Physics.OverlapSphereNonAlloc(voxelHit.point, BlastRadius, CollisionDetectionGlobalBuffers.colliderBuffer, layerMask_Player);
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < num; i++)
		{
			Collider collider = CollisionDetectionGlobalBuffers.colliderBuffer[i];
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(collider.transform);
			if (mVObject != null && !hashSet.Contains(mVObject.Id))
			{
				hashSet.Add(mVObject.Id);
				InteractionDataHandlerBase interactionDataHandlerBase = mVObject.InteractionDataHandlerBase;
				if (interactionDataHandlerBase != null && mVObject != owner.WorldObjectOwner)
				{
					interactionDataHandlerBase.HandleInteraction(owner, GodzillaLaserBurnPackage.Create(laserBurnType), interactionIsLocal: false);
				}
			}
		}
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		chargeSound.Play();
		isCharging = true;
		chargeBeginTime = Time.time;
		chargeParticles.Play();
	}

	public override void TriggerEnd()
	{
		if (charge >= 1f)
		{
			blastSound.Play();
			Fire();
		}
		isCharging = false;
		chargeSound.Stop();
		charge = 0f;
		chargeParticles.Stop();
	}

	private void Fire()
	{
		if (hitSomething)
		{
			HandleDirectRayhit();
			AreaDamage();
		}
		SendProjectile(pointOfImpact, hitSomething);
		laserSight.Flash();
	}

	public void HandleDirectRayhit()
	{
		int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
		if (owner.IsLocal && worldObjectClient != null)
		{
			InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
			if (interactionDataHandlerBase != null)
			{
				interactionDataHandlerBase.HandleInteraction(owner, GodzillaLaserHitPackage.Create(godzillaSizeModifier), interactionIsLocal: false);
			}
		}
	}

	public void SendProjectile(Vector3 pointOfImpact, bool hit)
	{
		hitExplosion.gameObject.transform.position = pointOfImpact;
		hitExplosion.Play();
	}

	public void AreaDamage()
	{
		if (owner.IsLocal)
		{
			LocalDealAreaDamage();
		}
	}

	private void LocalDealAreaDamage()
	{
		ExplosionEvent explosion = new ExplosionEvent(laserImpactType, voxelHit.point, voxelHit.normal);
		MVGameControllerBase.Game.World.RuntimeEventManager.SendRuntimeEvent(explosion);
		int num = Physics.OverlapSphereNonAlloc(voxelHit.point, BlastRadius, CollisionDetectionGlobalBuffers.colliderBuffer, layerMask_DefaultAndPlayer);
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < num; i++)
		{
			Collider collider = CollisionDetectionGlobalBuffers.colliderBuffer[i];
			MVWorldObjectClient mVObject = MVWorldObjectClientManager.GetMVObject(collider.transform);
			if (mVObject != null && !hashSet.Contains(mVObject.Id))
			{
				hashSet.Add(mVObject.Id);
				InteractionDataHandlerBase interactionDataHandlerBase = mVObject.InteractionDataHandlerBase;
				if (interactionDataHandlerBase != null && mVObject != owner.WorldObjectOwner)
				{
					float time = Vector3.Distance(voxelHit.point, collider.transform.position);
					float damage = scaledAreaDamageByRange.Evaluate(time);
					Vector3 normalized = (collider.transform.position - voxelHit.point).normalized;
					interactionDataHandlerBase.HandleInteraction(owner, ProximityDamageAndImpulse.Create(damage, normalized * scaledAreaImpulseByRange.Evaluate(time), PlayerKilledByType.GodzillaLaser), interactionIsLocal: false);
				}
			}
		}
	}
}
