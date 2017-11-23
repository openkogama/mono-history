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
	private ObscuredInt maxAmmo = 15;

	[SerializeField]
	private RailRay railGunRayPrefab;

	[SerializeField]
	private float targetFieldOfView = 25f;

	[SerializeField]
	private ObscuredFloat baseDamage = 25f;

	[SerializeField]
	private Color hitColor = new Color(0.2f, 0.3f, 0.9f);

	[SerializeField]
	private Color missColor = new Color(0.9f, 0.3f, 0.2f);

	[SerializeField]
	private AudioSource releaseSound;

	[SerializeField]
	private AudioSource chargeAudioSource;

	[SerializeField]
	private AnimationCurve chargeCurve;

	[SerializeField]
	private float curveChargeLength = 5f;

	private float toFieldOfView;

	private float initialFOV;

	private bool isCharging;

	private float chargeBeginTime;

	private ObscuredInt currentAmmo;

	private float currentCharge;

	private int hitLayerMask;

	public override AvatarItemType Type => AvatarItemType.RailGun;

	public override int Quantity => currentAmmo;

	public override float ChargeState
	{
		get
		{
			if (isCharging)
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
			if (currentCharge >= 1f)
			{
				return crossHairCanFire;
			}
			return Color.Lerp(crossHairCannotFireLow, crossHairCannotFireHigh, currentCharge);
		}
	}

	private void Awake()
	{
		hitLayerMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("Player"));
		currentAmmo = maxAmmo;
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
		currentAmmo = maxAmmo;
	}

	private void DoChargingAnimation()
	{
		currentCharge = chargeCurve.Evaluate((Time.time - chargeBeginTime) / curveChargeLength);
		chargeAudioSource.pitch = 0.2f + currentCharge;
		if (owner.IsLocal)
		{
			MVGameControllerBase.CameraController.MainCamera.fieldOfView = Mathf.Lerp(initialFOV, toFieldOfView, currentCharge);
		}
		chargeParticles.time = currentCharge;
	}

	private void Update()
	{
		if (isCharging)
		{
			DoChargingAnimation();
			if (!chargeAudioSource.isPlaying)
			{
				chargeAudioSource.Play();
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
			if (owner.IsLocal)
			{
				MVGameControllerBase.CameraController.MainCamera.fieldOfView = initialFOV;
			}
		}
	}

	public override void OnEquip()
	{
		base.OnEquip();
		initialFOV = MVGameControllerBase.CameraController.MainCamera.fieldOfView;
	}

	public override void OnUnequip()
	{
		base.OnUnequip();
		isFiring = false;
		if (owner.IsLocal)
		{
			isCharging = false;
			MVGameControllerBase.CameraController.MainCamera.fieldOfView = initialFOV;
		}
	}

	public override void TriggerBegin(int instigatorActorNr)
	{
		base.TriggerBegin(instigatorActorNr);
		isCharging = true;
		chargeBeginTime = Time.time;
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
			chargeAudioSource.Stop();
			chargeAudioSource.loop = false;
			isCharging = false;
			return;
		}
		if ((bool)releaseSound)
		{
			chargeAudioSource.Stop();
			chargeAudioSource.loop = false;
			chargeAudioSource.pitch = 1.5f - currentCharge * 0.8f;
			Vector3 position = ((!owner.IsLocal) ? muzzlePoint.position : (Camera.main.transform.position + Camera.main.transform.forward));
			MVGameControllerBase.AudioManager.Play("RailShot", releaseSound, position);
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
		Vector3 point;
		if (CollisionDetection.MVHit(ray, out var voxelHit, range, owner.IgnoreWOIDs, hitLayerMask))
		{
			point = voxelHit.point;
			int woIDHighestInHierarchyWithComponent = MVGameControllerBase.WOCM.GetWoIDHighestInHierarchyWithComponent<InteractionDataHandlerBase>(voxelHit.woId);
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woIDHighestInHierarchyWithComponent);
			if (worldObjectClient != null)
			{
				if (owner.IsLocal)
				{
					MVGameControllerBase.Game.World.RuntimeEventManager.SendRemoveOneFineGrainedCube(voxelHit, baseDamage);
					InteractionDataHandlerBase interactionDataHandlerBase = worldObjectClient.InteractionDataHandlerBase;
					if (interactionDataHandlerBase != null && !MVGameControllerBase.Game.TeamManager.IsOnSameTeam(worldObjectClient, MVGameControllerBase.Game.LocalPlayer.Avatar))
					{
						InteractionData interaction = RailgunHitPackage.Create();
						interactionDataHandlerBase.HandleInteraction(owner, interaction, interactionIsLocal: false);
					}
				}
				if (worldObjectClient is IBulletImpactVisualizer)
				{
					((IBulletImpactVisualizer)worldObjectClient).VisualizeBulletImpact(voxelHit, ray, owner.WorldObjectOwner.OwnerActorNr, baseDamage);
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
