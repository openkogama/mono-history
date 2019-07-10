using System;
using Assets.Scripts.Network.Player.SpawnRoles.SpawnRoleData.SpawnRoleVariableTypes;
using MV.Common;

public class SpawnRoleModeTypeWrapper
{
	private readonly SpawnRoleVariable<SpawnRoleModeType> spawnRoleType;

	public event Action<SpawnRoleModeType> OnChange;

	public SpawnRoleModeTypeWrapper(SpawnRoleVariable<SpawnRoleModeType> spawnRoleType)
	{
		this.spawnRoleType = spawnRoleType;
		spawnRoleType.OnChange += OnChangeInternal;
	}

	public bool IsInMode(SpawnRoleModeType t)
	{
		return (spawnRoleType.Value & t) > SpawnRoleModeType.None;
	}

	private void OnChangeInternal(SpawnRoleModeType obj)
	{
		if (OnChange != null)
		{
			OnChange(obj);
		}
	}
}
