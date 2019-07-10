using UnityEngine;

public interface ISpawnRoleChangeHandler
{
	void ActivateSpawnRole(int prevSpawnRoleId, int newSpawnRoleId, Vector3 position, Quaternion rotation);
}
