using UnityEngine;

public interface ISpawnRoleRemote
{
	int Id { get; }

	void Activate(int idFrom, Vector3 position, Quaternion rotation);

	void DeActivate(int idTo);
}
