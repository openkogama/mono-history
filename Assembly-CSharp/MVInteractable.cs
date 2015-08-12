using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public abstract class MVInteractable : MVInteractableBase
{
	protected AvatarModifierPackages modifierPackages = new AvatarModifierPackages();

	protected MVRuntimeDataVariable runtimeDataModifiers;

	protected MVRuntimeDataVariableClampedFloat health;

	public void Init(MVRuntimeDataVariable runtimeDataModifiers, MVRuntimeDataVariableClampedFloat health)
	{
		this.runtimeDataModifiers = runtimeDataModifiers;
		this.health = health;
		AvatarModifierPackages avatarModifierPackages = modifierPackages;
		avatarModifierPackages.OnModifierExpired = (AvatarModifierPackages.OnModifierExpiredDelegate)Delegate.Combine(avatarModifierPackages.OnModifierExpired, (AvatarModifierPackages.OnModifierExpiredDelegate)((AvatarModifierPackage modifier) =>
		{
			RemoveModifier(modifier.AvatarModifierPackageType, modifier.id);
		}));
	}

	protected bool IgnoreDamage(MVPlayer damageDealer)
	{
		MVTeam teamFromActorNr = MVGameController.Game.TeamManager.GetTeamFromActorNr(worldObjectParent.OwnerActorNr);
		if (MVGameController.Game.TeamManager.TeamCount() > 1 && damageDealer != null && teamFromActorNr == damageDealer.Team && MVGameController.Game.LocalPlayer.ActorNr != damageDealer.ActorNr)
		{
			return true;
		}
		return false;
	}

	private void Update()
	{
		modifierPackages.Update();
		Dictionary<int, float> dictionary = modifierPackages.ComputeModifierEffectGroupedById(AvatarModifierEffect.FlamerDamagePrSec, 0f);
		foreach (KeyValuePair<int, float> item in dictionary)
		{
			int key = item.Key;
			float num = item.Value * Time.deltaTime;
			if (num != 0f)
			{
				TakeDamage(num, MVGameController.Game.Players[key], PlayerKilledByType.FlameThrower);
			}
		}
		float num2 = HandleModifierEffect(AvatarModifierEffect.EnvironmentDamagePrSec, 0f) * Time.deltaTime;
		if (num2 != 0f)
		{
			TakeDamage(num2, null, PlayerKilledByType.Environmental);
		}
	}

	public override void AddModifier(AvatarModifierPackageType type, int id = -1, AvatarModifierPackage.AvatarModifier[] additionalModifers = null)
	{
		modifierPackages.AddModifier(type, id, additionalModifers);
		Dictionary<object, object> dictionary = new Dictionary<object, object>((Dictionary<object, object>)runtimeDataModifiers.Value);
		string key = "_" + type;
		if (!dictionary.ContainsKey(key))
		{
			dictionary.Add(key, (byte)0);
			runtimeDataModifiers.Value = dictionary;
		}
	}

	public override bool HasModifier(AvatarModifierPackageType type)
	{
		return modifierPackages.HasModifier(type);
	}

	public override void RemoveModifier(AvatarModifierPackageType type, int id = -1)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>((Dictionary<object, object>)runtimeDataModifiers.Value);
		string key = "_" + type;
		if (dictionary.ContainsKey(key))
		{
			dictionary.Remove(key);
			runtimeDataModifiers.Value = dictionary;
			modifierPackages.RemoveModifier(type, id);
		}
	}

	public override void ClearModifiers()
	{
		modifierPackages.ClearModifiers();
	}

	public override float HandleModifierEffect(AvatarModifierEffect avatarModifierEffect, float baseValue)
	{
		return modifierPackages.HandleModifierEffect(avatarModifierEffect, baseValue);
	}
}
