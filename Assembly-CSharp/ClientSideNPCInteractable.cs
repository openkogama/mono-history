using System;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class ClientSideNPCInteractable : MVInteractableBase
{
	private Action<float> takeDamageCallback;

	private int respawnInterval;

	public void Init(Action<float> takeDamageCallback)
	{
		this.takeDamageCallback = takeDamageCallback;
		respawnInterval = (int)SharedWorldObjectValuesRepository.GetValues(worldObjectParent.WorldObjectType)["RespawnInterval"];
	}

	public bool IsDead()
	{
		return respawnInterval > WaitForTicks.Diff((int)worldObjectParent.RunTimeData["deathTime"]);
	}

	public override void TakeDamage(float amount, MVPlayer damageDealer, PlayerKilledByType damageType)
	{
		if (!IsDead())
		{
			float num = (float)worldObjectParent.RunTimeData["health"];
			num -= amount;
			worldObjectParent.RunTimeData["health"] = num;
			if (num < 0f)
			{
				num = (float)RuntimeVariablesRepository.GetRuntimeVariables(worldObjectParent.WorldObjectType)["health"];
				worldObjectParent.RunTimeData["deathTime"] = WaitForTicks.GetEnvironmentTick(0);
			}
			takeDamageCallback(amount);
			worldObjectParent.RunTimeData["health"] = num;
		}
	}

	public void Reset()
	{
		worldObjectParent.RunTimeData = RuntimeVariablesRepository.GetRuntimeVariables(worldObjectParent.WorldObjectType);
		worldObjectParent.RunTimeData["deathTime"] = WaitForTicks.GetEnvironmentTick(-respawnInterval);
	}

	public override void AddModifier(AvatarModifierPackageType type, int id, AvatarModifierPackage.AvatarModifier[] additionalModifers)
	{
		Debug.Log((object)"Ignore add modifier");
	}

	public override bool HasModifier(AvatarModifierPackageType type)
	{
		return false;
	}

	public override void RemoveModifier(AvatarModifierPackageType type, int id)
	{
		Debug.Log((object)"Ignore remove modifier");
	}

	public override float HandleModifierEffect(AvatarModifierEffect avatarModifierEffect, float baseValue)
	{
		return baseValue;
	}

	public override void ClearModifiers()
	{
		Debug.Log((object)" ignore ClearModifiers");
	}
}
