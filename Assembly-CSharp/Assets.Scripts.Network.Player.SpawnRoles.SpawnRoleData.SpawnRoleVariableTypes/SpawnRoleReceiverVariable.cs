using System;
using Assets.Scripts.Network.Player.SpawnRoles.SpawnRoleData.Mediator;

namespace Assets.Scripts.Network.Player.SpawnRoles.SpawnRoleData.SpawnRoleVariableTypes;

public class SpawnRoleReceiverVariable<T>
{
	private readonly SpawnRoleDataReceiver spawnRoleDataReceiver;

	private readonly SubscribableVariable<T> subscribableVariableExternal;

	protected readonly SubscribableVariable<T> subscribableVariable;

	public T Value
	{
		get
		{
			return subscribableVariable.Value;
		}
		set
		{
			if (!spawnRoleDataReceiver.IsActive)
			{
				throw new Exception("SpawnRole receiver not active. Probably lingering callback.");
			}
			subscribableVariable.ValueSet = value;
		}
	}

	public SpawnRoleReceiverVariable(SubscribableVariable<T> subscribableVariableExternal, SpawnRoleDataReceiver spawnRoleDataReceiver)
	{
		this.subscribableVariableExternal = subscribableVariableExternal;
		subscribableVariable = new SubscribableVariable<T>(subscribableVariableExternal.Value);
		subscribableVariable.OnChange += OnChange;
		this.spawnRoleDataReceiver = spawnRoleDataReceiver;
	}

	private void OnChange(T newValue)
	{
		subscribableVariableExternal.ValueSet = newValue;
	}
}
