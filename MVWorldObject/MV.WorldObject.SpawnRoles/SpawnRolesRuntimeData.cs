using System;
using System.Collections.Generic;

namespace MV.WorldObject.SpawnRoles;

public class SpawnRolesRuntimeData
{
	public int activeSpawnRole = -1;

	public HashSet<int> spawnRoleAvatarIds = new HashSet<int>();

	public SpawnRolesRuntimeData()
	{
	}

	public SpawnRolesRuntimeData(int activeSpawnRole, HashSet<int> spawnRoleAvatarIds)
	{
		this.activeSpawnRole = activeSpawnRole;
		this.spawnRoleAvatarIds = spawnRoleAvatarIds;
	}

	public void AddSpawnRole(int id)
	{
		if (!spawnRoleAvatarIds.Add(id))
		{
			throw new Exception("Id already added");
		}
	}

	public void SetActiveSpawnRole(int id)
	{
		if (!spawnRoleAvatarIds.Contains(id))
		{
			throw new Exception("spawn role id not found");
		}
		if (activeSpawnRole == id)
		{
			throw new Exception("Spawn role already active");
		}
		activeSpawnRole = id;
	}

	public void RemoveSpawnRole(int id)
	{
		spawnRoleAvatarIds.Remove(id);
	}

	public override string ToString()
	{
		string text = $"ActiveSpawnRole: {activeSpawnRole}.\n";
		text += "All spawn roles:";
		foreach (int spawnRoleAvatarId in spawnRoleAvatarIds)
		{
			text = text + "\n" + spawnRoleAvatarId;
		}
		return text;
	}
}
