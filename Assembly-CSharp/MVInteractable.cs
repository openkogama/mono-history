using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public abstract class MVInteractable : MVInteractableBase
{
	protected AvatarModifierPackages modifierPackages = new AvatarModifierPackages();

	protected MVRuntimeDataVariable runtimeDataModifiers;

	protected MVRuntimeDataVariable<float> health;

	protected MVRuntimeDataVariable<int> maxHealth;

	protected MVRuntimeDataVariableClampedFloat shield;

	private float healthRegenerationPerSecondSetting;

	private float shieldRegenerationPerSecondSetting;

	public AvatarModifierPackages ModifierPackages => modifierPackages;

	public virtual void Init(MVRuntimeDataVariable runtimeDataModifiers, MVRuntimeDataVariable<float> health, MVRuntimeDataVariable<int> maxHealth, MVRuntimeDataVariableClampedFloat shield, WorldObjectSkillDataManager skillDataManager)
	{
		this.runtimeDataModifiers = runtimeDataModifiers;
		this.health = health;
		this.maxHealth = maxHealth;
		this.shield = shield;
		AvatarModifierPackages avatarModifierPackages = modifierPackages;
		avatarModifierPackages.OnModifierExpired = (AvatarModifierPackages.OnModifierExpiredDelegate)Delegate.Combine(avatarModifierPackages.OnModifierExpired, (AvatarModifierPackages.OnModifierExpiredDelegate)((AvatarModifierPackage modifier) =>
		{
			RemoveModifier(modifier.AvatarModifierPackageType, modifier.id);
		}));
	}

	public virtual void InitializeSetings(float healthRegenerationPerSecondSetting, float shieldRegenerationPerSecondSetting)
	{
		this.healthRegenerationPerSecondSetting = healthRegenerationPerSecondSetting;
		this.shieldRegenerationPerSecondSetting = shieldRegenerationPerSecondSetting;
	}

	protected virtual void RestoreShield(float restoredShieldAmount)
	{
		shield.Value += restoredShieldAmount;
	}

	protected bool IgnoreDamage(MVPlayer damageDealer)
	{
		MVTeam teamFromActorNr = MVGameControllerBase.Game.TeamManager.GetTeamFromActorNr(worldObjectParent.OwnerActorNr);
		if ((MVGameControllerBase.Game.TeamManager.TeamCount() > 1 && damageDealer != null && teamFromActorNr == damageDealer.Team && teamFromActorNr != MVTeam.None && MVGameControllerBase.Game.LocalPlayer.ActorNr != damageDealer.ActorNr) || !MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Playing))
		{
			return true;
		}
		return false;
	}

	protected bool IgnoreHealing(MVPlayer damageDealer)
	{
		MVTeam teamFromActorNr = MVGameControllerBase.Game.TeamManager.GetTeamFromActorNr(worldObjectParent.OwnerActorNr);
		if ((MVGameControllerBase.Game.TeamManager.TeamCount() > 1 && damageDealer != null && teamFromActorNr != damageDealer.Team && teamFromActorNr != MVTeam.None && MVGameControllerBase.Game.LocalPlayer.ActorNr != damageDealer.ActorNr) || !MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Playing))
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
				TakeDamage(num, MVGameControllerBase.Game.MVPlayerContainer[key], PlayerKilledByType.FlameThrower);
			}
		}
		float num2 = HandleModifierEffect(AvatarModifierEffect.EnvironmentDamagePrSec, 0f) * Time.deltaTime;
		if (num2 != 0f)
		{
			TakeDamage(num2, null, PlayerKilledByType.Environmental);
		}
		float num3 = healthRegenerationPerSecondSetting * Time.deltaTime;
		if (num3 != 0f)
		{
			TakeDamage(0f - num3, null, PlayerKilledByType.None);
		}
		float num4 = shieldRegenerationPerSecondSetting * Time.deltaTime;
		if (num4 != 0f)
		{
			RestoreShield(num4);
		}
	}

	public override void AddModifier(AvatarModifierPackageType type, int id = -1, AvatarModifierPackage.AvatarModifier[] additionalModifers = null)
	{
		ModifierActions actionToTakeWithPackageType = modifierPackages.GetActionToTakeWithPackageType(type);
		Dictionary<object, object> dictionary = new Dictionary<object, object>((Dictionary<object, object>)runtimeDataModifiers.Value);
		string key = AvatarModifierPackage.AvatarModifierPackageTypeLookupTable[(int)type];
		switch (actionToTakeWithPackageType)
		{
		case ModifierActions.Add:
			if (!dictionary.ContainsKey(key))
			{
				modifierPackages.AddModifier(type, id, additionalModifers);
				dictionary.Add(key, (byte)0);
				runtimeDataModifiers.Value = dictionary;
			}
			break;
		case ModifierActions.Renew:
			if (dictionary.ContainsKey(key))
			{
				modifierPackages.AddModifier(type, id, additionalModifers);
				byte b = (byte)((byte)dictionary[key] + 1);
				dictionary[key] = b;
				runtimeDataModifiers.Value = dictionary;
			}
			break;
		case ModifierActions.Replace:
			if (!dictionary.ContainsKey(key))
			{
				modifierPackages.AddModifier(type, id, additionalModifers);
				AvatarModifierPackageType packageToActWith = modifierPackages.GetPackageToActWith(type, actionToTakeWithPackageType);
				RemoveModifier(packageToActWith);
				dictionary.Remove("_" + packageToActWith);
				dictionary.Add(key, (byte)0);
				runtimeDataModifiers.Value = dictionary;
			}
			break;
		case ModifierActions.CancelOut:
			RemoveModifier(modifierPackages.GetPackageToActWith(type, actionToTakeWithPackageType), id);
			break;
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
		modifierPackages.ClearNonPersistantModifiers();
	}

	public override bool HasModifierEffect(AvatarModifierEffect avatarModifierEffect)
	{
		return modifierPackages.HasModifierEffect(avatarModifierEffect);
	}

	public override float HandleModifierEffect(AvatarModifierEffect avatarModifierEffect, float baseValue)
	{
		return modifierPackages.HandleModifierEffect(avatarModifierEffect, baseValue);
	}
}
