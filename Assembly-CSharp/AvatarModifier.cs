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
