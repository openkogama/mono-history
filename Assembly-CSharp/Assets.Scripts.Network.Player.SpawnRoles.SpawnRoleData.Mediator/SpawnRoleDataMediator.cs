using System;
using Assets.Scripts.Network.Player.SpawnRoles.SpawnRoleData.SpawnRoleVariableTypes;
using MV.Common;
using UnityEngine;

namespace Assets.Scripts.Network.Player.SpawnRoles.SpawnRoleData.Mediator;

public class SpawnRoleDataMediator
{
	protected class SpawnRoleDataReceiverInternal : SpawnRoleDataReceiver
	{
		public void DeActivate()
		{
			isActive = false;
		}
	}

	protected class SpawnRoleVariableInternal<T> : SpawnRoleVariable<T>
	{
		public SubscribableVariable<T> SubscribableVariable => subscribableVariable;

		public SpawnRoleVariableInternal(T value)
			: base(value)
		{
		}
	}

	protected SpawnRoleDataReceiverInternal spawnRoleDataReceiver;

	public readonly SpawnRoleModeTypeWrapper SpawnRoleModeTypeWrapper;

	protected readonly SpawnRoleVariableInternal<int> woId = new SpawnRoleVariableInternal<int>(-1);

	protected readonly SpawnRoleVariableInternal<SpawnRoleModeType> spawnRoleMode = new SpawnRoleVariableInternal<SpawnRoleModeType>(SpawnRoleModeType.None);

	protected readonly SpawnRoleVariableInternal<bool> isSeated = new SpawnRoleVariableInternal<bool>(value: false);

	protected readonly SpawnRoleVariableInternal<float> health = new SpawnRoleVariableInternal<float>(0f);

	protected readonly SpawnRoleVariableInternal<int> maxHealth = new SpawnRoleVariableInternal<int>(100);

	protected readonly SpawnRoleVariableInternal<float> shield = new SpawnRoleVariableInternal<float>(0f);

	protected readonly SpawnRoleVariableInternal<bool> isInGunMode = new SpawnRoleVariableInternal<bool>(value: false);

	protected readonly SpawnRoleVariableInternal<bool> isInVehicle = new SpawnRoleVariableInternal<bool>(value: false);

	protected readonly SpawnRoleVariableInternal<Vector3> position = new SpawnRoleVariableInternal<Vector3>(Vector3.zero);

	protected readonly SpawnRoleVariableInternal<Quaternion> rotation = new SpawnRoleVariableInternal<Quaternion>(Quaternion.identity);

	protected readonly SpawnRoleVariableInternal<Vector3> scale = new SpawnRoleVariableInternal<Vector3>(Vector3.one);

	protected readonly SpawnRoleVariableInternal<float> size = new SpawnRoleVariableInternal<float>(1f);

	protected readonly SpawnRoleVariableInternal<bool> pickupItemIsInHand = new SpawnRoleVariableInternal<bool>(value: false);

	protected readonly SpawnRoleVariableInternal<GamePassTier> tierRequirement = new SpawnRoleVariableInternal<GamePassTier>(GamePassTier.Tier0);

	public SpawnRoleVariable<int> WoId => woId;

	public SpawnRoleVariable<SpawnRoleModeType> SpawnRoleMode => spawnRoleMode;

	public SpawnRoleVariable<bool> IsSeated => isSeated;

	public SpawnRoleVariable<float> Health => health;

	public SpawnRoleVariable<int> MaxHealth => maxHealth;

	public SpawnRoleVariable<float> Shield => shield;

	public SpawnRoleVariable<bool> IsInGunMode => isInGunMode;

	public SpawnRoleVariable<bool> IsInVehicle => isInVehicle;

	public SpawnRoleVariable<Vector3> Position => position;

	public SpawnRoleVariable<Quaternion> Rotation => rotation;

	public SpawnRoleVariable<bool> PickupItemIsInHand => pickupItemIsInHand;

	public SpawnRoleVariable<Vector3> Scale => scale;

	public SpawnRoleVariable<GamePassTier> TierRequirement => tierRequirement;

	public SpawnRoleVariable<float> Size => size;

	public event Action<int, int, PlayerKilledByType> OnKilled;

	public event Action OnSuicide;

	public SpawnRoleDataMediator()
	{
		SpawnRoleModeTypeWrapper = new SpawnRoleModeTypeWrapper(SpawnRoleMode);
	}

	public void ActivateSpawnRole(ISpawnRoleLocal currentSpawnRole, ISpawnRoleLocal prevSpawnRole, Vector3 newPosition, Quaternion newRotation)
	{
		DeActivatePrevSpawnRoleDataReceiver(prevSpawnRole);
		SetupNewSpawnRoleDataReceiver();
		int idFrom = -1;
		if (prevSpawnRole != null)
		{
			idFrom = prevSpawnRole.Id;
		}
		currentSpawnRole.Activate(idFrom, spawnRoleDataReceiver, newPosition, newRotation);
	}

	private void DeActivatePrevSpawnRoleDataReceiver(ISpawnRoleLocal prevSpawnRole)
	{
		if (spawnRoleDataReceiver != null)
		{
			prevSpawnRole.DeActivate(prevSpawnRole.Id, spawnRoleDataReceiver);
			spawnRoleDataReceiver.DeActivate();
		}
	}

	private void SetupNewSpawnRoleDataReceiver()
	{
		spawnRoleDataReceiver = new SpawnRoleDataReceiverInternal();
		spawnRoleDataReceiver.woId = new SpawnRoleReceiverVariable<int>(woId.SubscribableVariable, spawnRoleDataReceiver);
		spawnRoleDataReceiver.spawnRoleMode = new SpawnRoleReceiverVariable<SpawnRoleModeType>(spawnRoleMode.SubscribableVariable, spawnRoleDataReceiver);
		spawnRoleDataReceiver.isSeated = new SpawnRoleReceiverVariable<bool>(isSeated.SubscribableVariable, spawnRoleDataReceiver);
		spawnRoleDataReceiver.health = new SpawnRoleReceiverVariable<float>(health.SubscribableVariable, spawnRoleDataReceiver);
		spawnRoleDataReceiver.maxHealth = new SpawnRoleReceiverVariable<int>(maxHealth.SubscribableVariable, spawnRoleDataReceiver);
		spawnRoleDataReceiver.size = new SpawnRoleReceiverVariable<float>(size.SubscribableVariable, spawnRoleDataReceiver);
		spawnRoleDataReceiver.shield = new SpawnRoleReceiverVariable<float>(shield.SubscribableVariable, spawnRoleDataReceiver);
		spawnRoleDataReceiver.isInGunMode = new SpawnRoleReceiverVariable<bool>(isInGunMode.SubscribableVariable, spawnRoleDataReceiver);
		spawnRoleDataReceiver.isInVehicle = new SpawnRoleReceiverVariable<bool>(isInVehicle.SubscribableVariable, spawnRoleDataReceiver);
		spawnRoleDataReceiver.position = new SpawnRoleReceiverVariable<Vector3>(position.SubscribableVariable, spawnRoleDataReceiver);
		spawnRoleDataReceiver.rotation = new SpawnRoleReceiverVariable<Quaternion>(rotation.SubscribableVariable, spawnRoleDataReceiver);
		spawnRoleDataReceiver.scale = new SpawnRoleReceiverVariable<Vector3>(scale.SubscribableVariable, spawnRoleDataReceiver);
		spawnRoleDataReceiver.pickupItemIsInHand = new SpawnRoleReceiverVariable<bool>(pickupItemIsInHand.SubscribableVariable, spawnRoleDataReceiver);
		spawnRoleDataReceiver.tierRequirement = new SpawnRoleReceiverVariable<GamePassTier>(tierRequirement.SubscribableVariable, spawnRoleDataReceiver);
		spawnRoleDataReceiver.OnKilled += SpawnRoleDataReceiverOnOnKilled;
		spawnRoleDataReceiver.OnSuicide += SpawnRoleDataReceiverOnOnSuicide;
	}

	protected void SpawnRoleDataReceiverOnOnKilled(int localPlayerActorNr, int dmgDealerActorNr, PlayerKilledByType damageType)
	{
		if (OnKilled != null)
		{
			OnKilled(localPlayerActorNr, dmgDealerActorNr, damageType);
		}
	}

	protected void SpawnRoleDataReceiverOnOnSuicide()
	{
		if (OnSuicide != null)
		{
			OnSuicide();
		}
	}
}
