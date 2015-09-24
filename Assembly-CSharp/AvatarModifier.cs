using UnityEngine;

public abstract class AvatarModifier : MonoBehaviour
{
	private bool isActivated;

	protected Avatar owner;

	public bool IsActivated => isActivated;

	public bool IsLocal => owner.IsLocal;

	public Avatar Owner => owner;

	public abstract AvatarModifierPackageType ModifierType { get; }

	public static AvatarModifier CreateFromType(AvatarModifierPackageType type, Avatar owner)
	{
		string empty = string.Empty;
		switch (type)
		{
		case AvatarModifierPackageType.Fire:
			empty = "Prefabs/AvatarModifiers/BurningModifier";
			break;
		case AvatarModifierPackageType.FlamerBurn:
			empty = "Prefabs/AvatarModifiers/BurningModifier";
			break;
		case AvatarModifierPackageType.Mutant:
			empty = "Prefabs/AvatarModifiers/MutantModifier";
			break;
		case AvatarModifierPackageType.Poison:
			empty = "Prefabs/AvatarModifiers/PoisonModifier";
			break;
		case AvatarModifierPackageType.Frozen:
			empty = "Prefabs/AvatarModifiers/FrozenModifier";
			break;
		case AvatarModifierPackageType.NinjaRun:
			empty = "Prefabs/AvatarModifiers/NinjaRunModifier";
			break;
		case AvatarModifierPackageType.Shrunken:
			empty = "Prefabs/AvatarModifiers/ShrinkModifier";
			break;
		default:
			return null;
		}
		Debug.Log(empty);
		AvatarModifier component = (Object.Instantiate(Resources.Load(empty)) as GameObject).GetComponent<AvatarModifier>();
		component.owner = owner;
		return component;
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
