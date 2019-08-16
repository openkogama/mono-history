using Assets.Scripts.Network.Player.SpawnRoles.SpawnRoleData.Mediator;
using UnityEngine;

public interface ISpawnRoleLocal
{
	int Id { get; }

	void Activate(int idFrom, SpawnRoleDataReceiver spawnRoleDataReceiver, Vector3 position, Quaternion rotation);

	void DeActivate(int idTo, SpawnRoleDataReceiver spawnRoleDataReceiver);

	void Suspend();

	void UnSuspend();
}
