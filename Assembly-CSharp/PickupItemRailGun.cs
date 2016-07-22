using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class PickupItemRailGun : PickupItem
{
	public ParticleSystem chargeParticles;

	public float range = 300f;

	public int ammo = 10;

	public RailRay railGunRayPrefab;

	public AudioClip hitSound;

	public AudioClip missSound;

	public Color hitColor = new Color(0.2f, 0.3f, 0.9f);

	public Color missColor = new Color(0.9f, 0.3f, 0.2f);

	public Color crossHairCannotFireLow;

	public Color crossHairCannotFireHigh;

	public Color crossHairCanFire;

	public AudioClip chargeSound;

	public AudioClip releaseSound;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private Renderer chargeParticlesRenderer;

	private float toFieldOfView;

	public AnimationCurve chargeCurve;

	private bool isCharging;

	private float chargeBeginTime;

	private ObscuredInt currentAmmo = 10;

	public override AvatarItemType Type => AvatarItemType.RailGun;

	public override int Quantity => currentAmmo;

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

	public override Color CrossHairColor
	{
		get
		{
			if (!isCharging)
			{
				return crossHairCannotFireLow;
			}
			float num = chargeCurve.Evaluate(Time.time - chargeBeginTime);
			if (num < 1f)
			{
				float t = num / 1f;
				return Color.Lerp(crossHairCannotFireLow, crossHairCannotFireHigh, t);
			}
			return crossHairCanFire;
		}
	}

	private void Awake()
	{
		currentAmmo = ammo;
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			toFieldOfView = 60f;
		}
		else
		{
			toFieldOfView = 25f;
		}
	}

	public override void OnStateChanged(Dictionary<object, object> newState)
	{
		currentAmmo = ammo;
	}

	private void DoChargingAnimation()
	{
		float num = chargeCurve.Evaluate(Time.time - chargeBeginTime);
		audioSource.volume = num * 0.2f;
		if (owner.IsLocal)
		{
			Camera.main.fieldOfView = Mathf.Lerp(60f, toFieldOfView, num);
		}
		chargeParticles.time = num;
	}

	private void Update()
	{
		if (isCharging)
		{
			if (!audioSource.isPlaying)
			{
				audioSource.Play();
			}
			if (!chargeParticles.isPlaying)
			{
				chargeParticles.Play();
			}
			DoChargingAnimation();
			return;
		}
		if (chargeParticles.isPlaying)
		{
			chargeParticles.Stop();
		}
		if (audioSource.isPlaying)
		{
			audioSource.Stop();
		}
		if (Camera.main.fieldOfView != 60f)
		{
			Camera.main.fieldOfView = 60f;
		}
	}

	public override void OnUnequip()
	{
		base.OnUnequip();
		if (owner.IsLocal)
		{
			isCharging = false;
			Camera.main.fieldOfView = 60f;
		}
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		if ((bool)chargeSound)
		{
			audioSource.clip = chargeSound;
			audioSource.loop = true;
		}
		isCharging = true;
		chargeBeginTime = Time.time;
	}

	public override void TriggerEnd()
	{
		if (!isCharging)
		{
			return;
		}
		float num = chargeCurve.Evaluate(Time.time - chargeBeginTime);
		if (num < 1f)
		{
			audioSource.Stop();
			audioSource.loop = false;
			isCharging = false;
			return;
		}
		if ((bool)releaseSound)
		{
			audioSource.Stop();
			audioSource.loop = false;
			if (gameObject.activeInHierarchy)
			{
				audioSource.PlayOneShot(releaseSound);
			}
		}
		missColor.a = 1f;
		hitColor.a = missColor.a;
		Fire();
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
		int layerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Player"));
		Vector3 point;
		if (CollisionDetection.MVHit(ray, out var voxelHit, range, owner.IgnoreWOIDs, layerMask))
		{
			InteractionData interaction = RailgunHitPackage.Create();
			MVGameControllerBase.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(voxelHit, interaction.Damage);
			point = voxelHit.point;
			int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
			if (owner.IsLocal && worldObjectClient != null)
			{
				InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
				if (interactionDataHandlerBase != null)
				{
					interactionDataHandlerBase.HandleInteraction(interaction, interactionIsLocal: false);
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
