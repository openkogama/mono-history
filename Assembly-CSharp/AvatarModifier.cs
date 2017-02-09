using System.Collections.Generic;
using UnityEngine;

public abstract class AvatarModifier : MonoBehaviour
{
	protected float timeStamp;

	private bool isActivated;

	protected Avatar owner;

	public bool IsActivated => isActivated;

	public bool IsLocal => owner.IsLocal;

	public Avatar Owner => owner;

	public abstract AvatarModifierPackageType ModifierType { get; }

	public static AvatarModifier CreateFromType(AvatarModifierPackageType type, Avatar owner)
	{
		AvatarModifier avatarModifier;
		switch (type)
		{
		case AvatarModifierPackageType.Fire:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.BurningModifier);
			break;
		case AvatarModifierPackageType.FlamerBurn:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.BurningModifier);
			break;
		case AvatarModifierPackageType.Mutant:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.MutantModifier);
			break;
		case AvatarModifierPackageType.Poison:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.PoisonModifier);
			break;
		case AvatarModifierPackageType.Frozen:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.FrozenModifier);
			break;
		case AvatarModifierPackageType.NinjaRun:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.NinjaRunModifier);
			break;
		case AvatarModifierPackageType.Shrunken:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.MouseModifier);
			break;
		case AvatarModifierPackageType.Enlarged:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.GrowthModifier);
			break;
		case AvatarModifierPackageType.Shielded:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.ShieldModifier);
			break;
		case AvatarModifierPackageType.GodzillaS:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.GodzillaModifierS);
			break;
		case AvatarModifierPackageType.GodzillaM:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.GodzillaModifierM);
			break;
		case AvatarModifierPackageType.GodzillaL:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.GodzillaModifierL);
			break;
		case AvatarModifierPackageType.GodzillaXL:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.GodzillaModifierXL);
			break;
		case AvatarModifierPackageType.GodzillaLaserBurnS:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.GodzillaLaserBurnModifierS);
			break;
		case AvatarModifierPackageType.GodzillaLaserBurnM:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.GodzillaLaserBurnModifierM);
			break;
		case AvatarModifierPackageType.GodzillaLaserBurnL:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.GodzillaLaserBurnModifierL);
			break;
		case AvatarModifierPackageType.GodzillaLaserBurnXL:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.GodzillaLaserBurnModifierXL);
			break;
		case AvatarModifierPackageType.SpawnProtection:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.SpawnProtectionModifier);
			break;
		case AvatarModifierPackageType.GodzillaGrowthInvulnerability:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.GodzillaGrowthInvulnerabilityModifier);
			break;
		default:
			return null;
		}
		avatarModifier.owner = owner;
		return avatarModifier;
	}

	public virtual bool EvaluateShouldBeAdded(Dictionary<AvatarModifierPackageType, AvatarModifier> modifiers)
	{
		return true;
	}

	public virtual void ResetTimeStamp()
	{
		timeStamp = Time.time;
	}

	public void Activate(Avatar target)
	{
		isActivated = true;
		OnActivated(target);
	}

	public void Deactivate(Avatar target)
	{
		isActivated = false;
		OnDeactivated(target);
	}

	protected virtual void OnActivated(Avatar target)
	{
	}

	protected virtual void OnDeactivated(Avatar target)
	{
	}
}
