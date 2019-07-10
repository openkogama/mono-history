using UnityEngine;

public class SpawnRoleChangeHandlerRemote : ISpawnRoleChangeHandler
{
	public void ActivateSpawnRole(int prevSpawnRoleId, int newSpawnRoleId, Vector3 position, Quaternion rotation)
	{
		ISpawnRoleRemote spawnRoleRemote = (ISpawnRoleRemote)MVGameControllerBase.WOCM.GetWorldObjectClient(prevSpawnRoleId);
		ISpawnRoleRemote spawnRoleRemote2 = (ISpawnRoleRemote)MVGameControllerBase.WOCM.GetWorldObjectClient(newSpawnRoleId);
		spawnRoleRemote.DeActivate(newSpawnRoleId);
		spawnRoleRemote2.Activate(prevSpawnRoleId, position, rotation);
	}
}
