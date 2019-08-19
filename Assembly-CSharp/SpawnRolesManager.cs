using MV.WorldObject;
using MV.WorldObject.SpawnRoles;
using UnityEngine;

public class SpawnRolesManager
{
	private readonly SpawnRolesRuntimeData spawnRolesRuntimeData;

	private readonly ISpawnRoleChangeHandler spawnRoleChangeHandler;

	public int SpawnRoleId => spawnRolesRuntimeData.activeSpawnRole;

	public SpawnRolesManager(ISpawnRoleChangeHandler spawnRoleChangeHandler, SpawnRolesRuntimeData spawnRolesRuntimeData)
	{
		this.spawnRolesRuntimeData = spawnRolesRuntimeData;
		this.spawnRoleChangeHandler = spawnRoleChangeHandler;
	}

	public void ActivateSpawnRole(int newSpawnRoleId, Vector3 position, Quaternion rotation)
	{
		int activeSpawnRole = spawnRolesRuntimeData.activeSpawnRole;
		spawnRolesRuntimeData.activeSpawnRole = newSpawnRoleId;
		spawnRoleChangeHandler.ActivateSpawnRole(activeSpawnRole, newSpawnRoleId, position, rotation);
	}

	public void OnAvatarCreated(int id)
	{
		if (id == SpawnRoleId)
		{
			MVWorldObject worldObject = MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObject(id);
			ActivateSpawnRole(id, worldObject.Position, worldObject.Rotation);
		}
	}

	public void AddSpawnRole(int id)
	{
		spawnRolesRuntimeData.AddSpawnRole(id);
	}
}
