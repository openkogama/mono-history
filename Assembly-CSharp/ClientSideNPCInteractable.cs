using System;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class ClientSideNPCInteractable : MVInteractableBase
{
	private Action<float, MVPlayer, PlayerKilledByType> takeDamageCallback;

	private int respawnInterval;

	public void Init(Action<float, MVPlayer, PlayerKilledByType> takeDamageCallback)
	{
		this.takeDamageCallback = takeDamageCallback;
		respawnInterval = (int)SharedWorldObjectValuesRepository.GetValues(worldObjectParent.WorldObjectType)["RespawnInterval"];
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
			worldObjectParent.RunTimeData.SetObscuredType("health", (ObscuredFloat)num);
			if (num < 0f)
			{
				num = (float)RuntimeVariablesRepository.GetRuntimeVariables(worldObjectParent.WorldObjectType)["health"];
				worldObjectParent.RunTimeData.SetObscuredType("deathTime", (ObscuredInt)WaitForTicks.GetEnvironmentTick(0));
				Debug.Log("TakeDamageKilled");
			}
			takeDamageCallback(amount, damageDealer, damageType);
			worldObjectParent.RunTimeData.SetObscuredType("health", (ObscuredFloat)num);
		}
	}

	public void Reset()
	{
		worldObjectParent.RunTimeData = RuntimeVariablesRepository.GetRuntimeVariables(worldObjectParent.WorldObjectType);
		worldObjectParent.RunTimeData.SetObscuredType("deathTime", (ObscuredInt)WaitForTicks.GetEnvironmentTick(-respawnInterval));
	}

	public override void AddModifier(AvatarModifierPackageType type, int id, AvatarModifierPackage.AvatarModifier[] additionalModifers)
	{
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

	public override float HandleModifierEffect(AvatarModifierEffect avatarModifierEffect, float baseValue)
	{
		return baseValue;
	}

	public override void ClearModifiers()
	{
		Debug.Log(" ignore ClearModifiers");
	}
}
