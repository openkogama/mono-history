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
		{
			GodzillaModifier godzillaModifier4 = Object.Instantiate(PrefabPool.Instance.GodzillaModifier);
			godzillaModifier4.Type = GodzillaModifier.GodzillaModifierPackageType.S;
			avatarModifier = godzillaModifier4;
			break;
		}
		case AvatarModifierPackageType.GodzillaM:
		{
			GodzillaModifier godzillaModifier3 = Object.Instantiate(PrefabPool.Instance.GodzillaModifier);
			godzillaModifier3.Type = GodzillaModifier.GodzillaModifierPackageType.M;
			avatarModifier = godzillaModifier3;
			break;
		}
		case AvatarModifierPackageType.GodzillaL:
		{
			GodzillaModifier godzillaModifier2 = Object.Instantiate(PrefabPool.Instance.GodzillaModifier);
			godzillaModifier2.Type = GodzillaModifier.GodzillaModifierPackageType.L;
			avatarModifier = godzillaModifier2;
			break;
		}
		case AvatarModifierPackageType.GodzillaXL:
		{
			GodzillaModifier godzillaModifier = Object.Instantiate(PrefabPool.Instance.GodzillaModifier);
			godzillaModifier.Type = GodzillaModifier.GodzillaModifierPackageType.XL;
			avatarModifier = godzillaModifier;
			break;
		}
		case AvatarModifierPackageType.GodzillaLaserBurnS:
		{
			GodzillaLaserBurnModifier godzillaLaserBurnModifier4 = Object.Instantiate(PrefabPool.Instance.GodzillaLaserBurnModifier);
			godzillaLaserBurnModifier4.SetType(AvatarModifierPackageType.GodzillaLaserBurnS);
			avatarModifier = godzillaLaserBurnModifier4;
			break;
		}
		case AvatarModifierPackageType.GodzillaLaserBurnM:
		{
			GodzillaLaserBurnModifier godzillaLaserBurnModifier3 = Object.Instantiate(PrefabPool.Instance.GodzillaLaserBurnModifier);
			godzillaLaserBurnModifier3.SetType(AvatarModifierPackageType.GodzillaLaserBurnM);
			avatarModifier = godzillaLaserBurnModifier3;
			break;
		}
		case AvatarModifierPackageType.GodzillaLaserBurnL:
		{
			GodzillaLaserBurnModifier godzillaLaserBurnModifier2 = Object.Instantiate(PrefabPool.Instance.GodzillaLaserBurnModifier);
			godzillaLaserBurnModifier2.SetType(AvatarModifierPackageType.GodzillaLaserBurnL);
			avatarModifier = godzillaLaserBurnModifier2;
			break;
		}
		case AvatarModifierPackageType.GodzillaLaserBurnXL:
		{
			GodzillaLaserBurnModifier godzillaLaserBurnModifier = Object.Instantiate(PrefabPool.Instance.GodzillaLaserBurnModifier);
			godzillaLaserBurnModifier.SetType(AvatarModifierPackageType.GodzillaLaserBurnXL);
			avatarModifier = godzillaLaserBurnModifier;
			break;
		}
		case AvatarModifierPackageType.GodzillaGrowthInvulnerability:
		{
			InvulnerabilityModifier invulnerabilityModifier2 = Object.Instantiate(PrefabPool.Instance.InvulnerabilityModifier);
			invulnerabilityModifier2.SetType(AvatarModifierPackageType.GodzillaGrowthInvulnerability);
			avatarModifier = invulnerabilityModifier2;
			break;
		}
		case AvatarModifierPackageType.SpawnProtection:
		{
			InvulnerabilityModifier invulnerabilityModifier = Object.Instantiate(PrefabPool.Instance.InvulnerabilityModifier);
			invulnerabilityModifier.SetType(AvatarModifierPackageType.SpawnProtection);
			avatarModifier = invulnerabilityModifier;
			break;
		}
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
