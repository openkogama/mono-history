using System;
using Assets.Scripts.Network.Player.SpawnRoles.SpawnRoleData.SpawnRoleVariableTypes;
using MV.Common;
using UnityEngine;

namespace Assets.Scripts.Network.Player.SpawnRoles.SpawnRoleData.Mediator;

public class SpawnRoleDataReceiver
{
	public SpawnRoleReceiverVariable<int> woId;

	public SpawnRoleReceiverVariable<SpawnRoleModeType> spawnRoleMode;

	public SpawnRoleReceiverVariable<bool> isSeated;

	public SpawnRoleReceiverVariable<float> health;

	public SpawnRoleReceiverVariable<int> maxHealth;

	public SpawnRoleReceiverVariable<float> shield;

	public SpawnRoleReceiverVariable<bool> isInGunMode;

	public SpawnRoleReceiverVariable<bool> pickupItemIsInHand;

	public SpawnRoleReceiverVariable<bool> isInVehicle;

	public SpawnRoleReceiverVariable<Vector3> position;

	public SpawnRoleReceiverVariable<Quaternion> rotation;

	public SpawnRoleReceiverVariable<Vector3> scale;

	public SpawnRoleReceiverVariable<float> size;

	public SpawnRoleReceiverVariable<GamePassTier> tierRequirement;

	protected bool isActive = true;

	public bool IsActive => isActive;

	public event Action<int, int, PlayerKilledByType> OnKilled;

	public event Action OnSuicide;

	public void NotifyKilled(int localPlayerActorNr, int dmgDealerActorNr, PlayerKilledByType damageType)
	{
		if (OnKilled != null)
		{
			OnKilled(localPlayerActorNr, dmgDealerActorNr, damageType);
		}
	}

	public void NotifySuicide()
	{
		if (OnSuicide != null)
		{
			OnSuicide();
		}
	}
}
