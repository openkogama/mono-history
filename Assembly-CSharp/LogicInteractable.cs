using System;
using MV.Common;

public class LogicInteractable : MVInteractableBase
{
	public event EventHandler<TakeDamageEventArgs> OnDamageEvent;

	public override void TakeDamage(float amount, MVPlayer damageDealer, PlayerKilledByType damageType)
	{
		if (OnDamageEvent != null)
		{
			OnDamageEvent(this, new TakeDamageEventArgs(amount, damageDealer, damageType));
		}
	}

	public override void AddModifier(AvatarModifierPackageType type, int id = -1, AvatarModifierPackage.AvatarModifier[] additionalModifers = null)
	{
	}

	public override bool HasModifier(AvatarModifierPackageType type)
	{
		return false;
	}

	public override void RemoveModifier(AvatarModifierPackageType type, int id = -1)
	{
	}

	public override float HandleModifierEffect(AvatarModifierEffect avatarModifierEffect, float baseValue)
	{
		return baseValue;
	}

	public override void ClearModifiers()
	{
	}

	public override bool HasModifierEffect(AvatarModifierEffect avatarModifierEffect)
	{
		return false;
	}
}
