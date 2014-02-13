using System.Collections;
using MV.Common;
using UnityEngine;

public class PickupItemRailGun : PickupItem
{
	public Transform muzzlePoint;

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

	public AnimationCurve chargeCurve;

	private bool isCharging;

	private float chargeBeginTime;

	private int currentAmmo = 10;

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
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			if (!isCharging)
			{
				return crossHairCannotFireLow;
			}
			float num = chargeCurve.Evaluate(Time.time - chargeBeginTime);
			if (num < 1f)
			{
				float num2 = num / 1f;
				return Color.Lerp(crossHairCannotFireLow, crossHairCannotFireHigh, num2);
			}
			return crossHairCanFire;
		}
	}

	public PickupItemRailGun()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Start()
	{
		meshRenderers = ((Component)this).GetComponentsInChildren<MeshRenderer>();
		currentAmmo = ammo;
	}

	public override void OnStateChanged(Hashtable newState)
	{
		currentAmmo = ammo;
	}

	private IEnumerator DoChargingAnimation()
	{
		chargeParticles.Play();
		Material m = ((Component)chargeParticles).renderer.sharedMaterial;
		while (isCharging && (Object)(object)owner.CurrentItem == (Object)(object)this)
		{
			float charge = chargeCurve.Evaluate(Time.time - chargeBeginTime);
			((Component)this).audio.volume = charge * 0.2f;
			if (owner.IsLocal)
			{
				Camera.main.fieldOfView = Mathf.Lerp(60f, 25f, charge);
			}
			chargeParticles.time = charge;
			if (charge >= 1f)
			{
				((Component)chargeParticles).renderer.material.SetColor("_TintColor", new Color(1f, 0.4f, 0.1f, 1f));
			}
			yield return 0;
		}
		((Component)chargeParticles).renderer.sharedMaterial = m;
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
		if (!isCharging)
		{
			return;
		}
		float num = chargeCurve.Evaluate(Time.time - chargeBeginTime);
		if (num < 1f)
		{
			((Component)this).audio.Stop();
			((Component)this).audio.loop = false;
			isCharging = false;
			return;
		}
		if (Object.op_Implicit((Object)(object)releaseSound))
		{
			((Component)this).audio.Stop();
			((Component)this).audio.loop = false;
			((Component)this).audio.PlayOneShot(releaseSound);
		}
		missColor.a = 1f;
		hitColor.a = missColor.a;
		Fire();
		isCharging = false;
		currentAmmo--;
		if (currentAmmo == 0)
		{
			MVEquipable component = ((Component)owner).GetComponent<MVEquipable>();
			if ((Object)(object)component != (Object)null)
			{
				component.Unequip();
			}
		}
	}

	private void Fire()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		Ray ray = new Ray(owner.LookOrigin, owner.LookDirection);
		bool flag = false;
		int layerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Player"));
		Vector3 point;
		if (CollisionDetection.MVHit(ray, out var voxelHit, range, owner.IgnoreWOIDs, layerMask))
		{
			point = voxelHit.point;
			int woIDHighestInHierarchyWithComponent = MVGameController.Instance.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
			if (owner.IsLocal && worldObjectClient != null)
			{
				InteractionDataHandlerBase component = worldObjectClient.GameObject.GetComponent<InteractionDataHandlerBase>();
				if ((Object)(object)component != (Object)null)
				{
					component.HandleInteraction(RailgunHitPackage.Create(), interactionIsLocal: false);
				}
			}
			flag = true;
		}
		else
		{
			point = ray.GetPoint(range);
		}
		RailRay railRay = Object.Instantiate((Object)(object)railGunRayPrefab, muzzlePoint.position, Quaternion.identity) as RailRay;
		railRay.target = point;
		railRay.startColor = ((!flag) ? missColor : hitColor);
	}
}
