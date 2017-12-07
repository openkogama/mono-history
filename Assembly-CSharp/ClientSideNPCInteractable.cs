using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class ClientSideNPCInteractable : MVInteractableBase
{
	private static readonly Dictionary<AvatarModifierPackageType, float> allowedModifiersDictionary = new Dictionary<AvatarModifierPackageType, float>
	{
		{
			AvatarModifierPackageType.RayHeal,
			-1f
		},
		{
			AvatarModifierPackageType.FlamerBurn,
			1.8f
		}
	};

	private Action<float, MVPlayer, PlayerKilledByType> takeDamageCallback;

	private int respawnInterval;

	private float maxHealth;

	public void Init(Action<float, MVPlayer, PlayerKilledByType> takeDamageCallback)
	{
		this.takeDamageCallback = takeDamageCallback;
		respawnInterval = (int)SharedWorldObjectValuesRepository.GetValues(worldObjectParent.WorldObjectType)["RespawnInterval"];
		maxHealth = (ObscuredFloat)worldObjectParent.RunTimeData.GetObscuredType("health");
	}

	public bool IsDead()
	{
		return respawnInterval > WaitForTicks.Diff((ObscuredInt)worldObjectParent.RunTimeData.GetObscuredType("deathTime"));
	}

	public override void TakeDamage(float amount, MVPlayer damageDealer, PlayerKilledByType damageType)
	{
		if (!IsDead())
		{
			float num = (ObscuredFloat)worldObjectParent.RunTimeData.GetObscuredType("health");
			num -= amount;
			if (num > maxHealth)
			{
				num = maxHealth;
			}
			worldObjectParent.RunTimeData.SetObscuredType("health", (ObscuredFloat)num);
			if (num <= 0f)
			{
				num = (float)RuntimeVariablesRepository.GetRuntimeVariables(worldObjectParent.WorldObjectType)["health"];
				worldObjectParent.RunTimeData.SetObscuredType("deathTime", (ObscuredInt)WaitForTicks.GetEnvironmentTick(0));
			}
			takeDamageCallback(amount, damageDealer, damageType);
			worldObjectParent.RunTimeData.SetObscuredType("health", (ObscuredFloat)num);
		}
	}

	public void Reset()
	{
		Dictionary<object, object> dictionary = (Dictionary<object, object>)ObscuredTypesConverter.CreateUnObscuredValue(worldObjectParent.RunTimeData);
		RuntimeVariablesRepository.SetupRuntimeVariable(worldObjectParent.WorldObjectType, dictionary);
		worldObjectParent.RunTimeData = dictionary;
		worldObjectParent.RunTimeData.SetObscuredType("deathTime", (ObscuredInt)WaitForTicks.GetEnvironmentTick(-respawnInterval));
	}

	public override void AddModifier(AvatarModifierPackageType type, int id, AvatarModifierPackage.AvatarModifier[] additionalModifers)
	{
		if (allowedModifiersDictionary.ContainsKey(type))
		{
			MVPlayer playerUnsafe = MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(id);
			if (playerUnsafe != null)
			{
				TakeDamage(allowedModifiersDictionary[type], MVGameControllerBase.Game.MVPlayerContainer.GetPlayerUnsafe(id), PlayerKilledByType.None);
				return;
			}
		}
		Debug.Log("Ignore add modifier");
	}

	public override bool HasModifier(AvatarModifierPackageType type)
	{
		return false;
	}

	public override void RemoveModifier(AvatarModifierPackageType type, int id)
	{
		Debug.Log("Ignore remove modifier");
	}

	public override bool HasModifierEffect(AvatarModifierEffect type)
	{
		return false;
	}

	public override float HandleModifierEffect(AvatarModifierEffect avatarModifierEffect, float baseValue)
	{
		return baseValue;
	}

	public override void ClearModifiers()
	{
		Debug.Log(" ignore ClearModifiers");
	}
}
