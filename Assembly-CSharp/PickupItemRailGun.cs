using System.Collections;
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

	private AudioSource audioSource;

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

	private void Start()
	{
		meshRenderers = GetComponentsInChildren<MeshRenderer>();
		currentAmmo = ammo;
		audioSource = GetComponent<AudioSource>();
		chargeParticlesRenderer = chargeParticles.GetComponent<Renderer>();
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

	private IEnumerator DoChargingAnimation()
	{
		chargeParticles.Play();
		Material m = chargeParticlesRenderer.sharedMaterial;
		while (isCharging && owner.CurrentItem == this)
		{
			float charge = chargeCurve.Evaluate(Time.time - chargeBeginTime);
			audioSource.volume = charge * 0.2f;
			if (owner.IsLocal)
			{
				Camera.main.fieldOfView = Mathf.Lerp(60f, toFieldOfView, charge);
			}
			chargeParticles.time = charge;
			if (charge >= 1f)
			{
				chargeParticlesRenderer.material.SetColor("_TintColor", new Color(1f, 0.4f, 0.1f, 1f));
			}
			yield return 0;
		}
		chargeParticlesRenderer.sharedMaterial = m;
		chargeParticles.Stop();
		if (owner.IsLocal)
		{
			Camera.main.fieldOfView = 60f;
		}
	}

	public override void OnUnequip()
	{
		base.OnUnequip();
		if (owner.IsLocal)
		{
			Camera.main.fieldOfView = 60f;
		}
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		if ((bool)chargeSound)
		{
			audioSource.clip = chargeSound;
			audioSource.loop = true;
			audioSource.Play();
		}
		isCharging = true;
		chargeBeginTime = Time.time;
		StartCoroutine(DoChargingAnimation());
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
			audioSource.PlayOneShot(releaseSound);
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
		RailRay railRay = Object.Instantiate(railGunRayPrefab, muzzlePoint.position, Quaternion.identity) as RailRay;
		railRay.target = point;
		railRay.startColor = ((!flag) ? missColor : hitColor);
	}
}
