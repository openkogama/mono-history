using Assets.Scripts.Network.Player.SpawnRoles.SpawnRoleData.Mediator;
using UnityEngine;

public class SpawnRoleChangeHandlerLocal : ISpawnRoleChangeHandler
{
	private readonly SpawnRoleDataMediator SpawnRoleDataMediator = new SpawnRoleDataMediator();

	public SpawnRoleChangeHandlerLocal(SpawnRoleDataMediator spawnRoleDataMediator)
	{
		SpawnRoleDataMediator = spawnRoleDataMediator;
	}

	public void ActivateSpawnRole(int prevSpawnRoleId, int newSpawnRoleId, Vector3 position, Quaternion rotation)
	{
		ISpawnRoleLocal prevSpawnRole = (ISpawnRoleLocal)MVGameControllerBase.WOCM.GetWorldObjectClient(prevSpawnRoleId);
		ISpawnRoleLocal currentSpawnRole = (ISpawnRoleLocal)MVGameControllerBase.WOCM.GetWorldObjectClient(newSpawnRoleId);
		SpawnRoleDataMediator.ActivateSpawnRole(currentSpawnRole, prevSpawnRole, position, rotation);
	}
}
