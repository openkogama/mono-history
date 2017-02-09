using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class PickupItemRailGun : PickupItemWithDelay
{
	public ParticleSystem chargeParticles;

	[SerializeField]
	private float range = 300f;

	[SerializeField]
	private int ammo = 10;

	[SerializeField]
	private RailRay railGunRayPrefab;

	[SerializeField]
	private float targetFieldOfView = 25f;

	[SerializeField]
	private float fireRate = 5f;

	[SerializeField]
	private float baseDamage = 50f;

	[SerializeField]
	private float chargeDamage = 30f;

	[SerializeField]
	private Color hitColor = new Color(0.2f, 0.3f, 0.9f);

	[SerializeField]
	private Color missColor = new Color(0.9f, 0.3f, 0.2f);

	[SerializeField]
	private AudioClip releaseSound;

	[SerializeField]
	private AudioSource audioSource;

	private float toFieldOfView;

	private float initialFOV = 60f;

	public AnimationCurve chargeCurve;

	private bool isCharging;

	private float chargeBeginTime;

	private float prevFireTime;

	private ObscuredInt currentAmmo = 10;

	private float currentCharge;

	private int hitLayerMask;

	public override AvatarItemType Type => AvatarItemType.RailGun;

	public override int Quantity => currentAmmo;

	public override float ChargeState
	{
		get
		{
			if (isCharging && Time.time > prevFireTime + fireRate)
			{
				return currentCharge;
			}
			return 0f;
		}
	}

	public override Color CrossHairColor
	{
		get
		{
			if (Time.time > prevFireTime + fireRate)
			{
				return crossHairCanFire;
			}
			float t = (Time.time - prevFireTime) / fireRate;
			return Color.Lerp(crossHairCannotFireLow, crossHairCannotFireHigh, t);
		}
	}

	private void Awake()
	{
		hitLayerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Player"));
		initialFOV = Camera.main.fieldOfView;
		currentAmmo = ammo;
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			toFieldOfView = initialFOV;
		}
		else
		{
			toFieldOfView = targetFieldOfView;
		}
	}

	public override void ResetAmmo()
	{
		base.ResetAmmo();
		currentAmmo = ammo;
	}

	private void DoChargingAnimation()
	{
		currentCharge = chargeCurve.Evaluate(Time.time - chargeBeginTime);
		audioSource.pitch = 0.2f + currentCharge;
		if (owner.IsLocal)
		{
			Camera.main.fieldOfView = Mathf.Lerp(initialFOV, toFieldOfView, currentCharge);
		}
		chargeParticles.time = currentCharge;
	}

	private void Update()
	{
		if (isCharging)
		{
			DoChargingAnimation();
			if (!audioSource.isPlaying)
			{
				audioSource.Play();
			}
			if (!chargeParticles.isPlaying)
			{
				chargeParticles.Play();
			}
		}
		else
		{
			if (chargeParticles.isPlaying)
			{
				chargeParticles.Stop();
			}
			if (Camera.main.fieldOfView != initialFOV)
			{
				Camera.main.fieldOfView = initialFOV;
			}
		}
	}

	public override void OnEquip()
	{
		base.OnEquip();
		prevFireTime = Time.time - fireRate;
	}

	public override void OnUnequip()
	{
		base.OnUnequip();
		isFiring = false;
		if (owner.IsLocal)
		{
			isCharging = false;
			Camera.main.fieldOfView = initialFOV;
		}
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		if (!(Time.time < prevFireTime + fireRate))
		{
			base.TriggerBegin(instigatorActorNr);
			isCharging = true;
			chargeBeginTime = Time.time;
		}
	}

	public override void TriggerEnd()
	{
		base.TriggerEnd();
		if (!isCharging)
		{
			return;
		}
		if (currentCharge < 1f)
		{
			audioSource.Stop();
			audioSource.loop = false;
			isCharging = false;
		}
		if ((bool)releaseSound)
		{
			audioSource.Stop();
			audioSource.loop = false;
			if (audioSource.gameObject.activeInHierarchy)
			{
				float num = 1.5f - currentCharge * 0.8f;
				audioSource.pitch = num;
				float pitch = num;
				Vector3 position = ((!owner.IsLocal) ? muzzlePoint.position : (Camera.main.transform.position + Camera.main.transform.forward));
				MVGameControllerBase.AudioManager.Play("RailShot", releaseSound, position, 0.2f, SoundRangeDistance.Long, pitch);
			}
		}
		missColor.a = 1f;
		hitColor.a = missColor.a;
		Fire();
		prevFireTime = Time.time;
		isCharging = false;
		currentAmmo = (int)currentAmmo - 1;
		if ((int)currentAmmo == 0)
		{
			MVEquipable component = owner.GetComponent<MVEquipable>();
			if (component != null)
			{
				component.Unequip();
			}
		}
	}

	private void Fire()
	{
		Ray ray = new Ray(owner.LookOrigin, owner.LookDirection);
		bool flag = false;
		Vector3 point;
		if (CollisionDetection.MVHit(ray, out var voxelHit, range, owner.IgnoreWOIDs, hitLayerMask))
		{
			point = voxelHit.point;
			int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
			if (worldObjectClient != null)
			{
				float damage = baseDamage + chargeDamage * currentCharge;
				if (owner.IsLocal)
				{
					MVGameControllerBase.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(voxelHit, damage);
					InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
					if (interactionDataHandlerBase != null && !MVGameControllerBase.Game.TeamManager.IsOnSameTeam(worldObjectClient.OwnerActorNr, MVGameControllerBase.Game.LocalPlayer.ActorNr))
					{
						InteractionData interaction = RailgunHitPackage.Create(damage);
						interactionDataHandlerBase.HandleInteraction(interaction, interactionIsLocal: false);
					}
				}
				if (worldObjectClient is IBulletImpactVisualizer)
				{
					((IBulletImpactVisualizer)worldObjectClient).VisualizeBulletImpact(voxelHit, ray, owner.WorldObjectOwner.OwnerActorNr, damage);
				}
			}
			flag = true;
		}
		else
		{
			point = ray.GetPoint(range);
		}
		RailRay railRay = PrefabPool.Instance.EnumPoolManager.Instantiate<RailRay>(PoolEnums.RailGunRay);
		railRay.target = point;
		railRay.startColor = ((!flag) ? missColor : hitColor);
		railRay.transform.localPosition = muzzlePoint.position;
		railRay.Reset();
	}
}
