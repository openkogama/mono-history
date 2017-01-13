using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class GodzillaModifier : AvatarModifier
{
	public class GodzillaModifierData
	{
		public readonly float sizeModifier;

		public readonly RuntimeEventType laserImpactEventType;

		public readonly LaserBurnInteractionPackageType laserBurnInteractionPackageType;

		public readonly AvatarModifierPackageType laserBurnModifierPackageType;

		public GodzillaModifierData(GodzillaModifierPackageType t)
		{
			switch (t)
			{
			case GodzillaModifierPackageType.S:
				sizeModifier = 4f;
				laserImpactEventType = RuntimeEventType.GodzillaLaserImpactS;
				laserBurnInteractionPackageType = LaserBurnInteractionPackageType.S;
				laserBurnModifierPackageType = AvatarModifierPackageType.GodzillaLaserBurnS;
				break;
			case GodzillaModifierPackageType.M:
				sizeModifier = 8f;
				laserImpactEventType = RuntimeEventType.GodzillaLaserImpactM;
				laserBurnInteractionPackageType = LaserBurnInteractionPackageType.M;
				laserBurnModifierPackageType = AvatarModifierPackageType.GodzillaLaserBurnM;
				break;
			case GodzillaModifierPackageType.L:
				sizeModifier = 20f;
				laserImpactEventType = RuntimeEventType.GodzillaLaserImpactL;
				laserBurnInteractionPackageType = LaserBurnInteractionPackageType.L;
				laserBurnModifierPackageType = AvatarModifierPackageType.GodzillaLaserBurnL;
				break;
			case GodzillaModifierPackageType.XL:
				sizeModifier = 40f;
				laserImpactEventType = RuntimeEventType.GodzillaLaserImpactXL;
				laserBurnInteractionPackageType = LaserBurnInteractionPackageType.XL;
				laserBurnModifierPackageType = AvatarModifierPackageType.GodzillaLaserBurnXL;
				break;
			default:
				Debug.LogError("Unknown GodzillaModifierPackageType");
				break;
			}
		}
	}

	public enum GodzillaModifierPackageType
	{
		S = 17,
		M,
		L,
		XL
	}

	public enum LaserBurnInteractionPackageType
	{
		S = 20,
		M,
		L,
		XL
	}

	public static readonly Dictionary<GodzillaModifierPackageType, GodzillaModifierData> constants = new Dictionary<GodzillaModifierPackageType, GodzillaModifierData>
	{
		{
			GodzillaModifierPackageType.S,
			new GodzillaModifierData(GodzillaModifierPackageType.S)
		},
		{
			GodzillaModifierPackageType.M,
			new GodzillaModifierData(GodzillaModifierPackageType.M)
		},
		{
			GodzillaModifierPackageType.L,
			new GodzillaModifierData(GodzillaModifierPackageType.L)
		},
		{
			GodzillaModifierPackageType.XL,
			new GodzillaModifierData(GodzillaModifierPackageType.XL)
		}
	};

	[SerializeField]
	[HideInInspector]
	private GodzillaModifierPackageType type;

	[HideInInspector]
	[SerializeField]
	private float sizeModifier;

	[SerializeField]
	private float growthTime = 3f;

	[SerializeField]
	private float shrinkTime = 1f;

	[SerializeField]
	private float growthRotationSpeed = 720f;

	[SerializeField]
	protected AudioSource audioSource;

	[SerializeField]
	private AudioClip growthSound;

	private Vector3 prevScale;

	private Vector3 modifiedScale;

	private float growthRate;

	private float shrinkRate;

	private bool isInitialized;

	public GodzillaModifierPackageType Type
	{
		set
		{
			type = value;
			sizeModifier = constants[type].sizeModifier;
		}
	}

	public override AvatarModifierPackageType ModifierType => ToAvatarModifierPackageType(type);

	public static AvatarModifierPackageType ToAvatarModifierPackageType(GodzillaModifierPackageType a)
	{
		return (AvatarModifierPackageType)a;
	}

	public static InteractionPackageType ToInteractionPackageType(LaserBurnInteractionPackageType a)
	{
		return (InteractionPackageType)a;
	}

	private void OnEnable()
	{
		if (isInitialized && owner != null)
		{
			if (IsActivated)
			{
				InstaGrow();
			}
			else
			{
				InstaShrink();
			}
		}
	}

	private void OnDisable()
	{
		if (isInitialized && owner != null)
		{
			if (IsActivated)
			{
				InstaGrow();
			}
			else
			{
				InstaShrink();
			}
		}
	}

	private void Destroy()
	{
		owner.mvAvatar.Scale = prevScale;
		Object.Destroy(gameObject);
	}

	protected override void OnActivated(Avatar target)
	{
		isInitialized = true;
		owner = target;
		prevScale = Vector3.one;
		modifiedScale = prevScale * sizeModifier;
		growthRate = sizeModifier / growthTime;
		shrinkRate = sizeModifier / shrinkTime;
		owner.mvAvatar.Body.BodyData.GetPartBone(BodyData.PartIndex.Head).gameObject.SetActive(value: false);
		owner.mvAvatar.Body.BodyData.GetPartBone(BodyData.PartIndex.LLowLeg).gameObject.SetActive(value: false);
		owner.mvAvatar.Body.BodyData.GetPartBone(BodyData.PartIndex.RLowLeg).gameObject.SetActive(value: false);
		owner.mvAvatar.Body.BodyData.GetPartBone(BodyData.PartIndex.LUpLeg).gameObject.SetActive(value: false);
		owner.mvAvatar.Body.BodyData.GetPartBone(BodyData.PartIndex.RUpLeg).gameObject.SetActive(value: false);
		audioSource.PlayOneShot(growthSound);
		owner.StartBlinking(BlinkType.Invulnerable);
		owner.mvAvatar.Invulnerable.Value = true;
		owner.mvAvatar.Health.Value = 100f;
		Scale();
	}

	protected override void OnDeactivated(Avatar target)
	{
		owner = target;
		owner.mvAvatar.Body.BodyData.GetPartBone(BodyData.PartIndex.LLowLeg).gameObject.SetActive(value: true);
		owner.mvAvatar.Body.BodyData.GetPartBone(BodyData.PartIndex.RLowLeg).gameObject.SetActive(value: true);
		owner.mvAvatar.Body.BodyData.GetPartBone(BodyData.PartIndex.LUpLeg).gameObject.SetActive(value: true);
		owner.mvAvatar.Body.BodyData.GetPartBone(BodyData.PartIndex.RUpLeg).gameObject.SetActive(value: true);
		UnScale();
	}

	private IEnumerator GrowthRoutine()
	{
		while (owner.mvAvatar.Scale.y < modifiedScale.y)
		{
			float growthAmnt = growthRate * Time.deltaTime;
			owner.mvAvatar.Scale = owner.mvAvatar.Scale + new Vector3(growthAmnt, growthAmnt, growthAmnt);
			owner.mvAvatar.Transform.Rotate(Vector3.up, growthRotationSpeed * Time.deltaTime);
			yield return 0;
		}
		InstaGrow();
	}

	private IEnumerator ShrinkRoutine()
	{
		while (owner.mvAvatar.Scale.y > prevScale.y)
		{
			float shrinkAmnt = shrinkRate * Time.deltaTime;
			owner.mvAvatar.Scale = owner.mvAvatar.Scale - new Vector3(shrinkAmnt, shrinkAmnt, shrinkAmnt);
			yield return 0;
		}
		InstaShrink();
	}

	private void Scale()
	{
		if (!owner.gameObject.activeInHierarchy)
		{
			InstaGrow();
		}
		else
		{
			StartCoroutine(GrowthRoutine());
		}
	}

	private void UnScale()
	{
		if (!owner.gameObject.activeInHierarchy)
		{
			InstaShrink();
		}
		else
		{
			StartCoroutine(ShrinkRoutine());
		}
	}

	private void InstaGrow()
	{
		owner.mvAvatar.Invulnerable.Value = false;
		owner.mvAvatar.Scale = modifiedScale;
		if (owner.IsLocal)
		{
			MVGameControllerBase.WOCM.AvatarLocal.SetAnimation("Idle");
		}
	}

	private void InstaShrink()
	{
		owner.mvAvatar.Scale = prevScale;
		owner.mvAvatar.Body.BodyData.GetPartBone(BodyData.PartIndex.Head).gameObject.SetActive(value: true);
		Destroy();
	}
}
