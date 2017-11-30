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
			avatarModifier = PrefabPool.Instance.EnumPoolManager.Instantiate<BurningModifier>(PoolEnums.BurningModifier);
			break;
		case AvatarModifierPackageType.FlamerBurn:
			avatarModifier = PrefabPool.Instance.EnumPoolManager.Instantiate<BurningModifier>(PoolEnums.BurningModifier);
			break;
		case AvatarModifierPackageType.Mutant:
			avatarModifier = PrefabPool.Instance.EnumPoolManager.Instantiate<MutantModifier>(PoolEnums.MutantModifier);
			break;
		case AvatarModifierPackageType.Poison:
			avatarModifier = PrefabPool.Instance.EnumPoolManager.Instantiate<PoisonModifier>(PoolEnums.PoisonModifier);
			break;
		case AvatarModifierPackageType.Frozen:
			avatarModifier = PrefabPool.Instance.EnumPoolManager.Instantiate<FrozenModifier>(PoolEnums.FrozenModifier);
			break;
		case AvatarModifierPackageType.NinjaRun:
			avatarModifier = PrefabPool.Instance.EnumPoolManager.Instantiate<NinjaRunModifier>(PoolEnums.NinjaRunModifier);
			break;
		case AvatarModifierPackageType.Shrunken:
			avatarModifier = PrefabPool.Instance.EnumPoolManager.Instantiate<MouseModifier>(PoolEnums.MouseModifier);
			break;
		case AvatarModifierPackageType.Enlarged:
			avatarModifier = PrefabPool.Instance.EnumPoolManager.Instantiate<GrowthModifier>(PoolEnums.GrowthModifier);
			break;
		case AvatarModifierPackageType.Shielded:
			avatarModifier = Object.Instantiate(PrefabPool.Instance.ShieldModifier);
			break;
		case AvatarModifierPackageType.GodzillaS:
		{
			GodzillaModifier godzillaModifier4 = PrefabPool.Instance.EnumPoolManager.Instantiate<GodzillaModifier>(PoolEnums.GodzillaModifier);
			godzillaModifier4.Type = GodzillaModifier.GodzillaModifierPackageType.S;
			avatarModifier = godzillaModifier4;
			break;
		}
		case AvatarModifierPackageType.GodzillaM:
		{
			GodzillaModifier godzillaModifier3 = PrefabPool.Instance.EnumPoolManager.Instantiate<GodzillaModifier>(PoolEnums.GodzillaModifier);
			godzillaModifier3.Type = GodzillaModifier.GodzillaModifierPackageType.M;
			avatarModifier = godzillaModifier3;
			break;
		}
		case AvatarModifierPackageType.GodzillaL:
		{
			GodzillaModifier godzillaModifier2 = PrefabPool.Instance.EnumPoolManager.Instantiate<GodzillaModifier>(PoolEnums.GodzillaModifier);
			godzillaModifier2.Type = GodzillaModifier.GodzillaModifierPackageType.L;
			avatarModifier = godzillaModifier2;
			break;
		}
		case AvatarModifierPackageType.GodzillaXL:
		{
			GodzillaModifier godzillaModifier = PrefabPool.Instance.EnumPoolManager.Instantiate<GodzillaModifier>(PoolEnums.GodzillaModifier);
			godzillaModifier.Type = GodzillaModifier.GodzillaModifierPackageType.XL;
			avatarModifier = godzillaModifier;
			break;
		}
		case AvatarModifierPackageType.GodzillaLaserBurnS:
		{
			GodzillaLaserBurnModifier godzillaLaserBurnModifier4 = PrefabPool.Instance.EnumPoolManager.Instantiate<GodzillaLaserBurnModifier>(PoolEnums.GodzillaLazerBurnModifier);
			godzillaLaserBurnModifier4.SetType(AvatarModifierPackageType.GodzillaLaserBurnS);
			avatarModifier = godzillaLaserBurnModifier4;
			break;
		}
		case AvatarModifierPackageType.GodzillaLaserBurnM:
		{
			GodzillaLaserBurnModifier godzillaLaserBurnModifier3 = PrefabPool.Instance.EnumPoolManager.Instantiate<GodzillaLaserBurnModifier>(PoolEnums.GodzillaLazerBurnModifier);
			godzillaLaserBurnModifier3.SetType(AvatarModifierPackageType.GodzillaLaserBurnM);
			avatarModifier = godzillaLaserBurnModifier3;
			break;
		}
		case AvatarModifierPackageType.GodzillaLaserBurnL:
		{
			GodzillaLaserBurnModifier godzillaLaserBurnModifier2 = PrefabPool.Instance.EnumPoolManager.Instantiate<GodzillaLaserBurnModifier>(PoolEnums.GodzillaLazerBurnModifier);
			godzillaLaserBurnModifier2.SetType(AvatarModifierPackageType.GodzillaLaserBurnL);
			avatarModifier = godzillaLaserBurnModifier2;
			break;
		}
		case AvatarModifierPackageType.GodzillaLaserBurnXL:
		{
			GodzillaLaserBurnModifier godzillaLaserBurnModifier = PrefabPool.Instance.EnumPoolManager.Instantiate<GodzillaLaserBurnModifier>(PoolEnums.GodzillaLazerBurnModifier);
			godzillaLaserBurnModifier.SetType(AvatarModifierPackageType.GodzillaLaserBurnXL);
			avatarModifier = godzillaLaserBurnModifier;
			break;
		}
		case AvatarModifierPackageType.GodzillaGrowthInvulnerability:
		{
			InvulnerabilityModifier invulnerabilityModifier2 = PrefabPool.Instance.EnumPoolManager.Instantiate<InvulnerabilityModifier>(PoolEnums.InvulnerabilityModifier);
			invulnerabilityModifier2.SetType(AvatarModifierPackageType.GodzillaGrowthInvulnerability);
			avatarModifier = invulnerabilityModifier2;
			break;
		}
		case AvatarModifierPackageType.SpawnProtection:
		{
			InvulnerabilityModifier invulnerabilityModifier = PrefabPool.Instance.EnumPoolManager.Instantiate<InvulnerabilityModifier>(PoolEnums.InvulnerabilityModifier);
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
